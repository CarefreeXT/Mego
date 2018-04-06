// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    /// <summary>
    /// 表示单元表达式中的的单个元素项（简称单元项）。
    /// </summary>
    public abstract class DbUnitItemTypeExpression : DbExpression, IDbUnitItemTypeExpression
    {
        /// <summary>
        /// 创建单元项表达式。
        /// </summary>
        /// <param name="type">数据项的CLR类型。</param>
        internal DbUnitItemTypeExpression(Type type)
        {
            ClrType = type;
        }
        /// <summary>
        /// 数据项的CLR类型。
        /// </summary>
        public Type ClrType { get; private set; }
        /// <summary>
        /// 项的单元表达式。
        /// </summary>
        public IDbUnitTypeExpression Unit { get; internal set; }
    }
}