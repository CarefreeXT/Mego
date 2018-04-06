// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    /// <summary>
    /// 常量表达式。
    /// </summary>
    public class DbConstantExpression : DbExpression
    {
        /// <summary>
        /// 创建常量表达式。
        /// </summary>
        /// <param name="type">常量的CLR类型。</param>
        /// <param name="value">常量值。</param>
        public DbConstantExpression(Type type, object value)
        {
            ClrType = type;
            Value = value;
        }
        /// <summary>
        /// 常量的CLR类型。
        /// </summary>
        public Type ClrType { get; private set; }
        /// <summary>
        /// 常量值。
        /// </summary>
        public object Value { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.Constant;
    }
}