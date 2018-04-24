// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.Outputs;
    using Caredev.Mego.Resolve.ValueGenerates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// 继承的提交单元基类。
    /// </summary>
    public abstract class InheritCommitUnitBase : InheritCommitBase, ICommitContentUnit
    {
        /// <summary>
        /// 创建提交单元。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">操作对象。</param>
        internal InheritCommitUnitBase(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
        }
        /// <summary>
        /// 提交的数据单元集合。
        /// </summary>
        public CommitUnitBase[] Units { get; internal protected set; }

        public IEnumerable<ColumnMetadata> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = Tables.SelectMany(a => a.Members).ToArray();
                }
                return _Columns;
            }
        }
        private IEnumerable<ColumnMetadata> _Columns;

        public abstract ValueGenerateBase GetValueGenerator(ColumnMetadata column);
        public IEnumerable<CommitMember> ReturnMembers => _ReturnMembers;
        private List<CommitMember> _ReturnMembers = new List<CommitMember>();

        public CommitMember ReisterReturnMember(CommitMember member)
        {
            if (Items.HasResult)
            {
                _ReturnMembers.Add(member);
            }
            return member;
        }
    }
}