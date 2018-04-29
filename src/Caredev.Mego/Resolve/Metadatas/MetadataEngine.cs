// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.ValueConversion;
    using System;
    using System.Collections.Generic;
    using Res = Properties.Resources;
    /// <summary>
    /// 元数据引擎对象，用于获取各种系统使用的元数据入口。
    /// </summary>
    public class MetadataEngine
    {
        private readonly static Type AttributeType = typeof(TableAttribute);
        //内部构造。
        internal MetadataEngine()
        {
            tableMetadataCache = new Dictionary<Type, TableMetadata>();
            typeMetadataCache = new Dictionary<System.Type, TypeMetadata>();
            typeConversion = new Dictionary<System.Type, ConversionInfo>();
        }
        //数据缓存对象。
        private readonly Dictionary<Type, TableMetadata> tableMetadataCache;
        private readonly Dictionary<Type, TypeMetadata> typeMetadataCache;
        private readonly Dictionary<Type, ConversionInfo> typeConversion;
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
        /// <summary>
        /// 注册全局类型转换器。
        /// </summary>
        /// <param name="conversion">转换器类型。</param>
        public void Register(Type conversion)
        {
            var info = new ConversionInfo(conversion);
            typeConversion.AddOrUpdate(info.ObjectType, info);
        }
        /// <summary>
        /// 尝试获取指定对象类型的转换器。
        /// </summary>
        /// <param name="type">对象属性类型类型。</param>
        /// <param name="conversion">转换器。</param>
        /// <returns>是否成功获取。</returns>
        internal bool TryGetConversion(Type type, out ConversionInfo conversion)
        {
            return typeConversion.TryGetValue(type, out conversion);
        }
    }
}