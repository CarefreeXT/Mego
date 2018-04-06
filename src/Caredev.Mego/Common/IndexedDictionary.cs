// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    /// <summary>
    /// 索引化字典，该集合强调顺序性。
    /// </summary>
    /// <typeparam name="TKey">字典中的键的类型。</typeparam>
    /// <typeparam name="TValue">字典中的值的类型。</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class IndexedDictionary<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly Dictionary<TKey, IndexValuePair> dictionarys;
        private readonly List<TValue> items;
        /// <summary>
        /// 初始化<see cref="IndexedDictionary{TKey, TValue}"/>新实例，该实例为空且具有默认的初始容量。
        /// </summary>
        public IndexedDictionary()
            : this(10)
        { }
        /// <summary>
        /// 初始化<see cref="IndexedDictionary{TKey, TValue}"/>新实例，该实例为空且具有指定的的初始容量。
        /// </summary>
        /// <param name="capacity">初始容量大小。</param>
        /// <exception cref="ArgumentOutOfRangeException">capacity 小于 0。</exception>
        public IndexedDictionary(int capacity)
        {
            dictionarys = new Dictionary<TKey, IndexValuePair>(capacity);
            items = new List<TValue>(capacity);
        }
        /// <summary>
        /// 初始化<see cref="IndexedDictionary{TKey, TValue}"/>新实例，该实例具有指定的项集合及键选择器函数。
        /// </summary>
        /// <param name="collection">指定的项集合。</param>
        /// <param name="keySelector">键选择器函数。</param>
        public IndexedDictionary(IEnumerable<TValue> collection, Func<TValue, TKey> keySelector)
        {
            Utility.NotNull(collection, nameof(collection));
            Utility.NotNull(keySelector, nameof(keySelector));

            items = new List<TValue>(collection);
            dictionarys = new Dictionary<TKey, IndexValuePair>(items.Capacity);
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                dictionarys.Add(keySelector(item), new IndexValuePair(i, item));
            }
        }
        /// <summary>
        /// 获取指定索引处的元素值。
        /// </summary>
        /// <param name="index">索引值。</param>
        /// <returns>指定索引对应的值。</returns>
        public TValue this[int index] => items[index];
        /// <summary>
        /// 获取指定键的元素值。
        /// </summary>
        /// <param name="key">指定键。</param>
        /// <returns>指定键对应的值。</returns>
        public TValue this[TKey key] => dictionarys[key].Value;
        /// <summary>
        /// 集合包含的元素数量。
        /// </summary>
        public int Count => items.Count;
        /// <summary>
        /// 根据键值获取指定项的索引。
        /// </summary>
        /// <param name="key">指定键。</param>
        /// <returns>指定键值相应的索引。</returns>
        public int IndexOf(TKey key) => dictionarys[key].Index;
        /// <summary>
        /// 根据键值确定当前是否包含该元素。
        /// </summary>
        /// <param name="key">指定键。</param>
        /// <returns>如果找到指定键返回 true；否则为 false。</returns>
        public bool Contains(TKey key) => dictionarys.ContainsKey(key);
        /// <summary>
        /// 添加指定键值对元素，重复键的元素会被忽略。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool Add(TKey key, TValue value)
        {
            if (!dictionarys.ContainsKey(key))
            {
                dictionarys.Add(key, new IndexValuePair(items.Count, value));
                items.Add(value);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 根据键获取指定元素的值。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected TValue GetValueByKey(TKey key) => dictionarys[key].Value;

        #region IEnumerable
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现。
        /// </summary>
        /// <returns></returns>
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => items.GetEnumerator();
        /// <summary>
        /// <see cref="IEnumerable"/>接口实现。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
        #endregion
        //索引值对
        private struct IndexValuePair
        {
            public IndexValuePair(int index, TValue value)
            {
                Value = value;
                Index = index;
            }

            public TValue Value;
            public int Index;
        }
    }
}