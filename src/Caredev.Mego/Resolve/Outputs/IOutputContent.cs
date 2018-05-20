// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    /// <summary>
    /// 输出内容接口。
    /// </summary>
    public interface IOutputContent
    {
        /// <summary>
        /// 当前输出内容。
        /// </summary>
        object Content { get; }
    }
}
