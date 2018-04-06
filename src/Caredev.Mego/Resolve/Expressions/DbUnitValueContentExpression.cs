// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 单元值内容表达式，当查询SELECT子句时只返回一个基础数据类型的属性时
    /// ，该对象表示需要内容的成员访问引用。
    /// </summary>
    public class DbUnitValueContentExpression : DbUnitItemTypeExpression
    {
        /// <summary>
        /// 使用指定的引用成员，创建一个单元值内容表达式。
        /// </summary>
        /// <param name="member">引用成员表达式。</param>
        public DbUnitValueContentExpression(DbMemberExpression member)
            : base(((System.Reflection.PropertyInfo)member.Member).PropertyType)
        {
            Content = member;
        }
        /// <summary>
        /// 当前值表达式。
        /// </summary>
        public DbMemberExpression Content { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.UnitValueContent;
    }
}