// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    /// <summary>
    /// 数据库操作类型。
    /// </summary>
    public enum EOperateType : int
    {
        /// <summary>
        /// 插入对象操作。
        /// </summary>
        InsertObjects = 0x0011,
        /// <summary>
        /// 按属性插入对象操作。
        /// </summary>
        InsertPropertys = 0x0012,
        /// <summary>
        /// 语句插入对象操作。
        /// </summary>
        InsertStatement = 0x0013,
        /// <summary>
        /// 更新对象操作。
        /// </summary>
        UpdateObjects = 0x0021,
        /// <summary>
        /// 按属性更新对象操作。
        /// </summary>
        UpdatePropertys = 0x0022,
        /// <summary>
        /// 语句更新对象操作。
        /// </summary>
        UpdateStatement = 0x0023,
        /// <summary>
        /// 删除对象操作。
        /// </summary>
        DeleteObjects = 0x0031,
        /// <summary>
        /// 语句删除操作。
        /// </summary>
        DeleteStatement = 0x0032,
        /// <summary>
        /// 添加关系操作。
        /// </summary>
        AddRelation = 0x0041,
        /// <summary>
        /// 删除关系操作。
        /// </summary>
        RemoveRelation = 0x0042,
        /// <summary>
        /// 查询集合操作。
        /// </summary>
        QueryCollection = 0x0051,
        /// <summary>
        /// 查询对象操作。
        /// </summary>
        QueryObject = 0x0052,

        /// <summary>
        /// 数据结构维护操作。
        /// </summary>
        Maintenance = 0x1000,

        /// <summary>
        /// 判断数据表是否存在操作。
        /// </summary>
        TableIsExsit = 0x0061,
        /// <summary>
        /// 创建表操作。
        /// </summary>
        CreateTable = 0x1062,
        /// <summary>
        /// 重命名表操作。
        /// </summary>
        RenameTable = 0x1063,
        /// <summary>
        /// 删除表操作。
        /// </summary>
        DropTable = 0x1064,
        /// <summary>
        /// 创建临时表操作。
        /// </summary>
        CreateTempTable = 0x0065,
        /// <summary>
        /// 创建表变量操作。
        /// </summary>
        CreateTableVariable = 0x0066,
        /// <summary>
        /// 判断视图是否存在操作。
        /// </summary>
        ViewIsExsit = 0x0071,
        /// <summary>
        /// 创建视图操作。
        /// </summary>
        CreateView = 0x1072,
        /// <summary>
        /// 重命名视图操作。
        /// </summary>
        RenameView = 0x1073,
        /// <summary>
        /// 删除视图操作。
        /// </summary>
        DropView = 0x1074,
        /// <summary>
        /// 判断关系是否存在操作。
        /// </summary>
        RelationIsExsit = 0x0081,
        /// <summary>
        /// 创建关系操作。
        /// </summary>
        CreateRelation = 0x1082,
        /// <summary>
        /// 删除关系操作。
        /// </summary>
        DropRelation = 0x1084,
        /// <summary>
        /// 判断关系是否存在操作。
        /// </summary>
        IndexIsExsit = 0x0091,
        /// <summary>
        /// 创建索引操作。
        /// </summary>
        CreateIndex = 0x1092,
        /// <summary>
        /// 重命名索引操作。
        /// </summary>
        RenameIndex = 0x1093,
        /// <summary>
        /// 删除索引操作。
        /// </summary>
        DropIndex = 0x1094,
    }
}