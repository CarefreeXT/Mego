// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 分组集合的表达式。
    /// </summary>
    public class DbGroupSetExpression : DbUnitTypeExpression, IDbDefaultUnitType
    {
        /// <summary>
        /// 创建分组集合的表达式。
        /// </summary>
        /// <param name="unit">分组的单元表达式。</param>
        public DbGroupSetExpression(DbUnitTypeExpression unit)
           : base(unit.ClrType, new DbGroupItemExpression(unit.Item))
        {
            Source = unit;
        }
        /// <summary>
        /// 当前表达的所属GroupJoin对象。
        /// </summary>
        public DbGroupJoinExpression Parent { get; set; }
        /// <summary>
        /// 对于外连接时如果目标为空时以<see cref="Default"/>表达式做为值。
        /// </summary>
        public DbExpression Default { get; set; }
        /// <summary>
        /// 分组的源表达式。
        /// </summary>
        public DbUnitTypeExpression Source { get; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.GroupSet;
    }
}