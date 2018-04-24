// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Res = Properties.Resources;
    /// <summary>
    /// 提交数据内容对象。
    /// </summary>
    public abstract class CommitContentBase : OperateContentBase
    {
        /// <summary>
        /// 创建提交内容对象。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="operate"></param>
        internal CommitContentBase(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
            if (operate is IConcurrencyCheckOperate concur)
            {
                NeedConcurrencyCheck = concur.NeedCheck;
            }
            Table = context.Metadata.Table(operate.ClrType);
            Items = operate;
            Items = operate;
            Loader = CreateLoader(context, Table);
            CommitObject = new CommitObjectFragment(context, Loader);
        }
        /// <summary>
        /// 对象集操作对象。
        /// </summary>
        public DbObjectsOperateBase Items { get; }
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
        /// 并发成员列表。
        /// </summary>
        public virtual IEnumerable<ColumnMetadata> ConcurrencyMembers
        {
            get
            {
                if (NeedConcurrencyCheck)
                {
                    foreach (var member in Table.Concurrencys)
                    {
                        yield return member;
                    }
                }
            }
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
            base.Inititalze(content);
            if (Items.Count == 1)
            {
                foreach (var obj in Items)
                {
                    Loader.Load(obj);
                }
            }
        }
        /// <summary>
        /// 连接并发成员集合，如果当前操作不需要并发检查则会忽略成员。
        /// </summary>
        /// <param name="table">指的表元数据。</param>
        /// <param name="columns">指定合并的列集合。</param>
        /// <returns>合并结果。</returns>
        public IEnumerable<ColumnMetadata> UnionConcurrencyMembers(TableMetadata table, IEnumerable<ColumnMetadata> columns)
        {
            if (NeedConcurrencyCheck)
            {
                return columns.Union(table.Concurrencys);
            }
            else
            {
                return columns;
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