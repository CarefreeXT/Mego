// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// 可分割的操作对象。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DbSplitObjectsOperate<T> : DbOperateBase, IEnumerable<T>, IDbSplitObjectsOperate
    {
        private HashSet<T> _Items;
        private IEnumerable<T> _SubItems;
        private int _SubCount = 0;
        /// <summary>
        /// 创建分割操作对象。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="type">操作数据类型。</param>
        /// <param name="items">初始化集合。</param>
        public DbSplitObjectsOperate(DbContext context, Type type, IEnumerable<T> items)
            : base(context, type)
        {
            _Items = items == null ? new HashSet<T>() : new HashSet<T>(items);
        }
        /// <summary>
        /// 每个对象提交时所需要的参数数量。
        /// </summary>
        public abstract int ItemParameterCount { get; }
        /// <summary>
        /// 当前操作中包含的提交对象的数量。
        /// </summary>
        public int Count => _SubItems == null ? _Items.Count : _SubCount;
        /// <summary>
        /// 判断当前是否已包含指定对象。
        /// </summary>
        /// <param name="item">用于判断的对象。</param>
        /// <returns>如果包含则返回 true，否则返回 false。</returns>
        internal bool Contains(T item) => _Items.Contains(item);
        /// <summary>
        /// 添加单个数据对象。
        /// </summary>
        /// <param name="item">添加的对象。</param>
        public void Add(T item)
        {
            lock (_Items) { _Items.Add(item); }
        }
        /// <summary>
        /// 删除单个数据对象。
        /// </summary>
        /// <param name="item">删除的对象。</param>
        protected void Remove(T item)
        {
            lock (_Items) { _Items.Remove(item); }
        }
        /// <summary>
        /// 添加多个数据对象。
        /// </summary>
        /// <param name="items">添加的对象集合。</param>
        internal void Add(IEnumerable<T> items)
        {
            lock (_Items) { _Items.UnionWith(items); }
        }
        /// <summary>
        /// 删除多个数据对象。
        /// </summary>
        /// <param name="items">删除的对象集合。</param>
        protected void Remove(IEnumerable<T> items)
        {
            lock (_Items) { _Items.ExceptWith(items); }
        }
        /// <summary>
        /// 实现<see cref="IEnumerable{T}"/>接口。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (_SubItems == null)
            {
                return _Items.GetEnumerator();
            }
            return _SubItems.GetEnumerator();
        }
        /// <summary>
        /// 实现<see cref="IEnumerable"/>接口。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// 在指定函数调用过程中，从指定的索引分割当前操作为指定长度的集合。
        /// </summary>
        /// <param name="index">开始索引。</param>
        /// <param name="length">操作长度。</param>
        /// <param name="action">执行的操作。</param>
        public void Split(int index, int length, Action action)
        {
            if (length > 0 && index >= 0 && length + index <= Count)
            {
                try
                {
                    _SubItems = _Items.Skip(index).Take(length);
                    _SubCount = length;
                    action();
                }
                finally
                {
                    _SubItems = null;
                }
            }
            else
            {
                action();
            }
        }
    }
}
