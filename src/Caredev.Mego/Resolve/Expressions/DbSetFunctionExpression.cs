// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    using System.Reflection;
    /// <summary>
    /// 数据集函数表达式。
    /// </summary>
    public class DbSetFunctionExpression : DbMapFunctionExpression
    {
        /// <summary>
        /// 创建数据集函数表达式。
        /// </summary>
        /// <param name="func">函数的CLR对象。</param>
        /// <param name="kind">函数种类。</param>
        /// <param name="argus">函数参数。</param>
        public DbSetFunctionExpression(MethodInfo func, EMapFunctionKind kind, params DbExpression[] argus)
            : base(func, kind, argus)
        {
            ReturnClrType = func.ReturnType.GetGenericArguments()[0];
        }
        /// <summary>
        /// 函数返回值的CLR类型。
        /// </summary>
        public Type ReturnClrType { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.SetFunction;
    }
}