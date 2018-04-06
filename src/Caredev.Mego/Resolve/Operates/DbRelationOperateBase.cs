// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    /// <summary>
    /// 关系操作基类。
    /// </summary>
    public abstract class DbRelationOperateBase : DbSplitObjectsOperate<DbRelationItem>
    {
        /// <summary>
        /// 创建关系操作基类。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="member">关系导航属性。</param>
        /// <param name="type">目标数据类型。</param>
        internal DbRelationOperateBase(DbContext context, PropertyInfo member, Type type)
            : base(context, type, null)
        {
            Member = member;

            Table = context.Configuration.Metadata.Table(type);
            Navigate = Table.Navigates[member];
            _ItemParameterCount = Navigate.Source.Keys.Length + Navigate.Target.Keys.Length;
        }
        /// <summary>
        /// 创建关系操作基类。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="member">关系导航属性。</param>
        /// <param name="type">目标数据类型。</param>
        /// <param name="table">当前数据表元数据。</param>
        /// <param name="navigate">当前关系元数据。</param>
        internal DbRelationOperateBase(DbContext context, PropertyInfo member, Type type, TableMetadata table, NavigateMetadata navigate)
            : base(context, type, null)
        {
            Member = member;
            Table = table;
            Navigate = navigate;

            _ItemParameterCount = Navigate.Source.Keys.Length + Navigate.Target.Keys.Length;
        }
        /// <summary>
        /// 所在的表元数据。
        /// </summary>
        public TableMetadata Table { get; }
        /// <summary>
        /// 导航元数据对象。
        /// </summary>
        public NavigateMetadata Navigate { get; }
        /// <summary>
        /// 关系的CLR属性描述对象。
        /// </summary>
        public PropertyInfo Member { get; }
        /// <summary>
        /// 添加关系数据。
        /// </summary>
        /// <param name="source">关系源对象。</param>
        /// <param name="target">关系目标对象。</param>
        public void Add(object source, object target) => Add(new DbRelationItem(source, target));
        /// <summary>
        /// <see cref="IDbSplitObjectsOperate.ItemParameterCount"/>实现
        /// </summary>
        public override int ItemParameterCount => _ItemParameterCount;
        private readonly int _ItemParameterCount;
        /// <inheritdoc/>
        public override bool HasResult => false;
        /// <inheritdoc/>
        internal override bool Read(DbDataReader reader) => true;
        /// <summary>
        /// 更新关系数据对象复杂成员。
        /// </summary>
        internal abstract void UpdateComplexMember();
        /// <summary>
        /// 更新关系数据对象基元成员
        /// </summary>
        internal abstract void UpdatePrimaryMember();
    }
}