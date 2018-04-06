// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    /// <summary>
    /// 对象关系模式。
    /// </summary>
    public enum EObjectRelationMode
    {
        /// <summary>
        /// 未设置。
        /// </summary>
        None = 0,
        /// <summary>
        /// 允许为空。
        /// </summary>
        Nullable = 1,
        /// <summary>
        /// 必须关系。
        /// </summary>
        Required = 2,
        /// <summary>
        /// 关系主体。
        /// </summary>
        Principal = 3
    }
}