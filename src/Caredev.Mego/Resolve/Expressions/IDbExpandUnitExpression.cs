// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Collections.Generic;
    /// <summary>
    /// 可展开的数据单元表达式，实现该接口的数据单元可以显示声明获取复
    /// 杂对象或集合属性，常用于数据查核的 Include 操作。
    /// </summary>
    public interface IDbExpandUnitExpression : IDbUnitTypeExpression
    {
        /// <summary>
        /// 需要展开属性表达式集合。
        /// </summary>
        IList<DbExpression> ExpandItems { get; }
    }
}