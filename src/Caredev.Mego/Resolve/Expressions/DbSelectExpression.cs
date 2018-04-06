// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Linq;
    /// <summary>
    /// SELECT查询表达式。
    /// </summary>
    public class DbSelectExpression : DbSetOperationExpression
    {
        /// <summary>
        /// 创建SELECT查询表达式。
        /// </summary>
        /// <param name="source">源数据单元表达式。</param>
        /// <param name="itemType">单元数据项CLR类型。</param>
        public DbSelectExpression(DbUnitTypeExpression source, DbUnitItemTypeExpression itemType)
            : base(typeof(IQueryable<>).MakeGenericType(itemType.ClrType), source, itemType)
        {

        }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.Select;
    }
}