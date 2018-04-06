// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    /// <summary>
    /// 泛型数据查询对象。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DbQuery<T> : DbQuery, IOrderedQueryable<T>
    {
        /// <summary>
        /// 初始化<see cref="DbQuery{T}"/>实例对象。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expression"></param>
        public DbQuery(DbContext context, Expression expression)
            : base(context, expression, typeof(T))
        {
        }
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现。
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Operate.Result.OfType<T>().GetEnumerator();
        }
    }
}