// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.ValueGenerates
{
    using System;
    /// <summary>
    /// 值生成目的。
    /// </summary>
    [Flags]
    public enum EGeneratedPurpose
    {
        /// <summary>
        /// 插入时生成。
        /// </summary>
        Insert = 1,
        /// <summary>
        /// 更新时生成。
        /// </summary>
        Update = 2,
        /// <summary>
        /// 插入或更新时生成。
        /// </summary>
        InsertUpdate = 3
    }
}