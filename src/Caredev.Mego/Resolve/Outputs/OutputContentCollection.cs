// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    /// <summary>
    /// 输出集合内容，该对象用于生成集合导航数据的中间对象。
    /// </summary>
    public class OutputContentCollection : IEnumerable, IEnumerable<object>
    {
        private readonly IEnumerable<object> Target;
        private readonly CollectionOutputInfo Output;
        private Dictionary<string, OutputContentItem> KeyCollection;
        /// <summary>
        /// 创建输出集合内容。
        /// </summary>
        /// <param name="output">所在的集合输出信息对象。</param>
        /// <param name="target">实际集合目标。</param>
        public OutputContentCollection(CollectionOutputInfo output, object target)
        {
            Output = output;
            Target = target as IEnumerable<object>;
            KeyCollection = new Dictionary<string, OutputContentItem>();
        }
        /// <summary>
        /// 使用指定的键值获取输出内容项。
        /// </summary>
        /// <param name="key">当前键值。</param>
        /// <param name="value">获取的内容项。</param>
        /// <returns>是否执行成功。</returns>
        public bool TryGetValue(string key, out OutputContentItem value)
        {
            return KeyCollection.TryGetValue(key, out value);
        }
        /// <summary>
        /// 添加指定键值及输出内容项。
        /// </summary>
        /// <param name="key">键值。</param>
        /// <param name="value">关联输出内容项。</param>
        public void Add(string key, OutputContentItem value)
        {
            KeyCollection.Add(key, value);
            Output.Metadata.CollectionAdd(Target, value.Content);
        }
        /// <summary>
        /// 实现<see cref="IEnumerable{T}"/>接口
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// 实现<see cref="IEnumerable{T}"/>接口
        /// </summary>
        /// <returns></returns>
        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return Target.GetEnumerator();
        }
    }
}