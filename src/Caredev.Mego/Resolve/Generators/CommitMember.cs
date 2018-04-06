// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.ValueGenerates;
    using System.Collections.Generic;
    /// <summary>
    /// 普通提交成员。
    /// </summary>
    public class CommitMember
    {
        /// <summary>
        /// 创建普通提交成员。
        /// </summary>
        /// <param name="metadata">列元数据。</param>
        public CommitMember(ColumnMetadata metadata)
            : this(metadata, null, ECommitValueType.Constant)
        {
        }
        /// <summary>
        /// 创建普通提交成员。
        /// </summary>
        /// <param name="metadata">列元数据。</param>
        /// <param name="gnerator">值生成器。</param>
        /// <param name="columnType">列提交类型。</param>
        public CommitMember(ColumnMetadata metadata, ValueGenerateBase gnerator, ECommitValueType columnType)
        {
            Metadata = metadata;
            Generator = gnerator;
            ValueType = columnType;
        }
        /// <summary>
        /// 当前列元数据。
        /// </summary>
        public ColumnMetadata Metadata { get; }
        /// <summary>
        /// 值生成对象。
        /// </summary>
        public ValueGenerateBase Generator { get; }
        /// <summary>
        /// 提交类型。
        /// </summary>
        public ECommitValueType ValueType { get; }
    }
}