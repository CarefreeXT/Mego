// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using Caredev.Mego.Common;
    using System.Reflection;
    /// <summary>
    /// 删除关系操作。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DbRemoveRelationOperate<T> : DbRelationOperateBase
    {
        /// <summary>
        /// 创建删除关系操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="member">成员属性。</param>
        internal DbRemoveRelationOperate(DbContext context, PropertyInfo member)
            : base(context, member, typeof(T))
        {
        }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.RemoveRelation;
        /// <inheritdoc/>
        internal override void UpdateComplexMember()
        {
            var navigate = this.Navigate;
            foreach (var item in this)
            {
                navigate.UpdateComplexMember(item.Source, null);
            }
        }
        /// <inheritdoc/>
        internal override void UpdatePrimaryMember()
        {
            var navigate = this.Navigate;
            foreach (var item in this)
            {
                navigate.DeleteForeignKey(item.Source, item.Target);
            }
        }
    }
}