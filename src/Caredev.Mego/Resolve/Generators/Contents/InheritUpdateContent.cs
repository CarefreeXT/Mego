// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.ValueGenerates;
    /// <summary>
    /// 更新继承数据的内容对象。
    /// </summary>
    public class InheritUpdateContent : InheritContentUnitBase
    {
        /// <summary>
        /// 创建内容对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">操作对象。</param>
        internal InheritUpdateContent(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
        }
        /// <inheritdoc/>
        public override ValueGenerateBase GetValueGenerator(ColumnMetadata column) => column.GeneratedForUpdate;
        /// <inheritdoc/>
        public override void Inititalze(DbExpression content = null)
        {
            base.Inititalze(content);
            Units = new CommitKeyUnit[Table.InheritSets.Length + 1];
            int index = 0;
            this.Tables.ForEach(metadata =>
            {
                var unit = new CommitKeyUnit(metadata);
                this.CreateCommitUnit(unit, metadata.Members, content);
                Units[index++] = unit;
            });
        }
    }
}