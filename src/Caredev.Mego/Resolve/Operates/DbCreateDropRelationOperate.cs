// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using Caredev.Mego.Resolve.Metadatas;
    using System.Linq;
    /// <summary>
    /// 创建或删除关系操作。
    /// </summary>
    internal class DbCreateDropRelationOperate : DbMaintenanceOperateBase
    {
        /// <summary>
        /// 初始化创建或删除关系操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="type">操作类型。</param>
        /// <param name="principal">主表元数据。</param>
        /// <param name="foreign">外表元数据。</param>
        /// <param name="pairs">主外键对集合。</param>
        internal DbCreateDropRelationOperate(DbContext context, EOperateType type,
            TableMetadata principal, TableMetadata foreign, ForeignPrincipalPair[] pairs)
             : base(context, typeof(object))
        {
            _Type = type;
            Foreign = foreign;
            Principal = principal;
            Pairs = pairs;

            ForeignName = $"FK_{foreign.Name}_{principal.Name}_{string.Join("_", pairs.Select(a => a.ForeignKey.Name).ToArray())}_{string.Join("_", pairs.Select(a => a.PrincipalKey.Name).ToArray())}";
        }
        /// <summary>
        /// 外键表元数据。
        /// </summary>
        public TableMetadata Foreign { get; }
        /// <summary>
        /// 主表元数据。
        /// </summary>
        public TableMetadata Principal { get; }
        /// <summary>
        /// 主外键对集合。
        /// </summary>
        public ForeignPrincipalPair[] Pairs { get; }
        /// <summary>
        /// 外键名（由系统自动生成）。
        /// </summary>
        public string ForeignName { get; }
        /// <inheritdoc/>
        public override EOperateType Type => _Type;
        private EOperateType _Type;
    }
}