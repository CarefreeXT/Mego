// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;
    using Caredev.Mego.Resolve.Outputs;
    /// <summary>
    /// 集合查询操作。
    /// </summary>
    internal class DbQueryCollectionOperate : DbQueryOperateBase
    {
        private readonly static Type IEnumerableType = typeof(IEnumerable<>);
        /// <summary>
        /// 集合查询操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="expression">查询表达式。</param>
        /// <param name="itemType">数据项CLR类型。</param>
        public DbQueryCollectionOperate(DbContext context, Expression expression, Type itemType)
            : base(context, expression, IEnumerableType.MakeGenericType(itemType))
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
        public IEnumerable<object> Result { get; private set; }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.QueryCollection;
        /// <inheritdoc/>
        internal override bool Read(DbDataReader reader)
        {
            Result = ((IMultiOutput)Output).GetResult(reader).ToArray();
            return true;
        }
    }
}