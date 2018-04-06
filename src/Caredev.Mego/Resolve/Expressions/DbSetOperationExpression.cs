// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    /// <summary>
    /// 单元操作表达式基类。
    /// </summary>
    public abstract class DbSetOperationExpression : DbUnitTypeExpression
    {
        /// <summary>
        /// 初始化数据集操作。
        /// </summary>
        /// <param name="type">数据集的元素CLR类型。</param>
        /// <param name="source">操作的单元表达式。</param>
        /// <param name="itemType">单元项表达式。</param>
        public DbSetOperationExpression(Type type, DbUnitTypeExpression source, DbUnitItemTypeExpression itemType)
            : base(type, itemType)
        {
            Source = source;
        }
        /// <summary>
        /// 操作的目标数据单元。
        /// </summary>
        public DbUnitTypeExpression Source { get; private set; }
    }
}