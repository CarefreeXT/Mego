// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// 数据集对象表达式，对应于数据库表（包含临时表）或视图
    /// </summary>
    public class DbDataSetExpression : DbUnitTypeExpression, IDbExpandUnitExpression
    {
        /// <summary>
        /// 数据集对象表达式
        /// </summary>
        /// <param name="type">数据对象类型。</param>
        /// <param name="name">数据集名称。</param>
        public DbDataSetExpression(Type type, DbName name = null)
            : base(type, new DbDataItemExpression(type.GetGenericArguments()[0]))
        {
            Name = name;
        }
        /// <summary>
        /// 当前数据集展开的成员表达式集合。
        /// </summary>
        public IList<DbExpression> ExpandItems { get; } = new List<DbExpression>();
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.DataSet;
        /// <summary>
        /// 自定义名称。
        /// </summary>
        internal DbName Name { get; }
    }
}