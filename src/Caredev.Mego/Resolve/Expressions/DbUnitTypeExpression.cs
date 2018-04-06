// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// 单元类型表达式。
    /// </summary>
    public abstract class DbUnitTypeExpression : DbExpression, IDbUnitTypeExpression
    {
        /// <summary>
        /// 创建单元类型表达式。
        /// </summary>
        /// <param name="type">数据项CLR类型。</param>
        /// <param name="itemType">数据项表达式。</param>
        public DbUnitTypeExpression(Type type, DbUnitItemTypeExpression itemType)
        {
            ClrType = type;
            ReplaceItem(itemType);
        }
        /// <summary>
        /// 数据项CLR类型。
        /// </summary>
        public Type ClrType { get; private set; }
        /// <summary>
        /// 数据项表达式。
        /// </summary>
        public DbUnitItemTypeExpression Item { get; private set; }
        /// <summary>
        /// 筛选表达式集合。
        /// </summary>
        public ICollection<DbExpression> Filters
        {
            get
            {
                if (_Filters == null)
                    _Filters = new List<DbExpression>();
                return _Filters;
            }
        }
        private ICollection<DbExpression> _Filters;
        /// <summary>
        /// 排序表达式集合。
        /// </summary>
        public ICollection<DbOrderExpression> Orders
        {
            get
            {
                if (_Orders == null)
                    _Orders = new List<DbOrderExpression>();
                return _Orders;
            }
        }
        private ICollection<DbOrderExpression> _Orders;
        /// <summary>
        /// 检索前 N 条数据，如果为0表示不限制。
        /// </summary>
        public int Take { get; set; } = 0;
        /// <summary>
        /// 跳过 N 条检索数据，如果为0表示不限制。
        /// </summary>
        public int Skip { get; set; } = 0;
        /// <summary>
        /// 排除重复项检索数据。
        /// </summary>
        public bool Distinct { get; set; } = false;
        /// <summary>
        /// 合并另一个单元表达式。
        /// </summary>
        /// <param name="source">合并目标表达式。</param>
        /// <returns>合并后的表达式。</returns>
        public DbUnitTypeExpression Merge(DbUnitTypeExpression source)
        {
            this.Take = source.Take;
            this.Skip = source.Skip;
            this.Distinct = source.Distinct;
            if (source._Filters != null && source._Filters.Count > 0)
            {
                foreach (var item in source._Filters)
                    this.Filters.Add(item);
            }
            if (source._Orders != null && source._Orders.Count > 0)
            {
                foreach (var item in source._Orders)
                    this.Orders.Add(item);
            }
            return this;
        }
        /// <summary>
        /// 替换当前的数据项表达式。
        /// </summary>
        /// <param name="item">替换表达式。</param>
        /// <returns>当前单元表达式。</returns>
        public DbUnitTypeExpression ReplaceItem(DbUnitItemTypeExpression item)
        {
            Item = item;
            item.Unit = this;
            return this;
        }
    }
}