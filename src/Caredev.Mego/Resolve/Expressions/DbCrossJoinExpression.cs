// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Linq;
    /// <summary>
    /// 笛卡尔积连接表达式。
    /// </summary>
    public class DbCrossJoinExpression : DbSetOperationExpression
    {
        /// <summary>
        /// 创建笛卡尔积连接表达式。
        /// </summary>
        /// <param name="source">源表达式。</param>
        /// <param name="target">目标表达式。</param>
        /// <param name="newExp">连接后输出的项表达式。</param>
        public DbCrossJoinExpression(DbUnitTypeExpression source, DbUnitTypeExpression target, DbUnitItemTypeExpression newExp)
            : base(typeof(IQueryable<>).MakeGenericType(newExp.ClrType), source, newExp)
        {
            Target = target;
        }
        /// <summary>
        /// 目标单元表达式。
        /// </summary>
        public DbUnitTypeExpression Target { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.CrossJoin;
    }
}