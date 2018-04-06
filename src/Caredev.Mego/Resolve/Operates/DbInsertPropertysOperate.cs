// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    /// <summary>
    /// 指定属性表达式，插入数据对象操作。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    public class DbInsertPropertysOperate<T> : DbPropertysOperateBase, IInsertReferenceRelation
        where T : class
    {
        /// <summary>
        /// 创建插入数据对象操作。
        /// </summary>
        /// <param name="target">操作目标。</param>
        /// <param name="expression">插入表达式。</param>
        /// <param name="items">数据对象集合。</param>
        internal DbInsertPropertysOperate(DbSet<T> target, Expression expression, IEnumerable<T> items)
            : base(target, expression, items, typeof(T))
        { }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.InsertPropertys;
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