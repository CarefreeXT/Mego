// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using System;
    /// <summary>
    /// 类型元数据基类。
    /// </summary>
    public abstract class TypeMetadataBase
    {
        /// <summary>
        /// 创建类型元数据基类。
        /// </summary>
        /// <param name="itemType">CLR类型。</param>
        /// <param name="engine">元数据引擎。</param>
        public TypeMetadataBase(Type itemType, MetadataEngine engine)
        {
            Engine = engine;
            ClrType = itemType;
        }
        /// <summary>
        /// CLR类型。
        /// </summary>
        public Type ClrType { get; }
        /// <summary>
        /// 元数据引擎。
        /// </summary>
        public MetadataEngine Engine { get; }
    }
}