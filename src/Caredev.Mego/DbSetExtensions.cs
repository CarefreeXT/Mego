// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    /// <summary>
    /// 用于<see cref="DbSet{T}"/>创建数据提交操作的方法
    /// </summary>
    public static partial class DbSetExtensions
    {

        private static DbObjectsOperateBase GetInsertOperate<T>(object item, DbOperateContext executor) where T : class
        {
            var list = executor.Find(typeof(T));
            if (list != null)
            {
                return list.Where(a => a.Type == EOperateType.InsertObjects || a.Type == EOperateType.InsertPropertys)
                    .OfType<DbObjectsOperateBase>().FirstOrDefault(a => a.Contains(item));
            }
            return null;
        }

        private static TOperate GetOrCreate<TSource, TOperate>(DbSet<TSource> dataSet, IEnumerable<TSource> dataItems
            , Func<DbContext, TOperate> create, Func<TOperate, bool> predicate = null)
            where TSource : class
            where TOperate : DbObjectsOperateBase
        {
            var context = dataSet.Context;
            var executor = context.Executor;
            lock (executor)
            {
                var content = executor.Find<TOperate>(typeof(TSource), predicate);
                if (content == null)
                {
                    content = create(context);
                    executor.Add(content);
                    return content;
                }
#if NET35
                content.Add(dataItems.OfType<object>());
#else
                content.Add(dataItems);
#endif
                return content;
            }
        }

        private static TOperate GetOrCreate<TSource, TOperate>(DbSet<TSource> dataSet, PropertyInfo propertyInfo
            , DbRelationItem item, Func<DbContext, PropertyInfo, TOperate> create)
            where TSource : class
            where TOperate : DbRelationOperateBase
        {
            var context = dataSet.Context;
            var executor = context.Executor;
            lock (executor)
            {
                var content = executor.Find<TOperate>(typeof(TSource), operate => operate.Member == propertyInfo);
                if (content == null)
                {
                    content = create(context, propertyInfo);
                    executor.Add(content);
                }
                content.Add(item);
                return content;
            }
        }

        private static DbRelationOperateBase AddRelation<TSource, TTarget>(this DbSet<TSource> dataSet, TSource source, LambdaExpression expression, TTarget target)
            where TSource : class
            where TTarget : class
        {
            var propertyInfo = expression.GetMember();
            var configure = dataSet.Context.Configuration;
            var table = configure.Metadata.Table(typeof(TSource));
            var navigate = table.Navigates[propertyInfo];
            DbObjectsOperateBase dependentInsert = null;
            DbObjectsOperateBase dependentInsert2 = null;
            var executor = dataSet.Context.Executor;
            if (!navigate.IsComposite)
            {
                DbObjectsOperateBase currentInsert = null;
                if (navigate.IsForeign == true)
                {
                    currentInsert = GetInsertOperate<TSource>(source, executor);
                    dependentInsert = GetInsertOperate<TTarget>(target, executor);
                }
                else
                {
                    currentInsert = GetInsertOperate<TTarget>(target, executor);
                    dependentInsert = GetInsertOperate<TSource>(source, executor);
                }
                if (currentInsert != null)
                {
                    var reference = (IInsertReferenceRelation)currentInsert;
                    var relation = reference.Relations.FirstOrDefault(a => a.Navigate == navigate);
                    if (relation == null)
                    {
                        relation = new DbAddRelationOperate<TSource>(dataSet.Context, propertyInfo, typeof(TSource), table, navigate);
                        reference.Relations.Add(relation);
                    }
                    relation.Add(source, target);
                    if (dependentInsert != null)
                    {
                        currentInsert.Dependents.Add(dependentInsert);
                    }
                    return relation;
                }
            }
            else
            {
                dependentInsert = GetInsertOperate<TSource>(source, executor);
                dependentInsert2 = GetInsertOperate<TTarget>(target, executor);
            }
            var result = GetOrCreate(dataSet, propertyInfo, new DbRelationItem(source, target)
                , (context, property) => new DbAddRelationOperate<TSource>(context, propertyInfo, typeof(TSource), table, navigate));
            if (dependentInsert != null)
            {
                result.Dependents.Add(dependentInsert);
            }
            if (dependentInsert2 != null)
            {
                result.Dependents.Add(dependentInsert2);
            }
            return result;
        }

        private static DbRelationOperateBase RemoveRelation<TSource, TTarget>(this DbSet<TSource> dataSet, TSource source, LambdaExpression property, TTarget target)
            where TSource : class
            where TTarget : class
            => GetOrCreate(dataSet, property.GetMember(), new DbRelationItem(source, target)
                , (context, propertyInfo) => new DbRemoveRelationOperate<TSource>(context, propertyInfo));
    }
    partial class DbSetExtensions
    {
        /// <summary>
        /// 添加多个数据对象。
        /// </summary>
        /// <typeparam name="TSource">目标数据类型。</typeparam>
        /// <param name="dataSet">数据集对象。</param>
        /// <param name="dataItems">添加的对象集合。</param>
        /// <returns></returns>
        public static DbInsertObjectsOperate<TSource> AddRange<TSource>(this DbSet<TSource> dataSet, IEnumerable<TSource> dataItems)
            where TSource : class
            => GetOrCreate(dataSet, dataItems, context => new DbInsertObjectsOperate<TSource>(dataSet, dataItems));
        /// <summary>
        /// 以指定有限的属性的方式添加多个数据对象
        /// </summary>
        /// <typeparam name="TSource">目标数据类型。</typeparam>
        /// <param name="dataSet">数据集对象。</param>
        /// <param name="expression">指定属性的表达式。</param>
        /// <param name="dataItems">添加的对象集合。</param>
        /// <returns></returns>
        public static DbInsertPropertysOperate<TSource> AddRange<TSource>(this DbSet<TSource> dataSet, Expression<Func<TSource, TSource>> expression, IEnumerable<TSource> dataItems)
            where TSource : class
            => GetOrCreate(dataSet, dataItems
                , context => new DbInsertPropertysOperate<TSource>(dataSet, expression, dataItems)
                , operate => operate.Expression.ToString() == expression.ToString());
        /// <summary>
        /// 更新多个数据对象。
        /// </summary>
        /// <typeparam name="TSource">目标数据类型。</typeparam>
        /// <param name="dataSet">数据集对象。</param>
        /// <param name="dataItems">更新的对象集合。</param>
        /// <returns></returns>
        public static DbUpdateObjectsOperate<TSource> UpdateRange<TSource>(this DbSet<TSource> dataSet, IEnumerable<TSource> dataItems)
            where TSource : class
            => GetOrCreate(dataSet, dataItems, context => new DbUpdateObjectsOperate<TSource>(dataSet, dataItems));
        /// <summary>
        /// 以指定有限的属性的方式更新多个数据对象
        /// </summary>
        /// <typeparam name="TSource">目标数据类型。</typeparam>
        /// <param name="dataSet">数据集对象。</param>
        /// <param name="expression">指定属性的表达式。</param>
        /// <param name="dataItems">更新的对象集合。</param>
        /// <returns></returns>
        public static DbUpdatePropertysOperate<TSource> UpdateRange<TSource>(this DbSet<TSource> dataSet, Expression<Func<TSource, TSource>> expression, IEnumerable<TSource> dataItems)
            where TSource : class
            => GetOrCreate(dataSet, dataItems
                , context => new DbUpdatePropertysOperate<TSource>(dataSet, expression, dataItems)
                , operate => operate.Expression.ToString() == expression.ToString());
        /// <summary>
        /// 更新多个数据对象。
        /// </summary>
        /// <typeparam name="TSource">目标数据类型。</typeparam>
        /// <param name="dataSet">数据集对象。</param>
        /// <param name="dataItems">删除的对象集合。</param>
        /// <returns></returns>
        public static DbDeleteObjectsOperate<TSource> RemoveRange<TSource>(this DbSet<TSource> dataSet, IEnumerable<TSource> dataItems)
            where TSource : class
            => GetOrCreate(dataSet, dataItems, context => new DbDeleteObjectsOperate<TSource>(dataSet, dataItems));
        /// <summary>
        /// 添加对象关系。
        /// </summary>
        /// <typeparam name="TSource">源对象类型。</typeparam>
        /// <typeparam name="TTarget">目标对象类型。</typeparam>
        /// <param name="dataSet">源数据集。</param>
        /// <param name="source">源对象。</param>
        /// <param name="property">添加的属性CLR描述对象。</param>
        /// <param name="target">目标对象。</param>
        /// <returns>关系操作。</returns>
        public static DbRelationOperateBase AddRelation<TSource, TTarget>(this DbSet<TSource> dataSet, TSource source, Expression<Func<TSource, TTarget>> property, TTarget target)
            where TSource : class
            where TTarget : class
        {
            return AddRelation<TSource, TTarget>(dataSet, source, (LambdaExpression)property, target);
        }
        /// <summary>
        /// 添加集合关系。
        /// </summary>
        /// <typeparam name="TSource">源对象类型。</typeparam>
        /// <typeparam name="TTarget">目标对象类型。</typeparam>
        /// <param name="dataSet">源数据集。</param>
        /// <param name="source">源对象。</param>
        /// <param name="property">添加的属性CLR描述对象。</param>
        /// <param name="target">目标对象。</param>
        /// <returns>关系操作。</returns>
        public static DbRelationOperateBase AddRelation<TSource, TTarget>(this DbSet<TSource> dataSet, TSource source, Expression<Func<TSource, IEnumerable<TTarget>>> property, TTarget target)
            where TSource : class
            where TTarget : class
        {
            return AddRelation<TSource, TTarget>(dataSet, source, (LambdaExpression)property, target);
        }
        /// <summary>
        /// 删除对象关系。
        /// </summary>
        /// <typeparam name="TSource">源对象类型。</typeparam>
        /// <typeparam name="TTarget">目标对象类型。</typeparam>
        /// <param name="dataSet">源数据集。</param>
        /// <param name="source">源对象。</param>
        /// <param name="property">删除的属性CLR描述对象。</param>
        /// <param name="target">目标对象。</param>
        /// <returns>关系操作。</returns>
        public static DbRelationOperateBase RemoveRelation<TSource, TTarget>(this DbSet<TSource> dataSet, TSource source, Expression<Func<TSource, TTarget>> property, TTarget target)
            where TSource : class
            where TTarget : class
        {
            return RemoveRelation(dataSet, source, (LambdaExpression)property, target);
        }
        /// <summary>
        /// 删除集合关系。
        /// </summary>
        /// <typeparam name="TSource">源对象类型。</typeparam>
        /// <typeparam name="TTarget">目标对象类型。</typeparam>
        /// <param name="dataSet">源数据集。</param>
        /// <param name="source">源对象。</param>
        /// <param name="property">删除的属性CLR描述对象。</param>
        /// <param name="target">目标对象。</param>
        /// <returns>关系操作。</returns>
        public static DbRelationOperateBase RemoveRelation<TSource, TTarget>(this DbSet<TSource> dataSet, TSource source, Expression<Func<TSource, IEnumerable<TTarget>>> property, TTarget target)
            where TSource : class
            where TTarget : class
        {
            return RemoveRelation(dataSet, source, (LambdaExpression)property, target);
        }
        /// <summary>
        /// 添加指定数据集的数据对象。
        /// </summary>
        /// <typeparam name="TSource">数据类型。</typeparam>
        /// <param name="dataSet">数据集对象。</param>
        /// <param name="dataItems">添加对象集合。</param>
        /// <returns>添加操作</returns>
        public static DbInsertObjectsOperate<TSource> Add<TSource>(this DbSet<TSource> dataSet, params TSource[] dataItems)
            where TSource : class
        {
            return AddRange(dataSet, dataItems);
        }
        /// <summary>
        /// 更新指定数据集的数据对象。
        /// </summary>
        /// <typeparam name="TSource">数据类型。</typeparam>
        /// <param name="dataSet">数据集对象。</param>
        /// <param name="dataItems">更新对象集合。</param>
        /// <returns>更新操作。</returns>
        public static DbUpdateObjectsOperate<TSource> Update<TSource>(this DbSet<TSource> dataSet, params TSource[] dataItems)
            where TSource : class
        {
            return UpdateRange(dataSet, dataItems);
        }
        /// <summary>
        /// 删除指定数据集的数据对象。
        /// </summary>
        /// <typeparam name="TSource">数据类型。</typeparam>
        /// <param name="dataSet">数据集对象。</param>
        /// <param name="dataItems">删除对象集合。</param>
        /// <returns>删除操作对象</returns>
        public static DbDeleteObjectsOperate<TSource> Remove<TSource>(this DbSet<TSource> dataSet, params TSource[] dataItems)
            where TSource : class
        {
            return RemoveRange(dataSet, dataItems);
        }
        /// <summary>
        /// 指定属性添加数据集中的数据对象。
        /// </summary>
        /// <typeparam name="TSource">数据类型。</typeparam>
        /// <param name="dataSet">指定数据集对象。</param>
        /// <param name="expression">添加数据表达式。</param>
        /// <param name="dataItems">添加的数据集合。</param>
        /// <returns>添加操作对象。</returns>
        public static DbInsertPropertysOperate<TSource> Add<TSource>(this DbSet<TSource> dataSet, Expression<Func<TSource, TSource>> expression, params TSource[] dataItems)
            where TSource : class
        {
            return AddRange<TSource>(dataSet, expression, dataItems);
        }
        /// <summary>
        /// 指定属性更新数据集中的数据对象。
        /// </summary>
        /// <typeparam name="TSource">数据类型。</typeparam>
        /// <param name="dataSet">指定数据集对象。</param>
        /// <param name="expression">更新数据表达式。</param>
        /// <param name="dataItems">更新的数据集合。</param>
        /// <returns>更新操作对象。</returns>
        public static DbUpdatePropertysOperate<TSource> Update<TSource>(this DbSet<TSource> dataSet, Expression<Func<TSource, TSource>> expression, params TSource[] dataItems)
            where TSource : class
        {
            return UpdateRange<TSource>(dataSet, expression, dataItems);
        }
        /// <summary>
        /// 根据指定查询表达式添加数据。
        /// </summary>
        /// <typeparam name="TSource">目标数据类型。</typeparam>
        /// <param name="dataSet">作用数据集对象。</param>
        /// <param name="query">执行表达式对象。</param>
        /// <returns>返回添加语句操作对象。</returns>
        public static DbStatementOperateBase Add<TSource>(this DbSet<TSource> dataSet, IQueryable<TSource> query)
          where TSource : class
        {
            return new DbInsertStatementOperate<TSource>(dataSet, query.Expression);
        }
        /// <summary>
        /// 根据指定查询表达式更新数据。
        /// </summary>
        /// <typeparam name="TSource">目标数据类型。</typeparam>
        /// <param name="dataSet">作用数据集对象。</param>
        /// <param name="query">执行表达式对象。</param>
        /// <returns>返回更新语句操作对象。</returns>
        public static DbStatementOperateBase Update<TSource>(this DbSet<TSource> dataSet, IQueryable<TSource> query)
            where TSource : class
        {
            return new DbUpdateStatementOperate<TSource>(dataSet, query.Expression);
        }
        /// <summary>
        /// 根据指定查询过滤表达式删除数据。
        /// </summary>
        /// <typeparam name="TSource">目标数据类型。</typeparam>
        /// <param name="dataSet">作用数据集对象。</param>
        /// <param name="expression">执行表达式对象。</param>
        /// <returns>返回删除语句操作对象。</returns>
        public static DbStatementOperateBase Remove<TSource>(this DbSet<TSource> dataSet, Expression<Func<TSource, bool>> expression)
           where TSource : class
        {
            return Remove(dataSet, dataSet.Where(expression));
        }
        /// <summary>
        /// 根据指定查询表达式删除数据。
        /// </summary>
        /// <typeparam name="TSource">目标数据类型。</typeparam>
        /// <param name="dataSet">作用数据集对象。</param>
        /// <param name="query">执行表达式对象。</param>
        /// <returns>返回删除语句操作对象。</returns>
        public static DbStatementOperateBase Remove<TSource>(this DbSet<TSource> dataSet, IQueryable<TSource> query)
            where TSource : class
        {
            return new DbDeleteStatementOperate<TSource>(dataSet, query.Expression);
        }
        /// <summary>
        /// 在当前查询中包含指定路径成员的数据。
        /// </summary>
        /// <typeparam name="TSource">查询数据成员类型。</typeparam>
        /// <param name="source">查询数据源对象。</param>
        /// <param name="path">包含的路径。</param>
        /// <returns>带有包含路径定义的查询对象。</returns>
        public static IQueryable<TSource> Include<TSource>(this IQueryable<TSource> source, string path)
        {
            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    SupportMembers.Queryable.Include1.MakeGenericMethod(typeof(TSource)),
                    source.Expression,
                    Expression.Constant(path)
                    ));
        }
        /// <summary>
        /// 在当前查询中包含指定表达式路径成员的数据。
        /// </summary>
        /// <typeparam name="TSource">查询数据成员类型。</typeparam>
        /// <typeparam name="TTarget">查询目标成员类型。</typeparam>
        /// <param name="source">查询数据源对象。</param>
        /// <param name="exp">路径表达式。</param>
        /// <returns>带有包含路径定义的查询对象。</returns>
        public static IQueryable<TSource> Include<TSource, TTarget>(this IQueryable<TSource> source, Expression<Func<TSource, TTarget>> exp)
        {
            return source.Provider.CreateQuery<TSource>(
               Expression.Call(
                   SupportMembers.Queryable.Include2.MakeGenericMethod(typeof(TSource), typeof(TTarget)),
                   source.Expression,
                   exp
                   ));
        }
        /// <summary>
        /// 在当前查询中包含指定路径成员的数据。
        /// </summary>
        /// <typeparam name="TSource">查询数据成员类型。</typeparam>
        /// <param name="source">查询数据源对象。</param>
        /// <param name="path">包含的路径。</param>
        /// <returns>带有包含路径定义的查询对象。</returns>
        public static IEnumerable<TSource> Include<TSource>(this IEnumerable<TSource> source, string path)
        {
            return source;
        }
        /// <summary>
        /// 在当前查询中包含指定表达式路径成员的数据。
        /// </summary>
        /// <typeparam name="TSource">查询数据成员类型。</typeparam>
        /// <typeparam name="TTarget">查询目标成员类型。</typeparam>
        /// <param name="source">查询数据源对象。</param>
        /// <param name="exp">路径表达式。</param>
        /// <returns>带有包含路径定义的查询对象。</returns>
        public static IEnumerable<TSource> Include<TSource, TTarget>(this IEnumerable<TSource> source, Expression<Func<TSource, TTarget>> exp)
        {
            return source;
        }
        /// <summary>
        /// 使用指定名称对象，创建数据集。
        /// </summary>
        /// <typeparam name="T">数据对象类型。</typeparam>
        /// <param name="context">数据上下文。</param>
        /// <param name="name">名称对象。</param>
        /// <returns>数据集。</returns>
        public static DbSet<T> Set<T>(this DbContext context, DbName name) where T : class
        {
            return new DbSet<T>(context, name);
        }
        /// <summary>
        /// 使用指定名称对象，创建匿名对象数据集。
        /// </summary>
        /// <typeparam name="T">数据对象类型。</typeparam>
        /// <param name="context">数据上下文。</param>
        /// <param name="item">匿名对象，该对象仅做类型推导用。</param>
        /// <param name="name">名称对象。</param>
        /// <returns>数据集。</returns>
        public static DbSet<T> Set<T>(this DbContext context, T item, DbName name) where T : class
        {
            return new DbSet<T>(context, name);
        }
        /// <summary>
        /// 使用指定名称对象，创建匿名对象数据集。
        /// </summary>
        /// <typeparam name="T">数据对象类型。</typeparam>
        /// <param name="context">数据上下文。</param>
        /// <param name="items">匿名对象集合，该对象仅做类型推导用。</param>
        /// <param name="name">名称对象。</param>
        /// <returns>数据集。</returns>
        public static DbSet<T> Set<T>(this DbContext context, IEnumerable<T> items, DbName name) where T : class
        {
            return new DbSet<T>(context, name);
        }
    }
}