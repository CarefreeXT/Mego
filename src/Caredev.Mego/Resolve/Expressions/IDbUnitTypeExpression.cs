// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    /// <summary>
    /// 表示一个单元类型表达式的接口（本系统统一将集合类型表达式称为单元）。
    /// </summary>
    public interface IDbUnitTypeExpression : IDbExpression
    {
        /// <summary>
        /// 数据项CLR类型。
        /// </summary>
        Type ClrType { get; }
        /// <summary>
        /// 当前集合中元素所对应的项表达式。
        /// </summary>
        DbUnitItemTypeExpression Item { get; }
    }
}