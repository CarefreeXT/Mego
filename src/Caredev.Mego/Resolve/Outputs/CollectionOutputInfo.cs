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
            Initialize();
            if (!HasCollectionProperty)
            {
                return CreateSimpleCollection(reader);
            }
            else
            {
                var type = typeof(HashSet<>).MakeGenericType(Metadata.ClrType);
                var items = new OutputContentCollection((CollectionOutputInfo)this, Activator.CreateInstance(type));
                while (reader.Read())
                {
                    LoadDataToContent(reader, items);
                }
                return items.Content as IEnumerable<object>;
            }
        }

        private IEnumerable<object> CreateSimpleCollection(DbDataReader reader)
        {
            while (reader.Read())
            {
                yield return CreateObjectItem(reader);
            }
        }
        /// <inheritdoc/>
        protected override void LoadDataToContent(DbDataReader reader, IOutputContent content)
        {
            if (!IsEmpty(reader))
            {
                var collection = (OutputContentCollection)content;
                if (HasCollectionProperty)
                {
                    string key = string.Join("_", ItemKeyFields.Select(a => reader.GetValue(a).ToString()).ToArray());
                    if (!collection.TryGetValue(key, out OutputContentObject item))
                    {
                        item = CreateContentItem(reader);
                        collection.Add(key, item);
                    }
                    base.LoadDataToContent(reader, item);
                }
                else
                {
                    collection.Add(CreateObjectItem(reader));
                }
            }
        }
    }
}