// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System.Linq.Expressions;
    using System.Linq;
    using System.Collections.Generic;
    /// <summary>
    /// 删除数据对象操作。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    public class DbDeleteObjectsOperate<T> : DbObjectsOperateBase, IConcurrencyCheckOperate
        where T : class
    {
        /// <summary>
        /// 创建删除数据对象操作。
        /// </summary>
        /// <param name="target">操作目标。</param>
        /// <param name="items">数据对象集合。</param>
        internal DbDeleteObjectsOperate(DbSet<T> target, IEnumerable<T> items)
            : base(target, items, typeof(T), Expression.Constant(target))
        {
            var context = target.Context;
            var metadata = context.Configuration.Metadata.Table(typeof(T));
            if (!context.Configuration.EnableConcurrencyCheck)
            {
                _ItemParameterCount = metadata.Keys.Length;
            }
            else
            {
                var count = metadata.Concurrencys.Length + metadata.InheritSets.Sum(a => a.Concurrencys.Length);
                _NeedCheck = count > 0;
                _ItemParameterCount = metadata.Keys.Length + count;
            }
        }
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
        /// <inheritdoc/>
        public override int ItemParameterCount => _ItemParameterCount;
        private readonly int _ItemParameterCount;
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.DeleteObjects;
        /// <inheritdoc/>
        public override bool HasResult => false;
        /// <summary>
        /// <see cref="IConcurrencyCheckOperate.NeedCheck"/>
        /// </summary>
        bool IConcurrencyCheckOperate.NeedCheck => _NeedCheck;
        private bool _NeedCheck = false;
        /// <summary>
        /// <see cref="IConcurrencyCheckOperate.ExpectCount"/>
        /// </summary>
        int IConcurrencyCheckOperate.ExpectCount { get; set; }
    }
}