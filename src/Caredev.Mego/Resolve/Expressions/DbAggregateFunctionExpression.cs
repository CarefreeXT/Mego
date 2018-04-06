// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Reflection;
    /// <summary>
    /// 汇总函数表达式。
    /// </summary>
    public class DbAggregateFunctionExpression : DbSourceFunctionExpression
    {
        /// <summary>
        /// 创建汇总函数表达式。
        /// </summary>
        /// <param name="source">源表达式。</param>
        /// <param name="function">函数CLR描述对象。</param>
        /// <param name="arguments">函数参数。</param>
        public DbAggregateFunctionExpression(IDbUnitTypeExpression source, MemberInfo function, params DbExpression[] arguments)
            : base(source, function, arguments)
        { }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.AggregateFunction;
    }
}