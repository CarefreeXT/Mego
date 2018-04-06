// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 排序表达式。
    /// </summary>
    public class DbOrderExpression : DbExpression
    {
        /// <summary>
        /// 创建排序表达式。
        /// </summary>
        /// <param name="member">排序成员表达式。</param>
        /// <param name="kind">排序种类。</param>
        public DbOrderExpression(DbExpression member, EOrderKind kind)
        {
            Member = member;
            Kind = kind;
        }
        /// <summary>
        /// 排序成员表达式。
        /// </summary>
        public DbExpression Member { get; private set; }
        /// <summary>
        /// 排序种类。
        /// </summary>
        public EOrderKind Kind { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.Order;
    }
}