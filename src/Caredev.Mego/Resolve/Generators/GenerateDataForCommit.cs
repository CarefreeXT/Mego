// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using System;
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using System.Collections.Generic;
    using Res = Properties.Resources;
    /// <summary>
    /// 用于提交数据的生成数据对象。
    /// </summary>
    public abstract class GenerateDataForCommit : GenerateData
    {
        /// <summary>
        /// 创建数据对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">当前操作对象。</param>
        internal GenerateDataForCommit(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
            if (operate is IConcurrencyCheckOperate concur)
            {
                NeedConcurrencyCheck = concur.NeedCheck;
            }
            Table = context.Metadata.Table(operate.ClrType);
            TableCount = Table.InheritSets.Length + 1;
            Items = operate;
            Loader = CreateLoader(context, Table);
            CommitObject = new CommitObjectFragment(context, Loader);
        }
        /// <summary>
        /// 当前关联的数据集合对象。
        /// </summary>
        internal DbObjectsOperateBase Items { get; }
        /// <summary>
        /// 提交相关的表元数据。
        /// </summary>
        public TableMetadata Table { get; }
        /// <summary>
        /// 当前作用目标名称。
        /// </summary>
        public DbName TargetName => Items.DbSet.Name;
        /// <summary>
        /// 属性值加载对象。
        /// </summary>
        public IPropertyValueLoader Loader { get; }
        /// <summary>
        /// 用于数据提交的对象。
        /// </summary>
        public CommitObjectFragment CommitObject { get; }
        /// <summary>
        /// 当前操作是否需要并发检查。
        /// </summary>
        public bool NeedConcurrencyCheck { get; }
        /// <summary>
        /// 当前操作涉及的表的总数。
        /// </summary>
        public int TableCount { get; }
        /// <summary>
        /// 获取实际需要提交的表（当前表与继承表）的元数据。
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TableMetadata> GetTables()
        {
            foreach (var table in Table.InheritSets)
            {
                yield return table;
            }
            yield return Table;
        }
        /// <summary>
        /// 创建属性值加载对象。
        /// </summary>
        protected virtual IPropertyValueLoader CreateLoader(GenerateContext context, TableMetadata table)
        {
            return new PropertyValueLoader(context, table);
        }
        /// <inheritdoc/>
        public override void Inititalze(DbExpression content = null)
        {
            if (this.Items.DbSet.Name != null && this.Table.InheritSets.Length > 0)
            {
                throw new NotSupportedException(Res.NotSupportedInheritSetUsingDbName);
            }
            if (Items.Count == 1)
            {
                foreach (var obj in Items)
                {
                    Loader.Load(obj);
                }
            }
        }
        /// <summary>
        /// 并发成员列表。
        /// </summary>
        public IEnumerable<ColumnMetadata> ConcurrencyMembers
        {
            get
            {
                if (NeedConcurrencyCheck)
                {
                    foreach (var table in GetTables())
                    {
                        foreach (var member in table.Concurrencys)
                        {
                            yield return member;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 设置并发检查数量。
        /// </summary>
        /// <param name="itemcount"></param>
        public void SetConcurrencyExpectCount(int itemcount)
        {
            if (NeedConcurrencyCheck)
            {
                var con = (IConcurrencyCheckOperate)Items;
                con.ExpectCount = Items.Count * itemcount;
            }
        }
    }
}