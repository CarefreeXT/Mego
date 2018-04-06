// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    using System;
    using Caredev.Mego.Resolve;
    /// <summary>
    /// 数据集接口
    /// </summary>
    public interface IDbSet : IContextContent
    {
        /// <summary>
        /// 数据库名称对象。
        /// </summary>
        DbName Name { get; }
        /// <summary>
        /// 数据项类型。
        /// </summary>
        Type ClrType { get; }
    }
}
