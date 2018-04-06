// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.ValueGenerates;
    using System.Reflection;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using Caredev.Mego.Common;
    /// <summary>
    /// 数据列元数据。
    /// </summary>
    public class ColumnMetadata : IPropertyMetadata
    {
        /// <summary>
        /// 创建数据列元数据。
        /// </summary>
        /// <param name="table">表元数据对象。</param>
        /// <param name="member">属性信息对象。</param>
        /// <param name="name">强制指定列名。</param>
        public ColumnMetadata(TableMetadata table, PropertyInfo member, string name = "") 
        {
            Table = table;
            Member = member;

            _Attributes = member.GetCustomAttributes()
                .OfType<IColumnAnnotation>()
                .ToDictionary(a => a.GetType(), a => a);

            _Column = member.GetCustomAttribute<ColumnAttribute>();
            if (_Column == null)
            {
                _Column = new ColumnAttribute(member.Name);
                _Attributes.Add(typeof(ColumnAttribute), _Column);
            }
            if (!string.IsNullOrEmpty(name) && _Column.Name != name)
            {
                _Column.Name = name;
            }
            foreach (var attr in member.GetCustomAttributes<GeneratedValueBaseAttribute>())
            {
                GeneratedForInsert = CreateGenerate(attr, EGeneratedPurpose.Insert, GeneratedForInsert);
                GeneratedForUpdate = CreateGenerate(attr, EGeneratedPurpose.Update, GeneratedForUpdate);
            }
        }
        /// <summary>
        /// 数据库列名称。
        /// </summary>
        public string Name => _Column.Name;
        /// <summary>
        /// 数据库列顺序。
        /// </summary>
        public int Order => _Column.Order;
        /// <summary>
        /// 数据列显示声明的类型。
        /// </summary>
        public string TypeName => _Column.TypeName;
        /// <summary>
        /// 添加时值生成。
        /// </summary>
        public ValueGenerateBase GeneratedForInsert { get; }
        /// <summary>
        /// 更新时值生成。
        /// </summary>
        public ValueGenerateBase GeneratedForUpdate { get; }
        /// <summary>
        /// 所在对象的属性对象。
        /// </summary>
        public PropertyInfo Member { get; }
        /// <summary>
        /// 所在的表元数据对象。
        /// </summary>
        public TableMetadata Table { get; }
        /// <summary>
        /// 当前列是否为主键列。
        /// </summary>
        public bool IsKey { get; internal set; }
        /// <summary>
        /// 当前数据列是否参与并发检查。
        /// </summary>
        public bool IsConcurrencyCheck => _Attributes.Keys.Any(a => typeof(IConcurrencyCheck).IsAssignableFrom(a));
        /// <summary>
        /// 获取数据列特性。
        /// </summary>
        /// <typeparam name="T">特性类型。</typeparam>
        /// <returns>返回相同类型的特性，否则返回 null。</returns>
        public T GetProperty<T>() where T : Attribute, IColumnAnnotation
        {
            if (_Attributes.TryGetValue(typeof(T), out IColumnAnnotation value))
            {
                return (T)value;
            }
            return default(T);
        }
        /// <summary>
        /// 设置数据列特性，如果设置值为空则表示删除该特性，禁止删除类
        /// 型为<see cref="ColumnAttribute"/>的特性。
        /// </summary>
        /// <typeparam name="T">特性类型。</typeparam>
        /// <param name="value">设置的值。</param>
        public void SetProperty<T>(T value) where T : Attribute, IColumnAnnotation
        {
            var type = typeof(T);
            try
            {
                if (type == typeof(ColumnAttribute))
                {
                    Utility.NotNull<T>(value, nameof(value));
                    _Column = value as ColumnAttribute;
                }
                else if (value == null)
                {
                    _Attributes.Remove(type);
                    return;
                }
                _Attributes.AddOrUpdate(type, value);
            }
            finally
            {
                if (typeof(IConcurrencyCheck).IsAssignableFrom(type))
                {
                    Table.RefreshConcurrencys();
                }
            }
        }
        private ValueGenerateBase CreateGenerate(GeneratedValueBaseAttribute attr, EGeneratedPurpose purpose, ValueGenerateBase source)
        {
            if (source == null && (attr.GeneratedPurpose & purpose) == purpose)
            {
                switch (attr.GeneratedOption)
                {
                    case EGeneratedOption.Ignore: return IgnoreValueGenerate;
                    case EGeneratedOption.Identity: return new ValueGenerateIdentity();
                    case EGeneratedOption.Database: return new ValueGenerateDatabase();
                    case EGeneratedOption.Memory:
                        var memoryAttr = (GeneratedMemoryValueAttribute)attr;
                        return new ValueGenerateMemory(memoryAttr.GeneratorType);
                    case EGeneratedOption.Expression:
                        var expAttr = (GeneratedExpressionAttribute)attr;
                        return new ValueGenerateExpression(expAttr.Expression);
                }
            }
            return source;
        }
        private static readonly ValueGenerateBase IgnoreValueGenerate = new ValueGenerateIgnore();
        private readonly Dictionary<Type, IColumnAnnotation> _Attributes;
        private ColumnAttribute _Column;
    }
}