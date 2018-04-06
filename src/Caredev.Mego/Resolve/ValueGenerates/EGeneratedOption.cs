// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.ValueGenerates
{
    /// <summary>
    /// 生成选项。
    /// </summary>
    public enum EGeneratedOption : int
    {
        /// <summary>
        /// 忽略生成。
        /// </summary>
        Ignore = 0,
        /// <summary>
        /// 使用数据库的标识体系生成值，只有在添加数据时才能使用。
        /// </summary>
        Identity = 1,
        /// <summary>
        /// 由数据库生成值，例如默认值或触发器等数据库行为。
        /// </summary>
        Database = 2,
        /// <summary>
        /// 使用数据库表达式生成值，在数据生成时执行返回标量值的语句表达式来产生相应的值。
        /// </summary>
        Expression = 3,
        /// <summary>
        /// 在内存中生成值。
        /// </summary>
        Memory = 4
    }
}