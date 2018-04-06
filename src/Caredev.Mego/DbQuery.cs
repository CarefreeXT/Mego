// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;
    /// <summary>
    /// 数据查询对象。
    /// </summary>
    public class DbQuery : IQueryable, IContextContent
    {
        /// <summary>
        /// 初始化<see cref="DbQuery"/>实例对象。
        /// </summary>
        /// <param name="context">数据执行上下文。</param>
        /// <param name="expression">LINQ表达式。</param>
        internal DbQuery(DbContext context, Expression expression)
            : this(context, expression, expression.Type)
        { }
        /// <summary>
        /// 初始化<see cref="DbQuery"/>实例对象。
        /// </summary>
        /// <param name="context">数据执行上下文。</param>
        /// <param name="expression">LINQ表达式。</param>
        /// <param name="elementType">查询元素类型。</param>
        internal DbQuery(DbContext context, Expression expression, Type elementType)
        {
            _Expression = expression;
            _ElementType = elementType;
            Context = context;
        }
        /// <summary>
        /// <see cref="IQueryable"/>接口实现。
        /// </summary>
        public Expression Expression => _Expression;
        private readonly Expression _Expression;
        /// <summary>
        /// <see cref="IQueryable"/>接口实现。
        /// </summary>
        public Type ElementType => _ElementType;
        private readonly Type _ElementType;
        /// <summary>
        /// <see cref="IQueryable"/>接口实现。
        /// </summary>
        public IQueryProvider Provider => _Provider ?? (_Provider = new DbQueryProvider(Context));
        private IQueryProvider _Provider;
        /// <summary>
        /// 数据执行上下文。
        /// </summary>
        public DbContext Context { get; }
        /// <summary>
        /// 当前数据查询操作对象。
        /// </summary>
        internal DbQueryCollectionOperate Operate
        {
            get
            {
                lock (this)
                {
                    if (_Operate == null)
                    {
                        _Operate = new DbQueryCollectionOperate(Context, Expression, ElementType);
                        Context.Executor.Execute(_Operate);
                    }
                    return _Operate;
                }
            }
        }
        private DbQueryCollectionOperate _Operate;
        /// <summary>
        /// <see cref="IEnumerable"/>接口实现。
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return Operate.Result.GetEnumerator();
        }
    }
}
