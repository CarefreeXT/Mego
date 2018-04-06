// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    /// <summary>
    /// 一元运算表达式。
    /// </summary>
    public class DbUnaryExpression : DbExpression
    {
        /// <summary>
        /// 创建一个指定表达式的一元运算表达式。
        /// </summary>
        /// <param name="kind">一元运算种类。</param>
        /// <param name="expression">原始表达式。</param>
        /// <param name="type">输出的CLR类型。</param>
        public DbUnaryExpression(EUnaryKind kind, DbExpression expression, Type type)
        {
            Kind = kind;
            Expression = expression;
            Type = type;
        }
        /// <summary>
        /// 原始表达式。
        /// </summary>
        public DbExpression Expression { get; }
        /// <summary>
        /// 输出的CLR类型。
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// 一元运算种类。
        /// </summary>
        public EUnaryKind Kind { get; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.Unary;
    }
}