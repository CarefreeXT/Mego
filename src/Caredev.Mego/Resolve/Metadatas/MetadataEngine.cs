// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Expressions;
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// 元数据引擎对象，用于获取各种系统使用的元数据入口。
    /// </summary>
    public class MetadataEngine
    {
        private readonly static Type AttributeType = typeof(TableAttribute);
        //内部构造。
        internal MetadataEngine() { }
        //数据缓存对象。
        private readonly Dictionary<Type, TableMetadata> tableMetadataCache = new Dictionary<Type, TableMetadata>();
        private readonly Dictionary<Type, TypeMetadata> typeMetadataCache = new Dictionary<System.Type, TypeMetadata>();
        /// <summary>
        /// 通过CLR类型获取相应的数据表元数据。
        /// </summary>
        /// <param name="clrType">指定的CLR类型。</param>
        /// <returns>查找的数据表元数据。</returns>
        public TableMetadata Table(Type clrType)
        {
            if (!tableMetadataCache.TryGetValue(clrType, out TableMetadata data))
            {
                var attr = Attribute.GetCustomAttribute(clrType, AttributeType) as TableAttribute;
                if (attr == null)
                    data = new TableMetadata(clrType, clrType.Name, this);
                else
                    data = new TableMetadata(clrType, attr, this);
                tableMetadataCache.Add(clrType, data);
            }
            return data;
        }
        /// <summary>
        /// 通过<see cref="DbDataSetExpression"/>相应的数据表元数据。
        /// </summary>
        /// <param name="exp">数据集对象。</param>
        /// <returns>查找的数据表元数据。</returns>
        internal TableMetadata Table(DbDataSetExpression exp)
        {
            return Table(exp.ClrType.GetGenericArguments()[0]);
        }
        /// <summary>
        /// 尝试通过CLR类型获取相应的数据表元数据。
        /// </summary>
        /// <param name="clrType">指定的CLR类型。</param>
        /// <returns>查找的数据表元数据。</returns>
        public TableMetadata TryGetTable(Type clrType)
        {
            if (!tableMetadataCache.TryGetValue(clrType, out TableMetadata result))
            {
                try
                {
                    result = Table(clrType);
                }
                catch { }
            }
            return result;
        }
        /// <summary>
        /// 通过CLR类型获取相应的类型元数据。
        /// </summary>
        /// <param name="clrType">指定的CLR类型。</param>
        /// <returns>查找的类型元数据。</returns>
        public TypeMetadata Type(Type clrType)
        {
            if (!typeMetadataCache.TryGetValue(clrType, out TypeMetadata data))
            {
                data = new TypeMetadata(clrType, this);
                typeMetadataCache.Add(clrType, data);
            }
            return data;
        }
    }
}