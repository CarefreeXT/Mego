// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections.Generic;
    using System.Text;
    /// <summary>
    /// 提交单元对接基类。
    /// </summary>
    public class CommitUnitBase
    {
        /// <summary>
        /// 初始化提交单元。
        /// </summary>
        /// <param name="table">提交相应的表元数据。</param>
        public CommitUnitBase(TableMetadata table)
        {
            Table = table;
        }
        /// <summary>
        /// 关联的表对象。
        /// </summary>
        public TableMetadata Table { get; }
        /// <summary>
        /// 数据成员。
        /// </summary>
        public IEnumerable<CommitMember> Members => _Members;
        private List<CommitMember> _Members = new List<CommitMember>();
        /// <summary>
        /// 添加成员对象。
        /// </summary>
        /// <param name="member">要添加的成员。</param>
        public virtual void Add(CommitMember member)
        {
            _Members.Add(member);
        }
    }
}