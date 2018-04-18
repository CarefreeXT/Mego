// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Res = Properties.Resources;
    /// <summary>
    /// 数据库操作上下文。
    /// </summary>
    public class DbOperateContext : IEnumerable<DbOperateBase>
    {
        /// <summary>
        /// 创建数据库操作上下文。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        public DbOperateContext(DbContext context)
        {
            Context = context;
            _Operates = new Dictionary<Type, List<DbOperateBase>>();
        }
        /// <summary>
        /// 数据上下文。
        /// </summary>
        public DbContext Context { get; }
        /// <summary>
        /// 当前命令对象。
        /// </summary>
        internal DbOperateCommandBase CurrentCommand { get; set; }
        /// <summary>
        /// 所有操作枚举对象。
        /// </summary>
        private IEnumerable<DbOperateBase> Operates
        {
            get
            {
                foreach (var list in _Operates.Values)
                {
                    foreach (var operate in list)
                    {
                        yield return operate;
                    }
                }
            }
        }
        private Dictionary<Type, List<DbOperateBase>> _Operates;
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现。
        /// </summary>
        /// <returns>枚举器。</returns>
        public IEnumerator<DbOperateBase> GetEnumerator()
        {
            return Operates.GetEnumerator();
        }
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现。
        /// </summary>
        /// <returns>枚举器。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Operates.GetEnumerator();
        }
        /// <summary>
        /// 查找指定类型相关的操作集合。
        /// </summary>
        /// <param name="type">查找的类型。</param>
        /// <returns>查找的结果列表。</returns>
        internal IEnumerable<DbOperateBase> Find(Type type)
        {
            lock (_Operates)
            {
                if (_Operates.TryGetValue(type, out List<DbOperateBase> value))
                {
                    return value;
                }
                return null;
            }
        }
        /// <summary>
        /// 按指定的成员类型及条件查找操作对象。
        /// </summary>
        /// <typeparam name="TOperate">操作类型。</typeparam>
        /// <param name="type">指定数据类型。</param>
        /// <param name="predicate">查找条件。</param>
        /// <returns>查找结果。</returns>
        internal TOperate Find<TOperate>(Type type, Func<TOperate, bool> predicate = null) where TOperate : DbOperateBase
        {
            lock (_Operates)
            {
                if (_Operates.TryGetValue(type, out List<DbOperateBase> operates))
                {
                    if (predicate == null)
                    {
                        return operates.OfType<TOperate>().FirstOrDefault();
                    }
                    else
                    {
                        return operates.OfType<TOperate>().FirstOrDefault(predicate);
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// 添加操作。
        /// </summary>
        /// <param name="item">操作对象。</param>
        internal void Add(DbOperateBase item)
        {
            lock (_Operates)
            {
                if (!_Operates.TryGetValue(item.ClrType, out List<DbOperateBase> value))
                {
                    value = new List<DbOperateBase>();
                    _Operates.Add(item.ClrType, value);
                }
                value.Add(item); /**/
            }
        }
        /// <summary>
        /// 删除操作。
        /// </summary>
        /// <param name="item">操作对象。</param>
        internal void Remove(DbOperateBase item)
        {
            lock (_Operates)
            {
                if (_Operates.TryGetValue(item.ClrType, out List<DbOperateBase> value))
                {
                    value.Remove(item);
                }
            }
        }
        /// <summary>
        /// 操作总数。
        /// </summary>
        internal int Count => _Operates.Values.Sum(a => a.Count);
        /// <summary>
        /// 是否包含指定的操作对象。
        /// </summary>
        /// <param name="item">判断对象。</param>
        /// <returns>包含则返回 true，否则返回 false。</returns>
        internal bool Contains(DbOperateBase item) => _Operates.Values.Any(a => a.Contains(item));
        /// <summary>
        /// 执行指定的操作，如果不指定则会提交上下文中缓存的所有操作。
        /// </summary>
        /// <param name="operates">执行的操作集合。</param>
        /// <returns>对数据库的影响行数。</returns>
        public int Execute(params DbOperateBase[] operates)
        {
            int result = 0;
            if (operates.Length == 0)
            {
                result = ExecuteImp(Operates);
                _Operates.Clear();
            }
            else
            {
                result = Execute(operates.OfType<DbOperateBase>());
            }
            return result;
        }
        /// <summary>
        /// 执行多个指定的操作。
        /// </summary>
        /// <param name="operates">执行的操作集合。</param>
        /// <returns>对数据库的影响行数。</returns>
        public int Execute(IEnumerable<DbOperateBase> operates)
        {
            var result = ExecuteImp(operates);
            foreach (var operate in operates)
            {
                if (_Operates.TryGetValue(operate.ClrType, out List<DbOperateBase> list))
                {
                    list.Remove(operate);
                }
            }
            return result;
        }
        //执行操作提交给数据库。
        private int ExecuteImp(IEnumerable<DbOperateBase> operates)
        {
            var readyOperates = new HashSet<DbOperateBase>(operates);
            if (readyOperates.Count > 0)
            {
                var currentOperates = GetOperates(readyOperates);
                var commands = GenerateCommands(currentOperates);
                var executor = Context.Database.Executor;

                int result = executor.ExecuteInTransaction(() =>
                {
                    var number = 0;
                    foreach (var command in commands)
                    {
                        number += command.Execute(executor);
                    }
                    while (readyOperates.Count > 0)
                    {
                        foreach (var operation in currentOperates)
                        {
                            foreach (var ready in readyOperates)
                            {
                                ready.Dependents.Remove(operation);
                            }
                        }
                        currentOperates = GetOperates(readyOperates);
                        commands = GenerateCommands(currentOperates);
                        foreach (var command in commands)
                        {
                            number += command.Execute(executor);
                        }
                    }
                    return number;
                });
                UpdateRelationOperate(operates);
                return result;
            }
            return 0;
        }
        //递归获取将要执行的操作，该函数用于处理操作间的依赖关系，若存在关系提交的
        //数据，则会更新相应CLR对象的值。
        private DbOperateBase[] GetOperates(HashSet<DbOperateBase> operates)
        {
            var currentOperates = operates.Where(a => a.Dependents.Count == 0).ToArray();
            foreach (var relations in currentOperates.OfType<IInsertReferenceRelation>())
            {
                var metadata = this.Context.Configuration.Metadata;
                foreach (var relation in relations.Relations)
                {
                    relation.UpdatePrimaryMember();
                }
            }
            if (currentOperates.Length == 0 && operates.Count > 0)
            {
                throw new NotSupportedException(Res.NotSupportedCircularDependenceOperate);
            }
            operates.ExceptWith(currentOperates);
            return currentOperates;
        }
        //更新关系操作值。
        private void UpdateRelationOperate(IEnumerable<DbOperateBase> operates)
        {
            foreach (var relation in operates.OfType<DbRelationOperateBase>())
            {
                var metadata = this.Context.Configuration.Metadata;
                if (!relation.Navigate.IsComposite)
                {
                    relation.UpdatePrimaryMember();
                }
                relation.UpdateComplexMember();
            }
            foreach (var operate in operates.OfType<IInsertReferenceRelation>().Where(a => a.Relations.Count > 0))
            {
                var metadata = this.Context.Configuration.Metadata;
                foreach (var relation in operate.Relations)
                {
                    relation.UpdateComplexMember();
                }
            }
        }
        //根据操作集合生成执行命令集合对象。
        private DbOperateCommandCollection GenerateCommands(IEnumerable<DbOperateBase> operateCollection)
        {
            var operates = operateCollection.ToArray();
            var commands = new DbOperateCommandCollection(this);
            var database = Context.Database;

            int parametercount = database.Feature.MaxParameterCountForOperate;
            foreach (var operate in operates)
            {
                if (operate is IDbSplitObjectsOperate itemoperate)
                {
                    var current = commands.CheckParameterCount(parametercount);
                    parametercount = itemoperate.ItemParameterCount * itemoperate.Count;
                    if (operate is IConcurrencyCheckOperate concurrency && concurrency.NeedCheck
                        && current.ConcurrencyExpectCount == 0 && !current.IsEmpty)
                    {
                        //如果当前操作需要参与并发检查，但是当前命令中已包含非并发检查操作，则移动到下一个命令中。
                        current = commands.NextCommand();
                    }
                    if (parametercount > current.ParameterCount)
                    {
                        commands.Register(itemoperate);
                    }
                    else
                    {
                        commands.Register(operate, parametercount);
                    }
                }
                else
                {
                    commands.Register(operate, parametercount);
                }
            }
            return commands;
        }
    }
}