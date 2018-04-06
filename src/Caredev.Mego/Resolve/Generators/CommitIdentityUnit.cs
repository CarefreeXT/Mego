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
    /// 带有标识列的提交单元。
    /// </summary>
    public class CommitIdentityUnit : CommitUnitBase
    {
        /// <summary>
        /// 创建提交单元。
        /// </summary>
        /// <param name="table">表元数据。</param>
        /// <param name="identity">标识列。</param>
        public CommitIdentityUnit(TableMetadata table, CommitMember identity)
            : base(table)
        {
            Identity = identity;
        }
        /// <summary>
        /// 标识列。
        /// </summary>
        public CommitMember Identity { get; }
    }
}