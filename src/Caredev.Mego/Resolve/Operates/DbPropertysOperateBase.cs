// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using Caredev.Mego.Common;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;
    /// <summary>
    /// 指定属性的操作基类。
    /// </summary>
    public abstract class DbPropertysOperateBase : DbObjectsOperateBase
    {
        /// <summary>
        /// 创建指定属性的操作。
        /// </summary>
        /// <param name="target">操作目标。</param>
        /// <param name="expression">操作表达式。</param>
        /// <param name="items">提交数据集合。</param>
        /// <param name="type">数据项类型。</param>
        public DbPropertysOperateBase(IDbSet target, Expression expression, IEnumerable items, Type type)
            : base(target, items, type, CreateExpression((IQueryable)target, type, expression))
        {
        }
        private static Expression CreateExpression(IQueryable source, Type type, Expression expression)
        {
            return System.Linq.Expressions.Expression.Call(
                SupportMembers.Queryable.Select.MakeGenericMethod(type, type),
                Expression.Constant(source),
                expression);
        }
    }
}