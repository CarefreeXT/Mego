// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Reflection;
    /// <summary>
    /// 数据项函数表达式。
    /// </summary>
    public class DbItemFunctionExpression : DbMapFunctionExpression
    {
        /// <summary>
        /// 创建数据项函数表达式。
        /// </summary>
        /// <param name="func">函数CLR描述对象。</param>
        /// <param name="kind">函数映射种类。</param>
        /// <param name="argus">函数参数。</param>
        public DbItemFunctionExpression(MethodInfo func, EMapFunctionKind kind, params DbExpression[] argus)
            : base(func, kind, argus)
        { }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.ItemFunction;
    }
}