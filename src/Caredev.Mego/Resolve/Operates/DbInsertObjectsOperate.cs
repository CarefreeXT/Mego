// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    /// <summary>
    /// 插入数据对象操作。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    public class DbInsertObjectsOperate<T> : DbObjectsOperateBase, IInsertReferenceRelation
        where T : class
    {
        /// <summary>
        /// 创建插入数据对象操作。
        /// </summary>
        /// <param name="target">操作目标。</param>
        /// <param name="items">数据对象集合。</param>
        internal DbInsertObjectsOperate(DbSet<T> target, IEnumerable<T> items)
            : base(target, items, typeof(T), Expression.Constant(target))
        {
        }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.InsertObjects;
        /// <summary>
        /// <see cref="IInsertReferenceRelation.Relations"/>
        /// </summary>
        ICollection<DbRelationOperateBase> IInsertReferenceRelation.Relations { get; } = new HashSet<DbRelationOperateBase>();
        /// <summary>
        /// 添加单个数据对象。
        /// </summary>
        /// <param name="item">数据对象。</param>
        public void Add(T item) => base.Add(item);
        /// <summary>
        /// 删除单个数据对象。
        /// </summary>
        /// <param name="item">数据对象。</param>
        public void Remove(T item) => base.Remove(item);
        /// <summary>
        /// 添加多个数据对象。
        /// </summary>
        /// <param name="items">数据对象集合。</param>
        public void Add(IEnumerable<T> items) => base.Add(items);
        /// <summary>
        /// 删除多个数据对象。
        /// </summary>
        /// <param name="items">数据对象集合。</param>
        public void Remove(IEnumerable<T> items) => base.Remove(items);
    }
}