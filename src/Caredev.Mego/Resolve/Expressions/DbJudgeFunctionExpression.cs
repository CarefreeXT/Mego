// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Reflection;
    /// <summary>
    /// 判断函数表达式，例如Any、All等LINQ函数。
    /// </summary>
    public class DbJudgeFunctionExpression : DbSourceFunctionExpression
    {
        /// <summary>
        /// 创建判断函数表达式。
        /// </summary>
        /// <param name="source">源表达式。</param>
        /// <param name="func">函数CLR描述对象。</param>
        /// <param name="argus">函数参数。</param>
        public DbJudgeFunctionExpression(IDbUnitTypeExpression source, MethodInfo func, params DbExpression[] argus)
            : base(source, func, argus)
        { }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.JudgeFunction;
    }
}