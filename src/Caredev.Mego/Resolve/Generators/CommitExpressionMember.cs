// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.ValueGenerates;
    /// <summary>
    /// 表达式提交成员。
    /// </summary>
    public class CommitExpressionMember : CommitMember
    {
        /// <summary>
        /// 创建提交成员。
        /// </summary>
        /// <param name="metadata">列元数据。</param>
        /// <param name="generator">值生成对象。</param>
        /// <param name="expression">当前值的表达式。</param>
        public CommitExpressionMember(ColumnMetadata metadata, ValueGenerateBase generator, DbExpression expression)
            : base(metadata, generator, ECommitValueType.Expression)
        {
            Expression = expression;
        }
        /// <summary>
        /// 值表达式。
        /// </summary>
        public DbExpression Expression { get; }
    }
}