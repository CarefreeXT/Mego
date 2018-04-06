// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 对象内容表达式，当查询SELECT子句时只返回一个对象属性时，该对象表示需要内容的成员访问引用。
    /// </summary>
    public class DbUnitObjectContentExpression : DbUnitItemTypeExpression
    {
        /// <summary>
        /// 创建对象内容表达式。
        /// </summary>
        /// <param name="member">对象成员引用表达式。</param>
        public DbUnitObjectContentExpression(DbObjectMemberExpression member)
            : base(((System.Reflection.PropertyInfo)member.Member).PropertyType)
        {
            Content = member;
        }
        /// <summary>
        /// 内容表达式。
        /// </summary>
        public DbObjectMemberExpression Content { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.UnitObjectContent;
    }
}