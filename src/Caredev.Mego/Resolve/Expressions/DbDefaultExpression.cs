// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    /// <summary>
    /// 默认值表达式。
    /// </summary>
    public class DbDefaultExpression : DbExpression
    {
        /// <summary>
        /// 创建默认值表达式。
        /// </summary>
        /// <param name="type"></param>
        public DbDefaultExpression(Type type)
        {
            ClrType = type;
        }
        /// <summary>
        /// 当前默认值的CLR类型。
        /// </summary>
        public Type ClrType { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.Default;
    }
}