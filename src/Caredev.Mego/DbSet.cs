// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Caredev.Mego.Resolve;

    /// <summary>
    /// 数据集对象，该对象用于映射到数据库表或视图对象。
    /// </summary>
    /// <typeparam name="T">数据对象类型。</typeparam>
    public class DbSet<T> : IOrderedQueryable<T>, IDbSet
        where T : class
    {
        /// <summary>
        /// 初始化<see cref="DbSet{T}"/>实例对象。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="name">自定义名称。</param>
        public DbSet(DbContext context, DbName name = null)
        {
            _Context = context;
            Name = name;
        }
        /// <summary>
        /// 数据执行上下文。
        /// </summary>
        public DbContext Context => _Context;
        private readonly DbContext _Context;
        /// <summary>
        /// 数据库名称对象，如果为空则表示使用默认名称。
        /// </summary>
        public DbName Name { get; }
        /// <summary>
        /// 数据项类型。
        /// </summary>
        public Type ClrType => typeof(T);
        /// <summary>
        /// <see cref="IQueryable"/>接口实现。
        /// </summary>
        public Expression Expression => Query.Expression;
        /// <summary>
        /// <see cref="IQueryable"/>接口实现。
        /// </summary>
        public Type ElementType => Query.ElementType;
        /// <summary>
        /// <see cref="IQueryable"/>接口实现。
        /// </summary>
        public IQueryProvider Provider => Query.Provider;
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Query).GetEnumerator();
        }
        /// <summary>
        /// <see cref="IEnumerable"/>接口实现。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Query.GetEnumerator();
        }
        private DbQuery<T> Query
        {
            get
            {
                if (_Query == null)
                {
                    _Query = new DbQuery<T>(Context, Expression.Constant(this));
                }
                return _Query;
            }
        }
        private DbQuery<T> _Query;
    }
}