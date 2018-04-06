// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    /// <summary>
    /// 复合关系元数据。
    /// </summary>
    public class CompositeNavigateMetadata : NavigateMetadata
    {
        /// <summary>
        /// 创建复合关系元数据。
        /// </summary>
        /// <param name="source">关系源。</param>
        /// <param name="target">关系目标。</param>
        /// <param name="table">所在的表元数据。</param>
        /// <param name="pairs">关系主外键对。</param>
        /// <param name="cpairs">复合关系主外键对。</param>
        public CompositeNavigateMetadata(
            TableMetadata source, TableMetadata target, TableMetadata table,
            ForeignPrincipalPair[] pairs, ForeignPrincipalPair[] cpairs)
            : base(source, target, pairs)
        {
            RelationTable = table;
            CompositePairs = cpairs;
        }
        /// <inheritdoc/>
        public override bool IsComposite => true;
        /// <summary>
        /// 复合关系中间表。
        /// </summary>
        public TableMetadata RelationTable { get; }
        /// <summary>
        /// 复合关系主外键对。
        /// </summary>
        public ForeignPrincipalPair[] CompositePairs { get; }
    }
}