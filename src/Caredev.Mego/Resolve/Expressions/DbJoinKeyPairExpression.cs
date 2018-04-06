// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 数据单元连接操作的键匹配表达式。
    /// </summary>
    public class DbJoinKeyPairExpression : DbExpression
    {
        /// <summary>
        /// 创建连接表达式。
        /// </summary>
        /// <param name="left">连接左端键的表达式。</param>
        /// <param name="right">连接右端键的表达式。</param>
        public DbJoinKeyPairExpression(DbExpression left, DbExpression right)
        {
            Left = left;
            Right = right;
        }
        /// <summary>
        /// 连接左端键的表达式。
        /// </summary>
        public DbExpression Left { get; }
        /// <summary>
        /// 连接右端键的表达式
        /// </summary>
        public DbExpression Right { get; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.JoinKeyPair;
    }
}