// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    using System.Linq.Expressions;
    /// <summary>
    /// 数据查询操作基类。
    /// </summary>
    internal abstract class DbQueryOperateBase : DbOperateBase
    {
        /// <summary>
        /// 初始化数据查询操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="expression">查询表达式。</param>
        /// <param name="type">查询项CLR类型。</param>
        internal DbQueryOperateBase(DbContext context, Expression expression, Type type)
            : base(context, type)
        {
            Expression = expression;
        }
        /// <summary>
        /// 查询表达式。
        /// </summary>
        public Expression Expression { get; }
        /// <inheritdoc/>
        public override bool HasResult => true;
    }
}