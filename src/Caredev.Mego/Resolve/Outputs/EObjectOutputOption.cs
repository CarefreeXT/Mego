// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    /// <summary>
    /// 对象输出选项，用于约束对象输出结果是否合法。
    /// </summary>
    public enum EObjectOutputOption : int
    {
        /// <summary>
        /// 只返回一个结果。
        /// </summary>
        One = 1,
        /// <summary>
        /// 只返回零个或一个结果。
        /// </summary>
        ZeroOrOne = 0,
        /// <summary>
        /// 有且仅有一个结果。
        /// </summary>
        OnlyOne = 2,
        /// <summary>
        /// 没有结果或有且仅有一个结果。
        /// </summary>
        ZeroOrOnlyOne = 3
    }
}