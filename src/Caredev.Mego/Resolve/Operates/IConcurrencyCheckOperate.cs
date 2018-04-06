// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    /// <summary>
    /// 并发检查操作接口。
    /// </summary>
    internal interface IConcurrencyCheckOperate : IDbSplitObjectsOperate
    {
        /// <summary>
        /// 当前操作是否需要并发检查。
        /// </summary>
        bool NeedCheck { get; }
        /// <summary>
        /// 用于并发检查的期望数量。
        /// </summary>
        int ExpectCount { get; set; }
    }
}