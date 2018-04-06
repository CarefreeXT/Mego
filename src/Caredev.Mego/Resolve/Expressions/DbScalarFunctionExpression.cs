// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Reflection;
    /// <summary>
    /// 标量函数调用表达式。
    /// </summary>
    public class DbScalarFunctionExpression : DbMapFunctionExpression
    {
        /// <summary>
        /// 创建标量函数调用表达式。
        /// </summary>
        /// <param name="func">CLR函数对象。</param>
        /// <param name="kind">函数种类。</param>
        /// <param name="argus">函数参数。</param>
        public DbScalarFunctionExpression(MemberInfo func, EMapFunctionKind kind, params DbExpression[] argus)
            : base(func, kind, argus)
        { }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.ScalarFunction;
    }
}