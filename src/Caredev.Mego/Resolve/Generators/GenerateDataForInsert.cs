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
    using System.Linq;
    /// <summary>
    /// 用于插入数据的生成数据对象。
    /// </summary>
    public class GenerateDataForInsert : GenerateDataForUnits
    {
        /// <summary>
        /// 创建数据对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">当前操作对象。</param>
        internal GenerateDataForInsert(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
        }
        /// <inheritdoc/>
        protected override ValueGenerateBase GetValueGenerator(ColumnMetadata column) => column.GeneratedForInsert;
        /// <inheritdoc/>
        public override void Inititalze(DbExpression content = null)
        {
            base.Inititalze(content);
            var table = Table;
            var context = GenerateContext;
            if (table.InheritSets.Any())
            {
                int index = 0;
                SubUnits = new CommitKeyUnit[table.InheritSets.Length];
                table.InheritSets.Concat(new TableMetadata[] { table }).ForEach(
                    first => MainUnit = CreateMainUnit(first, content),
                    other =>
                    {
                        var unit = new CommitKeyUnit(other);
                        foreach (var column in other.Keys)
                        {
                            unit.Add(CreateCommitUnitMember(column, null));
                        }
                        CreateCommitUnit(unit, other.Members, content);
                        SubUnits[index++] = unit;
                    });
            }
            else
            {
                MainUnit = CreateMainUnit(table, content);
            }
        }
        /// <summary>
        /// 是否存在表达式主键。
        /// </summary>
        public bool HasExpressionKey { get; private set; }
        private CommitUnitBase CreateMainUnit(TableMetadata metadata, DbExpression content)
        {
            var identity = metadata.Keys.FirstOrDefault(a
                => a.GeneratedForInsert != null && a.GeneratedForInsert.GeneratedOption == EGeneratedOption.Identity);
            if (identity != null)
            {
                var identityMember = new CommitMember(identity, identity.GeneratedForInsert, ECommitValueType.Database);
                var commitUnit = new CommitIdentityUnit(metadata, identityMember);
                ReisterReturnMember(identityMember);
                CreateCommitUnit(commitUnit, metadata.Members.Where(a => a != identity), content);
                HasExpressionKey = commitUnit.Members.Any(a => a.Metadata.IsKey && a.ValueType == ECommitValueType.Expression);
                return commitUnit;
            }
            else
            {
                var commitUnit = new CommitKeyUnit(metadata);
                CreateCommitUnit(commitUnit, metadata.Members, content);
                HasExpressionKey = commitUnit.Keys.Any(a => a.ValueType == ECommitValueType.Expression);
                return commitUnit;
            }
        }
    }
}