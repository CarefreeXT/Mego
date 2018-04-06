// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Collections.Generic;
    /// <summary>
    /// 单元函数表达式，它表示一个返回集合数据的函数表达式，例如数据库的表值函数对象。
    /// </summary>
    public class DbUnitFunctionExpression : DbUnitTypeExpression, IDbExpandUnitExpression
    {
        /// <summary>
        /// 创建单元函数表达式。
        /// </summary>
        /// <param name="function">数据集函数。</param>
        public DbUnitFunctionExpression(DbSetFunctionExpression function)
            : base(function.ClrType, new DbDataItemExpression(function.ReturnClrType))
        {
            Function = function;
        }
        /// <summary>
        /// 数据集函数。
        /// </summary>
        public DbSetFunctionExpression Function { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.UnitFunction;
        /// <summary>
        /// 当前单元展开的表达式内容。
        /// </summary>
        public IList<DbExpression> ExpandItems { get; } = new List<DbExpression>();
    }
}