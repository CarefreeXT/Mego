// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using Res = Properties.Resources;
    /// <summary>
    /// 语句式提交数据。
    /// </summary>
    public class StatementContent : OperateContentBase
    {
        /// <summary>
        /// 创建数据对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">当前操作对象。</param>
        internal StatementContent(GenerateContext context, DbStatementOperateBase operate)
            : base(context, operate)
        {
            Statement = operate;
            Table = context.Metadata.Table(operate.DbSet.ClrType);
        }
        /// <summary>
        /// 当前操作语句对象。
        /// </summary>
        public DbStatementOperateBase Statement { get; }
        /// <summary>
        /// 当前作用数据表元数据。
        /// </summary>
        public TableMetadata Table { get; }
        /// <summary>
        /// 当前作用目标名称。
        /// </summary>
        public DbName TargetName => Statement.DbSet.Name;
        /// <summary>
        /// 数据项表达式。
        /// </summary>
        public DbUnitItemTypeExpression ItemEpxression { get; private set; }
        /// <inheritdoc/>
        public override void Inititalze(DbExpression content = null)
        {
            base.Inititalze(content);
            if (Table.InheritSets.Length > 0 && Statement.DbSet.Name != null)
            {
                throw new NotSupportedException(Res.NotSupportedInheritSetUsingDbName);
            }
            var item = ((DbUnitTypeExpression)content).Item;
            if (Operate.Type == EOperateType.DeleteStatement)
            {
                if (item is DbUnitItemContentExpression itemcontent)
                {
                    item = itemcontent.Content;
                }
                if (item.ExpressionType != EExpressionType.DataItem)
                {
                    throw new GenerateException(Res.ExceptionUnitItemIsMustDataItem);
                }
            }
            else
            {
                if (!(content is DbSelectExpression unittype))
                {
                    throw new ArgumentException(Res.ExceptionInsertContentIsSelect, nameof(content));
                }
                if (!(item is DbNewExpression newitem))
                {
                    throw new GenerateException(Res.ExceptionInsertItemIsMustNew);
                }
            }
            ItemEpxression = item;
        }
    }
}