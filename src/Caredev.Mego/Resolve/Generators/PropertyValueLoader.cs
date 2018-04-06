// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caredev.Mego.Resolve.Metadatas;
    using System.Reflection;
    using Caredev.Mego.Resolve.Operates;
    /// <summary>
    /// 属性值加载器，用于加载对象指定属性集合的值。
    /// </summary>
    internal class PropertyValueLoader : IPropertyValueLoader
    {
        /// <summary>
        /// 加载<see cref="TypeMetadata"/>中指定属性的索引集合。
        /// </summary>
        private readonly int[] Indexs;
        /// <summary>
        /// 根据当前属性索引加载的属性值。
        /// </summary>
        private readonly object[] Values;
        /// <summary>
        /// 加载<see cref="TypeMetadata"/>中指定属性的名称集合。
        /// </summary>
        private readonly string[] Names;
        /// <summary>
        /// 创建属性值加载器。
        /// </summary>
        /// <param name="metadata">类型元数据。</param>
        /// <param name="members">需要加载的成员列表。</param>
        public PropertyValueLoader(TypeMetadata metadata, IEnumerable<IPropertyMetadata> members)
        {
            IPropertyMetadata[] memberlist = null;
            if (members.Any())
            {
                memberlist = members.ToArray();
            }
            else
            {
                memberlist = metadata.PrimaryMembers.ToArray();
            }
            Names = new string[memberlist.Length];
            Indexs = new int[memberlist.Length];
            var primarys = metadata.PrimaryMembers.Select(a => a.Member.Name).ToList();
            for (int i = 0; i < memberlist.Length; i++)
            {
                Names[i] = memberlist[i].Member.Name;
                Indexs[i] = primarys.IndexOf(Names[i]);
            }
            Values = new object[Indexs.Length];
            Metadata = metadata;
        }
        /// <summary>
        /// 创建属性值加载器。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="table">表元数据。</param>
        /// <param name="members">指定加载的成员列表。</param>
        public PropertyValueLoader(GenerateContext context, TableMetadata table, params IPropertyMetadata[] members)
            : this(context.Metadata.Type(table.ClrType), members)
        {
        }
        /// <summary>
        /// 返回指定成员在值集合中的索引值。
        /// </summary>
        /// <param name="member">指定成员。</param>
        /// <returns>索引值。</returns>
        public int IndexOf(MemberInfo member)
        {
            return Array.IndexOf(Names, member.Name);
        }
        /// <summary>
        /// 获取指定索引的属性值。
        /// </summary>
        /// <param name="index">指定索引值。</param>
        /// <returns>属性值。</returns>
        public object this[int index]
        {
            get { return Values[index]; }
        }
        /// <summary>
        /// 加载指定对象获取属性值。
        /// </summary>
        /// <param name="value">加载的对象。</param>
        /// <returns>当前加载器。</returns>
        public IPropertyValueLoader Load(object value)
        {
            Metadata.GetProperty(value, Indexs, Values);
            return this;
        }
        /// <summary>
        /// 类型元数据。
        /// </summary>
        public TypeMetadata Metadata { get; }
        /// <summary>
        /// 成员长度。
        /// </summary>
        public int Length => Names.Length;
    }
}