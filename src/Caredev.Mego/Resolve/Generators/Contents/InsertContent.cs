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
    /// 插入数据的内容对象。
    /// </summary>
    public class InsertContent : ContentUnitBase
    {
        /// <summary>
        /// 创建内容对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">操作对象。</param>
        internal InsertContent(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
        }
        /// <summary>
        /// 是否存在由表达式生成的主键成员。
        /// </summary>
        public bool HasExpressionKey => _HasExpressionKey;
        private bool _HasExpressionKey;
        /// <inheritdoc/>
        public override ValueGenerateBase GetValueGenerator(ColumnMetadata column) => column.GeneratedForInsert;
        /// <inheritdoc/>
        public override void Inititalze(DbExpression content = null)
        {
            base.Inititalze(content);
            Unit = this.CreateUnitForIdentity(Table, content, out _HasExpressionKey);
        }
    }
}