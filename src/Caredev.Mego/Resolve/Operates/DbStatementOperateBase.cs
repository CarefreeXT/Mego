// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    using System.Data.Common;
    using System.Linq.Expressions;
    /// <summary>
    /// 语句数据操作基类。
    /// </summary>
    public abstract class DbStatementOperateBase : DbOperateBase
    {
        /// <summary>
        /// 根据表达式创建语句操作。
        /// </summary>
        /// <param name="target">操作目标。</param>
        /// <param name="expression">语句操作表达式。</param>
        /// <param name="type">操作数据的CLR类型。</param>
        internal DbStatementOperateBase(IDbSet target, Expression expression, Type type)
            : base(target.Context, type)
        {
            DbSet = target;
            Expression = expression;
        }
        /// <summary>
        /// 目标数据集。
        /// </summary>
        public IDbSet DbSet { get; }
        /// <summary>
        /// 操作相关的LINQ表达式。
        /// </summary>
        public Expression Expression { get; }
        /// <inheritdoc/>
        public override bool HasResult => false;
        /// <inheritdoc/>
        internal override bool Read(DbDataReader reader) => true;
    }
}