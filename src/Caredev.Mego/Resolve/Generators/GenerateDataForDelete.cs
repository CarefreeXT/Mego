// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// 用于删除数据的生成数据对象。
    /// </summary>
    public class GenerateDataForDelete : GenerateDataForCommit
    {
        /// <summary>
        /// 创建数据对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">当前操作对象。</param>
        internal GenerateDataForDelete(GenerateContext context, DbObjectsOperateBase operate)
          : base(context, operate)
        {
        }
        /// <inheritdoc/>
        protected override IPropertyValueLoader CreateLoader(GenerateContext context, TableMetadata table)
        {
            if (this.NeedConcurrencyCheck)
            {
                return new PropertyValueLoader(context, table, table.Keys.Concat(ConcurrencyMembers).ToArray());
            }
            else
            {
                return new PropertyValueLoader(context, table, table.Keys);
            }
        }
        /// <inheritdoc/>
        public override IEnumerable<TableMetadata> GetTables() => base.GetTables().Reverse();
    }
}