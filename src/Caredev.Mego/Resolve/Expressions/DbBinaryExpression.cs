// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    /// <summary>
    /// 二元操作表达式。
    /// </summary>
    public class DbBinaryExpression : DbExpression
    {
        /// <summary>
        /// 创建二元操作表达式。
        /// </summary>
        /// <param name="kind">二元操作种类。</param>
        /// <param name="left">左端表达式。</param>
        /// <param name="right">右端表达式。</param>
        /// <param name="type">两端操作数据类型。</param>
        public DbBinaryExpression(EBinaryKind kind, DbExpression left, DbExpression right, Type type)
        {
            Kind = kind;
            Left = left;
            Right = right;
            ClrType = type;
        }
        /// <summary>
        /// 左端表达式。
        /// </summary>
        public DbExpression Left { get; private set; }
        /// <summary>
        /// 右端表达式。
        /// </summary>
        public DbExpression Right { get; private set; }
        /// <summary>
        /// 二元操作种类。
        /// </summary>
        public EBinaryKind Kind { get; private set; }
        /// <summary>
        /// 两端操作数据类型。
        /// </summary>
        public Type ClrType { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.Binary;
    }
}