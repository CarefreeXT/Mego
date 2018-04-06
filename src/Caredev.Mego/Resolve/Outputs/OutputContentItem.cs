// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    /// <summary>
    /// 输出内容项，用于输出对象时产生的中间对象。
    /// </summary>
    public class OutputContentItem
    {
        /// <summary>
        /// 创建输出内容项。
        /// </summary>
        public OutputContentItem()
        {
            Collections = new Dictionary<CollectionOutputInfo, OutputContentCollection>();
        }
        /// <summary>
        /// 当前输出内容。
        /// </summary>
        public object Content { get; set; }
        /// <summary>
        /// 当前输出的集合。
        /// </summary>
        public Dictionary<CollectionOutputInfo, OutputContentCollection> Collections { get; }
    }
}