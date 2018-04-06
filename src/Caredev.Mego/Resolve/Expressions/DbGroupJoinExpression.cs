// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 分组并连接表达式。
    /// </summary>
    public class DbGroupJoinExpression : DbInnerJoinExpression
    {
        /// <summary>
        /// 创建分组并连接表达式。
        /// </summary>
        /// <param name="source">源表达式。</param>
        /// <param name="target">目标表达式。</param>
        /// <param name="left">左端键表达式。</param>
        /// <param name="right">右端键表达式。</param>
        /// <param name="newExp">当前连接后输出的新对象表达工。</param>
        public DbGroupJoinExpression(DbUnitTypeExpression source, DbUnitTypeExpression target, DbExpression left, DbExpression right, DbUnitItemTypeExpression newExp)
           : base(source, target, left, right, newExp)
        { }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.GroupJoin;
    }
}