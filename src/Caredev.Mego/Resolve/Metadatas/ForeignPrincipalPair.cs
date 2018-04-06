// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    /// <summary>
    /// 描述关系的主外键对。
    /// </summary>
    public class ForeignPrincipalPair
    {
        /// <summary>
        /// 创建主外键对。
        /// </summary>
        /// <param name="foreign">外键列元数据。</param>
        /// <param name="principal">主键列元数据。</param>
        public ForeignPrincipalPair(ColumnMetadata foreign, ColumnMetadata principal)
        {
            ForeignKey = foreign;
            PrincipalKey = principal;
        }
        /// <summary>
        /// 外键列元数据。
        /// </summary>
        public ColumnMetadata ForeignKey { get; }
        /// <summary>
        /// 主键列元数据。
        /// </summary>
        public ColumnMetadata PrincipalKey { get; }
    }
}