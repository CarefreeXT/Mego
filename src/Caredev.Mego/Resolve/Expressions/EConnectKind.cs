// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 数据集连接操作各类。
    /// </summary>
    public enum EConnectKind
    {
        /// <summary>
        /// 并集。
        /// </summary>
        Union = 1,
        /// <summary>
        /// 交集
        /// </summary>
        Intersect = 2,
        /// <summary>
        /// 排除。
        /// </summary>
        Except = 3,
        /// <summary>
        /// 连接。
        /// </summary>
        Concat = 4,
    }
}