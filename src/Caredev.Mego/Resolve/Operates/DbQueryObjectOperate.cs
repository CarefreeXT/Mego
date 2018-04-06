// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using Caredev.Mego.Resolve.Outputs;
    using System;
    using System.Data.Common;
    using System.Linq.Expressions;
    /// <summary>
    /// 查询单个对象操作。
    /// </summary>
    internal class DbQueryObjectOperate : DbQueryOperateBase
    {
        /// <summary>
        /// 创建查询单个对象操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="expression">查询表达式。</param>
        /// <param name="itemType">数据项CLR类型。</param>
        public DbQueryObjectOperate(DbContext context, Expression expression, Type itemType)
            : base(context, expression, itemType)
        {
            ItemType = itemType;
        }
        /// <summary>
        /// 数据项CLR类型。
        /// </summary>
        public Type ItemType { get; }
        /// <summary>
        /// 返回结果。
        /// </summary>
        public object Result { get; private set; }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.QueryObject;
        /// <inheritdoc/>
        internal override bool Read(DbDataReader reader)
        {
            Result = ((ISingleOutput)Output).GetResult(reader);
            return true;
        }
    }
}