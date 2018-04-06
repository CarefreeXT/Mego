// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Fragments
{
    /// <summary>
    /// 关系数据库对象连接类型。
    /// </summary>
    public enum EJoinType
    {
        /// <summary>
        /// 交叉连接。
        /// </summary>
        CrossJoin,
        /// <summary>
        /// 内连接。
        /// </summary>
        InnerJoin,
        /// <summary>
        /// 左外连接。
        /// </summary>
        LeftJoin,
        /// <summary>
        /// 右外连接。
        /// </summary>
        RightJoin,
        /// <summary>
        /// 全外连接。
        /// </summary>
        FullJoin,
    }
}