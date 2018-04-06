// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    /// <summary>
    /// 实现<see cref="IGrouping{TKey, TElement}"/>接口，输出分组集合对象。
    /// </summary>
    /// <typeparam name="TKey">主键类型。</typeparam>
    /// <typeparam name="TElement">分组成员类型。</typeparam>
    internal class GroupCollectionImpl<TKey, TElement> : HashSet<TElement>, IGrouping<TKey, TElement>
    {
        /// <summary>
        /// 主键类型。
        /// </summary>
        public TKey Key { get; }
    }
}