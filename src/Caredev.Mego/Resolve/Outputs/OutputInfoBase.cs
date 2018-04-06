// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    /// <summary>
    /// 输出信息基类。
    /// </summary>
    public abstract class OutputInfoBase
    {
        /// <summary>
        /// 输出类型。
        /// </summary>
        public abstract EOutputType Type { get; }
    }
}