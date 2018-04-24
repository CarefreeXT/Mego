// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.ValueGenerates;
    /// <summary>
    /// 插入数据内容对象。
    /// </summary>
    public class InsertContent : CommitContentUnitBase
    {
        /// <summary>
        /// 创建内容对象。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="operate"></param>
        internal InsertContent(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
        }

        public bool HasExpressionKey => _HasExpressionKey;
        private bool _HasExpressionKey;

        public override ValueGenerateBase GetValueGenerator(ColumnMetadata column) => column.GeneratedForInsert;

        public override void Inititalze(DbExpression content = null)
        {
            base.Inititalze(content);
            Unit = this.CreateUnitForIdentity(Table, content, out _HasExpressionKey);
        }
    }
}