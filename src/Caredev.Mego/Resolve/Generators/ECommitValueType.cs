// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    /// <summary>
    /// 值类型。
    /// </summary>
    public enum ECommitValueType
    {
        /// <summary>
        /// 常量。
        /// </summary>
        Constant = 0,
        /// <summary>
        /// 数据库生成。
        /// </summary>
        Database = 1,
        /// <summary>
        /// 表达式创建。
        /// </summary>
        Expression = 2
    }
}