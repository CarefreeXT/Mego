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
    /// 具有一个或多个主键的提交单元。
    /// </summary>
    public class CommitKeyUnit : CommitUnitBase
    {
        /// <summary>
        /// 创建提交单元。
        /// </summary>
        /// <param name="table">表元数据。</param>
        public CommitKeyUnit(TableMetadata table)
            : base(table)
        {
        }
        /// <summary>
        /// 主键成员集合。
        /// </summary>
        public IEnumerable<CommitMember> Keys => _Keys;
        private List<CommitMember> _Keys = new List<CommitMember>();
        /// <inheritdoc/>
        public override void Add(CommitMember member)
        {
            if (member.Metadata.IsKey)
            {
                _Keys.Add(member);
            }
            else
            {
                base.Add(member);
            }
        }
    }
}