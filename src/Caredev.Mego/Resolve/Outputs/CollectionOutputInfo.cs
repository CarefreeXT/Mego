// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    /// <summary>
    /// 集合输出信息对象。
    /// </summary>
    public class CollectionOutputInfo : ComplexOutputInfo, IMultiOutput
    {
        /// <summary>
        /// 创建集合输出信息对象。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <param name="metadata">类型元数据。</param>
        public CollectionOutputInfo(DbDataReader reader, TypeMetadata metadata) : base(reader, metadata)
        {
        }
        /// <summary>
        /// 创建集合输出信息对象。
        /// </summary>
        /// <param name="metadata">类型元数据。</param>
        /// <param name="fields">属性索引列表。</param>
        public CollectionOutputInfo(TypeMetadata metadata, int[] fields)
            : base(metadata, fields)
        {
        }
        /// <inheritdoc/>
        public override EOutputType Type => EOutputType.Collection;
        /// <summary>
        /// 执行结果。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <returns>执行结果。</returns>
        public IEnumerable<object> GetResult(DbDataReader reader)
        {
            if (Initialize())
            {
                var type = typeof(HashSet<>).MakeGenericType(Metadata.ClrType);
                var items = new OutputContentCollection(this, Activator.CreateInstance(type));
                while (reader.Read())
                {
                    RegisterItem(reader, items);
                }
                return items;
            }
            return GetResultObjectItem(reader);
        }
        //注册使用内容项，用于生成集合属性用。
        private void RegisterItem(DbDataReader reader, OutputContentCollection collection)
        {
            string key = string.Join("_", ItemKeyFields.Select(a => reader.GetValue(a).ToString()).ToArray());
            if (!collection.TryGetValue(key, out OutputContentItem item))
            {
                item = new OutputContentItem();
                var content = CreateItem(reader, item);
                if (content != null)
                {
                    item.Content = content;
                    collection.Add(key, item);
                }
            }
            foreach (var kv in item.Collections)
            {
                kv.Key.RegisterItem(reader, kv.Value);
            }
        }
        //获取结果对象集合。
        private IEnumerable<object> GetResultObjectItem(DbDataReader reader)
        {
            while (reader.Read())
            {
                yield return CreateItem(reader);
            }
        }
    }
}