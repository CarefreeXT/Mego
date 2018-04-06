// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    /// <summary>
    /// 删除数据表操作。
    /// </summary>
    internal class DbDropTableOperate : DbMaintenanceOperateBase
    {
        /// <summary>
        /// 创建删除数据表操作。
        /// </summary>
        /// <param name="context">数据上下文</param>
        /// <param name="type">关联CLR类型。</param>
        internal DbDropTableOperate(DbContext context, Type type)
             : base(context, type)
        {
        }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.DropTable;
    }
}