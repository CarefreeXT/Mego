// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    /// <summary>
    /// 数据项表达式，由<see cref="DbDataSetExpression"/>成员元素表达式。
    /// </summary>
    public class DbDataItemExpression : DbUnitItemTypeExpression
    {
        /// <summary>
        /// 创建数据项表达式。
        /// </summary>
        /// <param name="type"></param>
        public DbDataItemExpression(Type type) : base(type)
        {
        }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.DataItem;
    }
}