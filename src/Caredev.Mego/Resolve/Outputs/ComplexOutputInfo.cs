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
        private bool IsInitialized = false;
        /// <summary>
        /// 表示当前输出信息层级中是否存在集合属性。
        /// </summary>
        protected bool HasCollectionProperty = false;
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
        /// 初始化当前输出信息对象。
        /// </summary>
        /// <returns>是否初始化成功。</returns>
        protected bool Initialize()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                Initialize(this);
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
        /// <summary>
        /// 判断当前读取器中是否存在需要输出的数据
        /// </summary>
        /// <param name="reader">数据读取器对象。</param>
        /// <returns>如果为空返回True，否则返回False。</returns>
        protected bool IsEmpty(DbDataReader reader) => reader.IsDBNull(ItemKeyFields[0]);
        //根据成员元数据创建集合实例对象
        private object CreateCollectionInstance(CollectionOutputInfo item)
        {
            var propertyMember = item.Parent.Metadata.ComplexMembers[item.Index];
            var propertyType = propertyMember.Member.PropertyType;
            if (propertyType.IsInterface)
            {
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IGrouping<,>))
                {
                    var collectionType = typeof(GroupCollectionImpl<,>).MakeGenericType(propertyType.GetGenericArguments());
                    return Activator.CreateInstance(collectionType);
                }
                else
                {
                    var collectionType = typeof(HashSet<>).MakeGenericType(item.Metadata.ClrType);
                    return Activator.CreateInstance(collectionType);
                }
            }
            else if (propertyType.IsClass)
            {
                return Activator.CreateInstance(propertyType);
            }
            else
            {
                throw new OutputException(this, string.Format(Res.NotSupportedInstanceType, propertyType));
            }
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
                        break;
                    case EOutputType.Object:
                        if (((ComplexOutputInfo)children).Initialize())
                        {
                            HasCollectionProperty = true;
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 从数据读取器中加载数据到输出的内容对象中。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <param name="content">目标内容。</param>
        protected virtual void LoadDataToContent(DbDataReader reader, IOutputContent content)
        {
            var item = (OutputContentObject)content;
            if (item.Content != null)
            {
                foreach (var kv in item.Members)
                {
                    kv.Key.LoadDataToContent(reader, kv.Value);
                }
            }
        }
        /// <summary>
        /// 创建带有层级结构的内容项。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <returns>创建结果。</returns>
        protected OutputContentObject CreateContentItem(DbDataReader reader)
        {
            var content = new OutputContentObject();
            if (!IsEmpty(reader))
            {
                var complexs = new object[Metadata.ComplexMembers.Count];
                foreach (var item in Children.OfType<ComplexOutputInfo>())
                {
                    if (item.Type == EOutputType.Object)
                    {
                        var objectInfo = (ObjectOutputInfo)item;
                        if (item.HasCollectionProperty)
                        {
                            if (!item.IsEmpty(reader))
                            {//如果数据为空，则不创建成员对象
                                var subcontent = item.CreateContentItem(reader);
                                content.Members.Add(objectInfo, subcontent);
                                complexs[item.Index] = subcontent.Content;
                            }
                        }
                        else
                        {
                            complexs[item.Index] = objectInfo.CreateObjectItem(reader);
                        }
                    }
                    else
                    {
                        var collectionItem = (CollectionOutputInfo)item;
                        var collectionInstance = CreateCollectionInstance(collectionItem);
                        complexs[item.Index] = collectionInstance;
                        content.Members.Add(collectionItem, new OutputContentCollection(collectionItem, collectionInstance));
                    }
                }
                content.Content = Metadata.CreateInstance(reader, ItemFields, complexs);
            }
            return content;
        }
        /// <summary>
        /// 直接创建对象，对于没有集合属性的对象可以直接创建出结果。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <returns>创建结果。</returns>
        protected object CreateObjectItem(DbDataReader reader)
        {
            if (_Children.Count == 0)
            {
                return Metadata.CreateInstance(reader, ItemFields, EmptyComplexObjects);
            }
            else
            {
                var complexs = new object[Metadata.ComplexMembers.Count];
                foreach (var item in Children.OfType<ComplexOutputInfo>())
                {
                    if (item.Type == EOutputType.Object)
                    {
                        complexs[item.Index] = item.CreateObjectItem(reader);
                    }
                }
                return Metadata.CreateInstance(reader, ItemFields, complexs);
            }
        }
    }
}