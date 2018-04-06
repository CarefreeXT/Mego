// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Reflection;
    /// <summary>
    /// 成员访问表达式。
    /// </summary>
    public class DbMemberExpression : DbExpression, IDbMemberExpression
    {
        /// <summary>
        /// 创建成员访问表达式。
        /// </summary>
        /// <param name="member">成员CLR描述对象。</param>
        /// <param name="source">源表达式。</param>
        public DbMemberExpression(MemberInfo member, DbExpression source)
        {
            Member = member;
            Expression = source;
        }
        /// <summary>
        /// 成员CLR描述对象。
        /// </summary>
        public MemberInfo Member { get; private set; }
        /// <summary>
        /// 源表达式。
        /// </summary>
        public DbExpression Expression { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.MemberAccess;
    }
}