// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Reflection;
    /// <summary>
    /// 集合检索函数表达式，例如First、Single等LINQ函数操作。
    /// </summary>
    public class DbRetrievalFunctionExpression : DbSourceFunctionExpression
    {
        /// <summary>
        /// 创建集合检索函数表达式。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <param name="argus"></param>
        public DbRetrievalFunctionExpression(IDbUnitTypeExpression source, MethodInfo func, params DbExpression[] argus)
            : base(source, func, argus)
        { }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.RetrievalFunction;
    }
}