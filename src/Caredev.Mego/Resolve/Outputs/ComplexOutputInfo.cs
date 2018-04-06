// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using Caredev.Mego.Exceptions;
    using Res = Properties.Resources;
    /// <summary>
    /// 复杂对象输出信息体。
    /// </summary>
    public abstract class ComplexOutputInfo : OutputInfoBase
    {
        private readonly object[] EmptyComplexObjects;
        private bool HasCollectionProperty = false;
        private bool IsInitialized = false;
        /// <summary>
        /// 根据<see cref="DbDataReader"/>的字段名创建复杂对象输出信息体。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <param name="metadata">类型元数据。</param>
        public ComplexOutputInfo(DbDataReader reader, TypeMetadata metadata)
            : this(metadata, GenerateFields(reader, metadata))
        {
        }
        /// <summary>
        /// 创建复杂对象输出信息体。
        /// </summary>
        /// <param name="metadata">对象类型元数据。</param>
        /// <param name="fields">一般属性的字段索引列表。</param>
        public ComplexOutputInfo(TypeMetadata metadata, int[] fields)
        {
            Metadata = metadata;
            ItemFields = fields;
            _Children = new List<OutputInfoBase>(5);
            EmptyComplexObjects = new object[Metadata.ComplexMembers.Count];
        }
        /// <summary>
        /// 当前输出对象在父级属性的索引值。
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 当前输出对象输出的普通属性索引列表。
        /// </summary>
        public int[] ItemFields { get; }
        /// <summary>
        /// 当前输出对象的键值列索引列表，该属性只能在匿名对象时才能设置，并且只能设置一次。
        /// </summary>
        public int[] ItemKeyFields
        {
            get
            {
                if (_ItemKeyFields == null && Metadata.KeyIndexs != null)
                {
                    var fields = ItemFields;
                    _ItemKeyFields = Metadata.KeyIndexs.Select(a => fields[a]).ToArray();
                }
                return _ItemKeyFields;
            }
            set
            {
                if (_ItemKeyFields == null && Metadata.KeyIndexs == null)
                    _ItemKeyFields = value;
            }
        }
        private int[] _ItemKeyFields;
        /// <summary>
        /// 当前输出对象的类型元数据。
        /// </summary>
        public TypeMetadata Metadata { get; }
        /// <summary>
        /// 当前对象的复杂属性集合。
        /// </summary>
        public IEnumerable<OutputInfoBase> Children => _Children;
        private readonly IList<OutputInfoBase> _Children;
        /// <summary>
        /// 当前输出体的父成员。
        /// </summary>
        public ComplexOutputInfo Parent { get; private set; }
        /// <summary>
        /// 创建数据项。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <param name="parent">如果是导航属性则为父级对象</param>
        /// <returns>创建结果。</returns>
        protected object CreateItem(DbDataReader reader, OutputContentItem parent = null)
        {
            if (_Children.Count == 0 && !HasCollectionProperty)
                return Metadata.CreateInstance(reader, ItemFields, EmptyComplexObjects);
            else
            {
                var complexs = new object[Metadata.ComplexMembers.Count];
                foreach (var item in Children.OfType<ComplexOutputInfo>())
                {
                    if (item.Type == EOutputType.Object)
                    {
                        complexs[item.Index] = item.CreateItem(reader, parent);
                    }
                    else
                    {
                        var propertyMember = item.Parent.Metadata.ComplexMembers[item.Index];
                        var propertyType = ((PropertyInfo)propertyMember.Member).PropertyType;
                        if (propertyType.IsInterface)
                        {
                            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IGrouping<,>))
                            {
                                var collectionType = typeof(GroupCollectionImpl<,>).MakeGenericType(propertyType.GetGenericArguments());
                                complexs[item.Index] = Activator.CreateInstance(collectionType);
                            }
                            else
                            {
                                var collectionType = typeof(HashSet<>).MakeGenericType(item.Metadata.ClrType);
                                complexs[item.Index] = Activator.CreateInstance(collectionType);
                            }
                        }
                        else if (propertyType.IsClass)
                        {
                            complexs[item.Index] = Activator.CreateInstance(propertyType);
                        }
                        else
                        {

                            throw new OutputException(this, string.Format(Res.NotSupportedInstanceType, propertyType));
                        }
                        parent.Collections.Add((CollectionOutputInfo)item, new OutputContentCollection((CollectionOutputInfo)item, complexs[item.Index]));
                    }
                }
                return Metadata.CreateInstance(reader, ItemFields, complexs);
            }
        }
        /// <summary>
        /// 初始化当前输出信息对象。
        /// </summary>
        /// <returns>是否初始化成功。</returns>
        protected bool Initialize()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;

                foreach (var children in Children)
                {
                    switch (children.Type)
                    {
                        case EOutputType.Collection:
                            HasCollectionProperty = true;
                            break;
                        case EOutputType.Object:
                            Initialize((ObjectOutputInfo)children);
                            break;
                    }
                }
            }
            return HasCollectionProperty;
        }
        /// <summary>
        /// 对于有导航成员对象添加输出信息。
        /// </summary>
        /// <param name="output"></param>
        public void AddChildren(OutputInfoBase output)
        {
            var complex = output as ComplexOutputInfo;
            if (complex != null)
            {
                complex.Parent = this;
            }
            _Children.Add(output);
        }
        //生成输出属性索引列表
        private static int[] GenerateFields(DbDataReader reader, TypeMetadata metadata)
        {
            var members = metadata.PrimaryMembers;
            var fields = new int[members.Count];
            for (int i = 0; i < fields.Length; i++)
                fields[i] = -1;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var field = members[name];
                //var fieldtype = reader.GetFieldType(i);
                if (field != null)// && field.Member.PropertyType.IsAssignableFrom(fieldtype))
                    fields[members.IndexOf(field.Member)] = i;
            }

            return fields;
        }
        //初始化复杂输出信息（公共）
        private void Initialize(ComplexOutputInfo parent)
        {
            foreach (var children in parent.Children)
            {
                switch (children.Type)
                {
                    case EOutputType.Collection:
                        HasCollectionProperty = true;
                        //SubItemCollection.Add((CollectionOutputInfo)children);
                        break;
                    case EOutputType.Object:
                        Initialize((ObjectOutputInfo)children);
                        break;
                }
            }
        }
    }
}