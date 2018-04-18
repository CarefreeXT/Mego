// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    /// <summary>
    /// 数据关系的引用行为。
    /// </summary>
    public enum EReferenceAction
    {
        /// <summary>
        /// 级联删除。
        /// </summary>
        Cascade,
        /// <summary>
        /// 设置为空。
        /// </summary>
        SetNull,
        /// <summary>
        /// 设置为默认值。
        /// </summary>
        SetDefault,
        /// <summary>
        /// 不执行任何操作。
        /// </summary>
        NoAction,
    }
}