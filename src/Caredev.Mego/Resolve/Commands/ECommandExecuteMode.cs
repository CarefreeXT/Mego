// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Commands
{
    using System;
    using Caredev.Mego.Resolve.Operates;
    /// <summary>
    /// 命令执行模式。
    /// </summary>
    [Flags]
    public enum ECommandExecuteMode
    {
        /// <summary>
        /// 默认执行模式。
        /// </summary>
        Simple = 0x01,
        /// <summary>
        /// 按分号分割语句执行。
        /// </summary>
        Split = 0x02,
        /// <summary>
        /// 对于提交操作的数据，使用循环方式执行。
        /// </summary>
        Loop = 0x03
    }
}