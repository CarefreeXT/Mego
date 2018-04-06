// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 更新数据时获取数据库原始值引用对象表达式。
    /// </summary>
    public class DbOriginalObjectExpression : DbExpression
    {
        /// <summary>
        /// 创建原始值对象表达式。
        /// </summary>
        /// <param name="item"></param>
        public DbOriginalObjectExpression(DbExpression item)
        {
            Item = item;
        }
        /// <summary>
        /// 数据项对象。
        /// </summary>
        public DbExpression Item { get; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.OriginalObject;
    }
}