// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    /// <summary>
    /// 数据库对象枚举。
    /// </summary>
    public enum EDatabaseObject
    {
        /// <summary>
        /// 数据库。
        /// </summary>
        Database,
        /// <summary>
        /// 表。
        /// </summary>
        Table,
        /// <summary>
        /// 视图。
        /// </summary>
        View,
        /// <summary>
        /// 序列。
        /// </summary>
        Sequence,
        /// <summary>
        /// 检查约束。
        /// </summary>
        CheckConstraint,
        /// <summary>
        /// 外键约束。
        /// </summary>
        ForeignKey,
        /// <summary>
        /// 主键约束。
        /// </summary>
        PrimaryKey,
        /// <summary>
        /// 默认值约束。
        /// </summary>
        DefaultConstraint,
        /// <summary>
        /// 标量函数。
        /// </summary>
        ScalarFunction,
        /// <summary>
        /// 表值函数。
        /// </summary>
        TableValuedFunction,
        /// <summary>
        /// 存储过程。
        /// </summary>
        StoredProcedure
    }
}