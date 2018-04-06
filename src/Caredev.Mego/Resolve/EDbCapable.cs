// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    /// <summary>
    /// 描述特定数据库拥有的高级特性。
    /// </summary>
    [Flags]
    public enum EDbCapable : ulong
    {
        /// <summary>
        /// 构架。
        /// </summary>
        Schema = 0x00000001,
        /// <summary>
        /// 数据结构定义，即 DDL 语句支持。
        /// </summary>
        DataDefinition = 0x00000002,

        /// <summary>
        /// 表级继承（仅 PostgreSQL 支持）。
        /// </summary>
        TableInherit = 0x00000010,
        /// <summary>
        /// 临时表。
        /// </summary>
        TemporaryTable = 0x00000020,
        /// <summary>
        /// 表变量。
        /// </summary>
        TableVariable = 0x00000040,
        /// <summary>
        /// 表值函数。
        /// </summary>
        TableValuedFunction = 0x00000100,
        /// <summary>
        /// 窗口函数。
        /// </summary>
        WindowFunction = 0x00000200,

        /// <summary>
        /// 外部复合语句，在非BEGIN - END 语句块声明复合语句。
        /// </summary>
        ExternalCompoundStatement = 0x00001000,
        /// <summary>
        /// 隐式声明变量。
        /// </summary>
        ImplicitDeclareVariable = 0x00002000,
        /// <summary>
        /// 外部局部变量。
        /// </summary>
        ExternalLocalVariable = 0x00004000,

        /// <summary>
        /// 修改数据后可直接返回数据。
        /// </summary>
        ModifyReturning = 0x00010000,
        /// <summary>
        /// 修改数据时可以连接多个其他表。
        /// </summary>
        ModifyJoin = 0x00020000,
        /// <summary>
        /// 批量插入语句（仅 MSSQL2005 不支持）。
        /// </summary>
        BatchInsert = 0x00040000,
        /// <summary>
        /// 子查询。
        /// </summary>
        SubQuery = 0x00080000,
    }
}