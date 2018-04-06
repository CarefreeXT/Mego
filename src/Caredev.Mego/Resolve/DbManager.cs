// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using linq = System.Linq.Expressions;
    using Res = Properties.Resources;
    /// <summary>
    /// 数据库管理对象。
    /// </summary>
    public sealed class DbManager
    {
        private readonly MetadataEngine _Metadata;
        private readonly DbContext _Context;
        /// <summary>
        /// 创建数据库管理对象。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="metadata">元数据引擎。</param>
        public DbManager(DbContext context, MetadataEngine metadata)
        {
            _Context = context;
            _Metadata = metadata;
        }
        /// <summary>
        /// 判断是否存在。
        /// </summary>
        /// <typeparam name="T">指定对象类型。</typeparam>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase Exsit<T>() => Exsit(typeof(T));
        /// <summary>
        /// 判断是否存在。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase Exsit(Type type)
        {
            var table = _Metadata.Table(type);
            return new DbObjectExsitOperate(_Context, type, EDatabaseObject.Table)
            {
                Name = table.Name,
                Schema = table.Schema
            };
        }
        /// <summary>
        /// 创建数据表。
        /// </summary>
        /// <typeparam name="T">指定对象类型。</typeparam>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateTable<T>() => CreateTable(typeof(T));
        /// <summary>
        /// 创建数据表。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateTable(Type type)
        {
            return new DbCreateTableOperate(_Context, type);
        }
        /// <summary>
        /// 删除数据表。
        /// </summary>
        /// <typeparam name="T">指定对象类型。</typeparam>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase DropTable<T>() => DropTable(typeof(T));
        /// <summary>
        /// 删除数据表。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase DropTable(Type type)
        {
            return new DbDropTableOperate(_Context, type);
        }
        /// <summary>
        /// 创建对象关系。
        /// </summary>
        /// <typeparam name="TSource">关系源类型。</typeparam>
        /// <typeparam name="TTarget">关系目标类型。</typeparam>
        /// <param name="expressions">成员表达式。</param>
        /// <returns>操作实例。</returns>
        public IEnumerable<DbMaintenanceOperateBase> CreateRelation<TSource, TTarget>(linq.Expression<Func<TSource, TTarget>> expressions)
        {
            var property = ((System.Linq.Expressions.LambdaExpression)expressions).GetMember();
            return CreateDropRelation(typeof(TSource), property, EOperateType.CreateRelation);
        }
        /// <summary>
        /// 创建集合关系。
        /// </summary>
        /// <typeparam name="TSource">关系源类型。</typeparam>
        /// <typeparam name="TTarget">关系目标类型。</typeparam>
        /// <param name="expressions">成员表达式。</param>
        /// <returns>操作实例。</returns>
        public IEnumerable<DbMaintenanceOperateBase> CreateRelation<TSource, TTarget>(linq.Expression<Func<TSource, IEnumerable<TTarget>>> expressions)
        {
            var property = ((System.Linq.Expressions.LambdaExpression)expressions).GetMember();
            return CreateDropRelation(typeof(TSource), property, EOperateType.CreateRelation);
        }
        /// <summary>
        /// 删除对象关系。
        /// </summary>
        /// <typeparam name="TSource">关系源类型。</typeparam>
        /// <typeparam name="TTarget">关系目标类型。</typeparam>
        /// <param name="expressions">成员表达式。</param>
        /// <returns>操作实例。</returns>
        public IEnumerable<DbMaintenanceOperateBase> DropRelation<TSource, TTarget>(linq.Expression<Func<TSource, TTarget>> expressions)
        {
            var property = ((System.Linq.Expressions.LambdaExpression)expressions).GetMember();
            return CreateDropRelation(typeof(TSource), property, EOperateType.DropRelation);
        }
        /// <summary>
        /// 删除集合关系。
        /// </summary>
        /// <typeparam name="TSource">关系源类型。</typeparam>
        /// <typeparam name="TTarget">关系目标类型。</typeparam>
        /// <param name="expressions">成员表达式。</param>
        /// <returns>操作实例。</returns>
        public IEnumerable<DbMaintenanceOperateBase> DropRelation<TSource, TTarget>(linq.Expression<Func<TSource, IEnumerable<TTarget>>> expressions)
        {
            var property = ((System.Linq.Expressions.LambdaExpression)expressions).GetMember();
            return CreateDropRelation(typeof(TSource), property, EOperateType.DropRelation);
        }
        /// <summary>
        /// 创建继承表关系。
        /// </summary>
        /// <typeparam name="TSource">相应对象类型。</typeparam>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateRelation<TSource>() => CreateDropKeyRelation<TSource>(EOperateType.CreateRelation);
        /// <summary>
        /// 删除继承表关系。
        /// </summary>
        /// <typeparam name="TSource">相应对象类型。</typeparam>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase DropRelation<TSource>() => CreateDropKeyRelation<TSource>(EOperateType.DropRelation);
        //创建删除继承表关系。
        private DbMaintenanceOperateBase CreateDropKeyRelation<TSource>(EOperateType type)
        {
            var table = _Metadata.Table(typeof(TSource));
            if (table.InheritSets.Length > 0)
            {
                var principal = table.InheritSets[table.InheritSets.Length - 1];

                return new DbCreateDropRelationOperate(_Context, type
                    , principal, table, table.Keys.Select(a => new ForeignPrincipalPair(a, a)).ToArray());
            }
            throw new InvalidOperationException(string.Format(Res.ExceptionIsNotInheritSet, typeof(TSource)));
        }
        //创建删除普通关系。
        private IEnumerable<DbMaintenanceOperateBase> CreateDropRelation(Type source, PropertyInfo property, EOperateType type)
        {
            var table = _Metadata.Table(source);
            var navigate = table.Navigates[property];
            if (navigate.IsComposite)
            {
                var composite = (CompositeNavigateMetadata)navigate;
                yield return new DbCreateDropRelationOperate(_Context, type
                    , composite.Source, composite.RelationTable, composite.Pairs);
                yield return new DbCreateDropRelationOperate(_Context, type
                    , composite.Target, composite.RelationTable, composite.CompositePairs);
            }
            else
            {
                if (navigate.IsForeign.Value)
                {
                    yield return new DbCreateDropRelationOperate(_Context, type
                        , navigate.Target, navigate.Source, navigate.Pairs);
                }
                else
                {
                    yield return new DbCreateDropRelationOperate(_Context, type
                         , navigate.Source, navigate.Target, navigate.Pairs);
                }
            }
        }
    }
}