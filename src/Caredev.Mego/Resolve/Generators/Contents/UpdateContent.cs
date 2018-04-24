// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using Caredev.Mego.Resolve.Expressions;
using Caredev.Mego.Resolve.Metadatas;
using Caredev.Mego.Resolve.Operates;
using Caredev.Mego.Resolve.ValueGenerates;

namespace Caredev.Mego.Resolve.Generators.Contents
{
    /// <summary>
    /// 更新数据内容对象。
    /// </summary>
    public class UpdateContent : CommitContentUnitBase
    {
        /// <summary>
        /// 创建内容对象。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="operate"></param>
        internal UpdateContent(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
        }

        public override ValueGenerateBase GetValueGenerator(ColumnMetadata column) => column.GeneratedForUpdate;

        public override void Inititalze(DbExpression content = null)
        {
            base.Inititalze(content);
            Unit = this.CreateCommitUnit(new CommitKeyUnit(Table), Table.Members, content);
        }
    }
}