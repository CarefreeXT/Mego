// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 该接口声明当前表达式可用于数据连接操作，若连接的数据为空则用该默认值替换空值。
    /// </summary>
    public interface IDbDefaultUnitType : IDbExpression
    {
        /// <summary>
        /// 在数据连接操作中数据为空时的默认值。
        /// </summary>
        DbExpression Default { get; set; }
    }
}
