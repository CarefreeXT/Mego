// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Reflection;
    /// <summary>
    /// 添加关系操作。
    /// </summary>
    /// <typeparam name="T">源数据类型。</typeparam>
    internal class DbAddRelationOperate<T> : DbRelationOperateBase
    {
        /// <summary>
        /// 添加关系操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="member">成员CLR描述对象。</param>
        internal DbAddRelationOperate(DbContext context, PropertyInfo member)
            : base(context, member, typeof(T))
        { }
        /// <summary>
        /// 添加关系操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="member">成员CLR描述对象。</param>
        /// <param name="type">相关源类型。</param>
        /// <param name="table">相关表元数据。</param>
        /// <param name="navigate">相关导航元数。</param>
        internal DbAddRelationOperate(DbContext context, PropertyInfo member, Type type, TableMetadata table, NavigateMetadata navigate)
            : base(context, member, type, table, navigate)
        { }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.AddRelation;
        /// <inheritdoc/>
        internal override void UpdateComplexMember()
        {
            var navigate = this.Navigate;
            foreach (var item in this)
            {
                navigate.UpdateComplexMember(item.Source, item.Target);
            }
        }
        /// <inheritdoc/>
        internal override void UpdatePrimaryMember()
        {
            var navigate = this.Navigate;
            foreach (var item in this)
            {
                navigate.CreateForeignKey(item.Source, item.Target);
            }
        }
    }
}