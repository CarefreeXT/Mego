// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.

namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    /// <summary>
    /// 关系元数据。
    /// </summary>
    public class NavigateMetadata
    {
        private Dictionary<Type, IColumnAnnotation> _Attributes;
        /// <summary>
        /// 创建关系元数据。
        /// </summary>
        /// <param name="source">关系源表元数据。</param>
        /// <param name="target">关系目标表元数据。</param>
        /// <param name="pairs">关系主外键对集合</param>
        public NavigateMetadata(TableMetadata source, TableMetadata target, ForeignPrincipalPair[] pairs)
        {
            Source = source;
            Target = target;
            Pairs = pairs;
        }
        /// <summary>
        /// 是否为复合导航成员。
        /// </summary>
        public virtual bool IsComposite => false;
        /// <summary>
        /// 导航属性是否为外键导航，即导航属性的目标类型是否为当前关系的主体。为空则表示不明确。
        /// </summary>
        public bool? IsForeign { get; internal set; }
        /// <summary>
        /// 导航所在类型。
        /// </summary>
        public TableMetadata Source { get; }
        /// <summary>
        /// 导航目标类型。
        /// </summary>
        public TableMetadata Target { get; }
        /// <summary>
        /// 外键主键属性对。
        /// </summary>
        public ForeignPrincipalPair[] Pairs { get; }
        /// <summary>
        /// 初始化关系元数据。
        /// </summary>
        /// <param name="sourceType">关系源表元数据。</param>
        /// <param name="targetType">目标表元数据。</param>
        /// <param name="propertyInfo">关系属性的CLR描述对象。</param>
        /// <param name="isCollection">是否为集合属性。</param>
        internal void Initial(TypeMetadata sourceType,
            TypeMetadata targetType, PropertyInfo propertyInfo, bool isCollection)
        {
            _Attributes = propertyInfo.GetCustomAttributes()
                .OfType<IColumnAnnotation>().ToDictionary(a => a.GetType(), a => a);

            var index = sourceType.ComplexMembers.IndexOf(propertyInfo);
            if (isCollection)
            {
                UpdateComplexMember = (source, target) =>
                {
                    var collection = sourceType.GetComplexProperty(source, index);
                    if (target == null)
                    {
                        if (collection != null)
                        {
                            targetType.CollectionRemove(source, target);
                        }
                    }
                    else
                    {
                        if (collection == null)
                        {
                            collection = Activator.CreateInstance(typeof(HashSet<>).MakeGenericType(targetType.ClrType));
                            sourceType.SetComplexProperty(source, index, collection);
                        }
                        targetType.CollectionAdd(collection, target);
                    }
                };
            }
            else
            {
                UpdateComplexMember = (source, target) => sourceType.SetComplexProperty(source, index, target);
            }

            if (!IsComposite && IsForeign.HasValue)
            {
                if (IsForeign.Value)
                {
                    var foreignIndexs = Pairs.Select(a => sourceType.PrimaryMembers.IndexOf(a.ForeignKey.Member)).ToArray();
                    var primaryIndexs = Pairs.Select(a => targetType.PrimaryMembers.IndexOf(a.PrincipalKey.Member)).ToArray();
                    CreateForeignKey = (source, target) =>
                    {
                        var values = new object[primaryIndexs.Length];
                        targetType.GetProperty(target, primaryIndexs, values);
                        for (int i = 0; i < foreignIndexs.Length; i++)
                        {
                            sourceType.SetProperty(source, foreignIndexs[i], values[i]);
                        }
                    };
                    DeleteForeignKey = (source, target) => foreignIndexs.ForEach(i => sourceType.SetProperty(source, foreignIndexs[i], null));
                }
                else
                {
                    var foreignIndexs = Pairs.Select(a => targetType.PrimaryMembers.IndexOf(a.ForeignKey.Member)).ToArray();
                    var primaryIndexs = Pairs.Select(a => sourceType.PrimaryMembers.IndexOf(a.PrincipalKey.Member)).ToArray();
                    CreateForeignKey = (source, target) =>
                    {
                        var values = new object[primaryIndexs.Length];
                        sourceType.GetProperty(source, primaryIndexs, values);
                        for (int i = 0; i < foreignIndexs.Length; i++)
                        {
                            targetType.SetProperty(target, foreignIndexs[i], values[i]);
                        }
                    };
                    DeleteForeignKey = (source, target) =>
                    {
                        if (target == null)
                        {
                            target = sourceType.GetComplexProperty(source, index);
                        }
                        foreignIndexs.ForEach(i => sourceType.SetProperty(target, foreignIndexs[i], null));
                    };
                }
            }
        }
        /// <summary>
        /// 更新关系复合对象属性函数。
        /// </summary>
        internal Action<object, object> UpdateComplexMember { get; private set; }
        /// <summary>
        /// 创建外键函数。
        /// </summary>
        internal Action<object, object> CreateForeignKey { get; private set; }
        /// <summary>
        /// 删除外键函数。
        /// </summary>
        internal Action<object, object> DeleteForeignKey { get; private set; }
        /// <summary>
        /// 获取数据列特性。
        /// </summary>
        /// <typeparam name="T">特性类型。</typeparam>
        /// <returns>找到返回特性实例，否则返回 null。</returns>
        public T GetProperty<T>() where T : Attribute, IColumnAnnotation
        {
            if (_Attributes.TryGetValue(typeof(T), out IColumnAnnotation value))
            {
                return (T)value;
            }
            return default(T);
        }
        /// <summary>
        /// 设置数据列特性，如果设置值为空则表示删除该特性。
        /// </summary>
        /// <typeparam name="T">特性类型。</typeparam>
        /// <param name="value">设置值。</param>
        public void SetProperty<T>(T value) where T : Attribute, IColumnAnnotation
        {
            if (value == null)
            {
                _Attributes.Remove(typeof(T));
            }
            else
            {
                _Attributes.AddOrUpdate(typeof(T), value);
            }
        }
    }
}