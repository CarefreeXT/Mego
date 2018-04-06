// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Operates;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    /// <summary>
    /// LINQ 查询提供程序。
    /// </summary>
    internal sealed class DbQueryProvider : IQueryProvider
    {
        /// <summary>
        /// 创建 LINQ 查询提供程序。
        /// </summary>
        /// <param name="context">数据上下文对象。</param>
        public DbQueryProvider(DbContext context)
        {
            Utility.NotNull(context, nameof(context));

            Context = context;
        }
        /// <summary>
        /// 当前数据库上下文。
        /// </summary>
        public DbContext Context { get; }
        /// <summary>
        /// 构造一个 IQueryable 对象，该对象可计算指定表达式目录树所表示的查询。
        /// </summary>
        /// <param name="expression">表示 LINQ 查询的表达式目录树。</param>
        /// <returns>返回<see cref="IQueryable"/>，它可计算指定表达式目录树所表示的查询。</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            return new DbQuery(Context, expression);
        }
        /// <summary>
        /// 构造一个<see cref="IQueryable{T}"/>对象，该对象可计算指定表达式目录树所表示的查询。
        /// </summary>
        /// <typeparam name="TElement">查询元素类型。</typeparam>
        /// <param name="expression">查询表达式。</param>
        /// <returns>返回<see cref="IQueryable{T}"/>对象。</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DbQuery<TElement>(Context, expression);
        }
        /// <summary>
        /// 执行指定表达式目录树所表示的查询。
        /// </summary>
        /// <param name="expression">查询表达式。</param>
        /// <returns>执行结果。</returns>
        public object Execute(Expression expression)
        {
            if (_Operates == null)
            {
                _Operates = new Dictionary<string, DbQueryObjectOperate>();
            }
            var expressionString = expression.ToString();
            if (!_Operates.TryGetValue(expressionString, out DbQueryObjectOperate operate))
            {
                var context = Context;
                operate = new DbQueryObjectOperate(context, expression, expression.Type);
                context.Executor.Execute(operate);
                _Operates.Add(expressionString, operate);
            }
            return operate.Result;
        }
        private Dictionary<string, DbQueryObjectOperate> _Operates;
        /// <summary>
        /// 执行指定表达式目录树所表示的强类型查询。
        /// </summary>
        /// <typeparam name="TResult">查询元素类型。</typeparam>
        /// <param name="expression">执行表达式。</param>
        /// <returns>查询结果对象。</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            var result = Execute(expression);
            if (result == null || result.GetType() == typeof(TResult))
                return (TResult)result;
            else
                return (TResult)System.Convert.ChangeType(result, typeof(TResult));
        }
    }
}