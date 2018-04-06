// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    using System.Data.Common;
    /// <summary>
    /// 数据库维护操作基类。
    /// </summary>
    public abstract class DbMaintenanceOperateBase : DbOperateBase
    {
        /// <summary>
        /// 创建数据库维护操作。
        /// </summary>
        /// <param name="context">数据库上下文。</param>
        /// <param name="type">相关的CLR类型。</param>
        internal DbMaintenanceOperateBase(DbContext context, Type type)
           : base(context, type)
        { }
        /// <summary>
        /// 操作名称。
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// 操作架构名。
        /// </summary>
        public string Schema { get; internal set; }
        /// <inheritdoc/>
        public override bool HasResult => false;
        /// <inheritdoc/>
        internal override bool Read(DbDataReader reader) => true;
        /// <inheritdoc/>
        internal override string GenerateSql()
        {
            var context = Executor.Context;
            return context.Database.Generator.Generate(this, null);
        }
    }
}