// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.ValueGenerates
{
    /// <summary>
    /// 值生成定义基类。
    /// </summary>
    public abstract class ValueGenerateBase
    {
        /// <summary>
        /// 当前值生成的选项。
        /// </summary>
        public abstract EGeneratedOption GeneratedOption { get; }
    }
}