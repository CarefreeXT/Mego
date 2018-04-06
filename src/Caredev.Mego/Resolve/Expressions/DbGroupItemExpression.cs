// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 针对单元项表达的分组项表达式。
    /// </summary>
    public class DbGroupItemExpression : DbUnitItemTypeExpression, IDbUnitTypeExpression
    {
        /// <summary>
        /// 创建分组项表达式。
        /// </summary>
        /// <param name="unititem">单元项表达式。</param>
        public DbGroupItemExpression(DbUnitItemTypeExpression unititem)
            : base(unititem.ClrType)
        {
            Source = unititem;
            Item = new DbDataItemExpression(unititem.ClrType)
            {
                Unit = this
            };
        }
        /// <summary>
        /// 源表达式。
        /// </summary>
        public DbUnitItemTypeExpression Source { get; private set; }
        /// <summary>
        /// 单元项表达式。
        /// </summary>
        public DbUnitItemTypeExpression Item { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.GroupItem;
    }
}