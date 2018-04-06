// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using System.Reflection;
    /// <summary>
    /// 属性元数据接口。
    /// </summary>
    public interface IPropertyMetadata
    {
        /// <summary>
        /// 当前元数据成员的CLR描述对象。
        /// </summary>
        PropertyInfo Member { get; }
    }
}