// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Fragments
{
    using System.Collections;
    using System.Collections.Generic;
    /// <summary>
    ///  SQL 语句块片段。
    /// </summary>
    public class BlockFragment : SqlFragment, IEnumerable<ISqlFragment>
    {
        private List<ISqlFragment> _Fragments = new List<ISqlFragment>();
        /// <summary>
        /// 创建 SQL 语句块片段对象。
        /// </summary>
        /// <param name="context">生成语句上下文。</param>
        /// <param name="fragments">语句片段对象。</param>
        public BlockFragment(GenerateContext context, params ISqlFragment[] fragments)
            : base(context)
        {
            _Fragments = new List<ISqlFragment>(fragments);
        }
        /// <inheritdoc/>
        public override bool HasTerminator => false;
        /// <summary>
        /// 添加语句片段对象。
        /// </summary>
        /// <param name="fragment">添加的对象。</param>
        public void Add(ISqlFragment fragment) => _Fragments.Add(fragment);
        /// <summary>
        /// 添加语句多个片段对象。
        /// </summary>
        /// <param name="collection">添加的对象集合。</param>
        public void Add(IEnumerable<ISqlFragment> collection) => _Fragments.AddRange(collection);
        /// <summary>
        /// 向指定索引处插入语句片段对象。
        /// </summary>
        /// <param name="index">指定索引。</param>
        /// <param name="fragment">插入的对象。</param>
        public void Insert(int index, ISqlFragment fragment) => _Fragments.Insert(0, fragment);
        /// <summary>
        /// 向指定索引处插入多个语句片段对象。
        /// </summary>
        /// <param name="index">指定索引。</param>
        /// <param name="collection">插入的对象集合。</param>
        public void Insert(int index, IEnumerable<ISqlFragment> collection) => _Fragments.InsertRange(0, collection);
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现。
        /// </summary>
        /// <returns>枚举器对象。</returns>
        public IEnumerator<ISqlFragment> GetEnumerator() => _Fragments.GetEnumerator();
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现。
        /// </summary>
        /// <returns>枚举器对象。</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}