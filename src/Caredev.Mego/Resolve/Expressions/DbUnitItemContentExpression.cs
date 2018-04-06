// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 数据项内容表达式。
    /// </summary>
    public class DbUnitItemContentExpression : DbUnitItemTypeExpression
    {
        /// <summary>
        /// 创建数据项内容表达式。
        /// </summary>
        /// <param name="content">当前内容表达式。</param>
        public DbUnitItemContentExpression(DbUnitItemTypeExpression content)
            : base(content.ClrType)
        {
            Content = content;
        }
        /// <summary>
        /// 数据项内容表达式。
        /// </summary>
        public DbUnitItemTypeExpression Content { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.UnitItemContent;
    }
}