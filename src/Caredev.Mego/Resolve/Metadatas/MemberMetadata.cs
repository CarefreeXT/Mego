// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using System.Reflection;
    /// <summary>
    /// CLR成员元数据。
    /// </summary>
    public class MemberMetadata : IPropertyMetadata
    {
        /// <summary>
        /// 创建CLR成员元数据。
        /// </summary>
        /// <param name="member">成员CLR描述对象。</param>
        /// <param name="kind">成员种类。</param>
        public MemberMetadata(PropertyInfo member, MemberKind kind)
        {
            Member = member;
            Kind = kind;
        }
        /// <summary>
        /// 成员种类。
        /// </summary>
        public MemberKind Kind { get; }
        /// <summary>
        /// 当前成员在类型元数据中的索引。
        /// </summary>
        public int Index { get; }
        /// <summary>
        /// 成员CLR描述对象。
        /// </summary>
        public PropertyInfo Member { get; }
    }
}