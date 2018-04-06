// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    /// <summary>
    /// 判断数据库对象存在性操作。
    /// </summary>
    internal class DbObjectExsitOperate : DbMaintenanceOperateBase
    {
        /// <summary>
        /// 创建存在判断操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="type">相关CLR类型。</param>
        /// <param name="kind">数据库对象种类。</param>
        internal DbObjectExsitOperate(DbContext context, Type type, EDatabaseObject kind)
            : base(context, type ?? typeof(object))
        {
            Kind = kind;
        }
        /// <summary>
        /// 数据库对象种类。
        /// </summary>
        public EDatabaseObject Kind { get; }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.ObjectExsit;
    }
}