// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    /// <summary>
    /// 数据库操作类型。
    /// </summary>
    public enum EOperateType
    {
        /// <summary>
        /// 插入对象操作。
        /// </summary>
        InsertObjects,
        /// <summary>
        /// 按属性插入对象操作。
        /// </summary>
        InsertPropertys,
        /// <summary>
        /// 语句插入对象操作。
        /// </summary>
        InsertStatement,
        /// <summary>
        /// 更新对象操作。
        /// </summary>
        UpdateObjects,
        /// <summary>
        /// 按属性更新对象操作。
        /// </summary>
        UpdatePropertys,
        /// <summary>
        /// 语句更新对象操作。
        /// </summary>
        UpdateStatement,
        /// <summary>
        /// 删除对象操作。
        /// </summary>
        DeleteObjects,
        /// <summary>
        /// 语句删除操作。
        /// </summary>
        DeleteStatement,
        /// <summary>
        /// 添加关系操作。
        /// </summary>
        AddRelation,
        /// <summary>
        /// 删除关系操作。
        /// </summary>
        RemoveRelation,
        /// <summary>
        /// 查询集合操作。
        /// </summary>
        QueryCollection,
        /// <summary>
        /// 查询对象操作。
        /// </summary>
        QueryObject,
        /// <summary>
        /// 数据库对象存在判断操作。
        /// </summary>
        ObjectExsit,
        /// <summary>
        /// 创建表操作。
        /// </summary>
        CreateTable,
        /// <summary>
        /// 删除表操作。
        /// </summary>
        DropTable,
        /// <summary>
        /// 创建关系操作。
        /// </summary>
        CreateRelation,
        /// <summary>
        /// 删除关系操作。
        /// </summary>
        DropRelation,
        /// <summary>
        /// 创建视图操作。
        /// </summary>
        CreateView,
        /// <summary>
        /// 删除视图操作。
        /// </summary>
        DropView,
        /// <summary>
        /// 创建索引操作。
        /// </summary>
        CreateIndex,
        /// <summary>
        /// 删除索引操作。
        /// </summary>
        DropIndex,
    }
}