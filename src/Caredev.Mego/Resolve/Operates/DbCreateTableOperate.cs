﻿// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    /// <summary>
    /// 创建表操作。
    /// </summary>
    internal class DbCreateTableOperate : DbMaintenanceOperateBase
    {
        /// <summary>
        /// 初始化创建表操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="type">数据项CLR类型。</param>
        /// <param name="operateType">当前操作类型。</param>
        /// <param name="name">操作对象名称。</param>
        internal DbCreateTableOperate(DbContext context, Type type, EOperateType operateType, DbName name)
             : base(context, type, name)
        {
            _Type = operateType;
        }
        /// <inheritdoc/>
        public override EOperateType Type => _Type;
        private readonly EOperateType _Type;
    }
}