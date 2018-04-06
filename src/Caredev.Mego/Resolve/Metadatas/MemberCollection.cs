// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.Common;
    using System.Collections.Generic;
    using System.Reflection;
    /// <summary>
    /// 成员元数据集合。
    /// </summary>
    /// <typeparam name="TMember">成员元数据类型。</typeparam>
    public class MemberCollection<TMember> : IndexedDictionary<string, TMember>
        where TMember : IPropertyMetadata
    {
        /// <summary>
        /// 使用默认设置创建成员集合。
        /// </summary>
        public MemberCollection()
        {
        }
        /// <summary>
        /// 使用指定的成员列表创建成员集合。
        /// </summary>
        /// <param name="source">指定的成员列表。</param>
        public MemberCollection(IEnumerable<TMember> source)
            : base(source, item => item.Member.Name)
        {
        }
        /// <summary>
        /// 添加一个新的成员对象。
        /// </summary>
        /// <param name="member">要添加的成员。</param>
        public void Add(TMember member)
        {
            base.Add(member.Member.Name, member);
        }
        /// <summary>
        /// 使用指定属性检索在当前集合中的索引值。
        /// </summary>
        /// <param name="property">属性对象。</param>
        /// <returns>查找后的索引值，如果没找到则返回 -1。</returns>
        public int IndexOf(PropertyInfo property) => IndexOf(property.Name);
        /// <summary>
        /// 判断指定属性是否在当前集合中存在
        /// </summary>
        /// <param name="property">属性对象。</param>
        /// <returns>如果存在返回 true，否则返回 false。</returns>
        public bool Contains(PropertyInfo property) => Contains(property.Name);
        /// <summary>
        /// 当前集合索引器，使用指定属性查找相应的成员元数据。
        /// </summary>
        /// <param name="property">指定的属性。</param>
        /// <returns>返回查找结果。</returns>
        public TMember this[PropertyInfo property]
        {
            get { return this[property.Name]; }
        }
    }
}