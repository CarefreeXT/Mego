// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
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
        /// 创建临时数据表。
        /// </summary>
        /// <typeparam name="T">指定对象类型。</typeparam>
        /// <param name="name">指定表名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateTempTable<T>(string name)
        {
            return CreateTempTable(typeof(T), name);
        }
        /// <summary>
        /// 创建临时数据表。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="name">指定表名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateTempTable(Type type, string name)
        {
            return new DbCreateTableOperate(_Context, type, EOperateType.CreateTempTable, DbName.Contact(name));
        }
        /// <summary>
        /// 创建数据表变量。
        /// </summary>
        /// <typeparam name="T">指定对象类型。</typeparam>
        /// <param name="name">指定表名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateTableVariable<T>(string name)
        {
            return CreateTableVariable(typeof(T), name);
        }
        /// <summary>
        /// 创建数据表变量。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="name">指定表名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateTableVariable(Type type, string name)
        {
            return new DbCreateTableOperate(_Context, type, EOperateType.CreateTableVariable, DbName.Contact(name));
        }
        /// <summary>
        /// 判断指定的数据表是否存在。
        /// </summary>
        /// <typeparam name="T">映射的对象类型。</typeparam>
        /// <param name="name">表名。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase TableIsExsit<T>(string name = "", string schema = "")
        {
            if (name == "")
            {
                return TableIsExsit(typeof(T));
            }
            else
            {
                return TableIsExsit(typeof(T), DbName.Create(name, schema));
            }
        }
        /// <summary>
        /// 判断指定的数据表是否存在。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="name">操作对象名称。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase TableIsExsit(Type type, DbName name = null)
        {
            if (name == null)
            {
                var table = _Metadata.Table(type);
                name = DbName.Create(table.Name, table.Schema);
            }
            return new DbObjectIsExsitOperate(_Context, type, EOperateType.TableIsExsit, name);
        }
        /// <summary>
        /// 创建数据表。
        /// </summary>
        /// <typeparam name="T">指定对象类型。</typeparam>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateTable<T>(string name = "", string schema = "")
        {
            if (name == "")
            {
                return CreateTable(typeof(T));
            }
            else
            {
                return CreateTable(typeof(T), DbName.Create(name, schema));
            }
        }
        /// <summary>
        /// 创建数据表。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="name">指定表名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateTable(Type type, DbName name = null)
        {
            if (name == null)
            {
                var table = _Metadata.Table(type);
                name = DbName.Create(table.Name, table.Schema);
            }
            return new DbCreateTableOperate(_Context, type, EOperateType.CreateTable, name);
        }
        /// <summary>
        /// 删除数据表。
        /// </summary>
        /// <typeparam name="T">映射的对象类型。</typeparam>
        /// <param name="newName">操作对象名称。</param>
        /// <param name="name">表名。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase RenameTable<T>(string newName, string name = "", string schema = "")
        {
            if (name == "")
            {
                return RenameTable(typeof(T), newName);
            }
            else
            {
                return RenameTable(typeof(T), newName, DbName.Create(name, schema));
            }
        }
        /// <summary>
        /// 重命名数据表。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="newName">操作对象名称。</param>
        /// <param name="name">操作对象名称。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase RenameTable(Type type, string newName, DbName name = null)
        {
            if (name == null)
            {
                var table = _Metadata.Table(type);
                name = DbName.Create(table.Name, table.Schema);
            }
            return new DbRenameObjectOperate(_Context, type, EOperateType.RenameTable, name, newName);
        }
        /// <summary>
        /// 删除数据表。
        /// </summary>
        /// <typeparam name="T">映射的对象类型。</typeparam>
        /// <param name="name">表名。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase DropTable<T>(string name = "", string schema = "")
        {
            if (name == "")
            {
                return DropTable(typeof(T));
            }
            else
            {
                return DropTable(typeof(T), DbName.Create(name, schema));
            }
        }
        /// <summary>
        /// 删除数据表。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="name">操作对象名称。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase DropTable(Type type, DbName name = null)
        {
            if (name == null)
            {
                var table = _Metadata.Table(type);
                name = DbName.Create(table.Name, table.Schema);
            }
            return new DbDropObjectOperate(_Context, type, EOperateType.DropTable, name);
        }

        /// <summary>
        /// 判断指定的视图是否存在。
        /// </summary>
        /// <typeparam name="T">映射的对象类型。</typeparam>
        /// <param name="name">视图名。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase ViewIsExsit<T>(string name = "", string schema = "")
        {
            if (name == "")
            {
                return ViewIsExsit(typeof(T));
            }
            else
            {
                return ViewIsExsit(typeof(T), DbName.Create(name, schema));
            }
        }
        /// <summary>
        /// 判断指定的视图是否存在。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="name">操作对象名称。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase ViewIsExsit(Type type, DbName name = null)
        {
            if (name == null)
            {
                var View = _Metadata.Table(type);
                name = DbName.Create(View.Name, View.Schema);
            }
            return new DbObjectIsExsitOperate(_Context, type, EOperateType.ViewIsExsit, name);
        }
        /// <summary>
        /// 创建视图。
        /// </summary>
        /// <typeparam name="T">指定对象类型。</typeparam>
        /// <param name="query">查询内容。</param>
        /// <param name="name">视图名。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateView<T>(IQueryable<T> query, string name = "", string schema = "")
        {
            if (name == "")
            {
                return CreateView(typeof(T), query);
            }
            else
            {
                return CreateView(typeof(T), query, DbName.Create(name, schema));
            }
        }
        /// <summary>
        /// 创建视图。
        /// </summary>
        /// <typeparam name="T">指定对象类型。</typeparam>
        /// <param name="content">视图内容。</param>
        /// <param name="name">视图名。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateView<T>(string content, string name = "", string schema = "")
        {
            if (name == "")
            {
                return CreateView(typeof(T), content);
            }
            else
            {
                return CreateView(typeof(T), content, DbName.Create(name, schema));
            }
        }
        /// <summary>
        /// 创建视图。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="query">查询内容。</param>
        /// <param name="name">指定视图名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateView(Type type, IQueryable query, DbName name = null)
        {
            if (name == null)
            {
                var table = _Metadata.Table(type);
                name = DbName.Create(table.Name, table.Schema);
            }
            return new DbCreateViewOperate(_Context, type, name, query.Expression);
        }
        /// <summary>
        /// 创建视图。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="content">视图内容。</param>
        /// <param name="name">指定视图名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase CreateView(Type type, string content, DbName name = null)
        {
            if (name == null)
            {
                var table = _Metadata.Table(type);
                name = DbName.Create(table.Name, table.Schema);
            }
            return new DbCreateViewOperate(_Context, type, name, content);
        }
        /// <summary>
        /// 删除视图。
        /// </summary>
        /// <typeparam name="T">映射的对象类型。</typeparam>
        /// <param name="newName">操作对象名称。</param>
        /// <param name="name">视图名。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase RenameView<T>(string newName, string name = "", string schema = "")
        {
            if (name == "")
            {
                return RenameView(typeof(T), newName);
            }
            else
            {
                return RenameView(typeof(T), newName, DbName.Create(name, schema));
            }
        }
        /// <summary>
        /// 重命名视图。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="newName">操作对象名称。</param>
        /// <param name="name">操作对象名称。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase RenameView(Type type, string newName, DbName name = null)
        {
            if (name == null)
            {
                var View = _Metadata.Table(type);
                name = DbName.Create(View.Name, View.Schema);
            }
            return new DbRenameObjectOperate(_Context, type, EOperateType.RenameView, name, newName);
        }
        /// <summary>
        /// 删除视图。
        /// </summary>
        /// <typeparam name="T">映射的对象类型。</typeparam>
        /// <param name="name">视图名。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase DropView<T>(string name = "", string schema = "")
        {
            if (name == "")
            {
                return DropView(typeof(T));
            }
            else
            {
                return DropView(typeof(T), DbName.Create(name, schema));
            }
        }
        /// <summary>
        /// 删除视图。
        /// </summary>
        /// <param name="type">指定对象类型。</param>
        /// <param name="name">操作对象名称。</param>
        /// <returns>操作实例。</returns>
        public DbMaintenanceOperateBase DropView(Type type, DbName name = null)
        {
            if (name == null)
            {
                var View = _Metadata.Table(type);
                name = DbName.Create(View.Name, View.Schema);
            }
            return new DbDropObjectOperate(_Context, type, EOperateType.DropView, name);
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
                DbCreateDropRelationOperate operate = null;
                if (navigate.IsForeign.Value)
                {
                    operate = new DbCreateDropRelationOperate(_Context, type
                        , navigate.Target, navigate.Source, navigate.Pairs);
                }
                else
                {
                    operate = new DbCreateDropRelationOperate(_Context, type
                         , navigate.Source, navigate.Target, navigate.Pairs);
                }
                if (type == EOperateType.CreateRelation)
                {
                    operate.Action = navigate.GetProperty<RelationActionAttribute>();
                }
                yield return operate;
            }
        }
    }
}