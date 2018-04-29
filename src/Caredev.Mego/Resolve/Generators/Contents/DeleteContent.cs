// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using System.Linq;
    /// <summary>
    /// 更新数据的内容对象。
    /// </summary>
    public class DeleteContent : ContentBase
    {
        /// <summary>
        /// 创建内容对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">操作对象。</param>
        internal DeleteContent(GenerateContext context, DbObjectsOperateBase operate)
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
    }
}