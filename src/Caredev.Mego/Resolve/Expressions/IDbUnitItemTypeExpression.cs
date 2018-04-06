// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    /// <summary>
    /// 该接口表示集合查询表达式中的单个元素。
    /// </summary>
    public interface IDbUnitItemTypeExpression : IDbExpression
    {
        /// <summary>
        /// 对应元素的CLR类型。
        /// </summary>
        Type ClrType { get; }
        /// <summary>
        /// 当前所对应的集合查询表达式。
        /// </summary>
        IDbUnitTypeExpression Unit { get; }
    }
}