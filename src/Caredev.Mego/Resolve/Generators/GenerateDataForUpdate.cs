// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.ValueGenerates;
    /// <summary>
    /// 用于更新数据的生成数据对象。
    /// </summary>
    public class GenerateDataForUpdate : GenerateDataForUnits
    {
        /// <summary>
        /// 创建数据对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">当前操作对象。</param>
        internal GenerateDataForUpdate(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
        }
        /// <inheritdoc/>
        protected override ValueGenerateBase GetValueGenerator(ColumnMetadata column) => column.GeneratedForUpdate;
        /// <inheritdoc/>
        public override void Inititalze(DbExpression content = null)
        {
            base.Inititalze(content);
            if (Table.InheritSets.Length == 0)
            {
                var unit = new CommitKeyUnit(Table);
                MainUnit = CreateCommitUnit(unit, Table.Members, content);
            }
            else
            {
                SubUnits = new CommitKeyUnit[Table.InheritSets.Length];
                int index = 0;
                GetTables().ForEach(
                    metadata =>
                    {
                        var unit = new CommitKeyUnit(metadata);
                        MainUnit = CreateCommitUnit(unit, metadata.Members, content);
                    },
                    metadata =>
                    {
                        var unit = new CommitKeyUnit(metadata);
                        CreateCommitUnit(unit, metadata.Members, content);
                        SubUnits[index++] = unit;
                    });
            }
        }
    }
}