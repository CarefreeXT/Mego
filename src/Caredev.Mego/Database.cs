// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0. 
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve;
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Outputs;
    using Caredev.Mego.Resolve.Providers;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using GeneratorCollection = System.Collections.Generic.SortedDictionary<short, Caredev.Mego.Resolve.IDbSqlGenerator>;
    using Res = Properties.Resources;
    /// <summary>
    /// 数据库操作对象，<see cref="DbContext"/>实际访问数据库的中间对象。
    /// </summary>
    public class Database : IDisposable
    {
        private readonly static Dictionary<string, IDbAccessProvider> _Providers;
        private readonly static Dictionary<string, GeneratorCollection> _Generators;
        private readonly static Dictionary<string, IDbSqlGenerator> _ConnectionStringGenerators;
        private readonly IDbAccessProvider _Provider;
        private readonly DatabaseExecutor _DatabaseExecutor;
        private readonly DbContext _Context;
        private readonly MetadataEngine _Metadata;
        private DbManager _Manager;
        private DbFeature _Feature;
        private IDbSqlGenerator _Generator;
        static Database()
        {
            _Providers = new Dictionary<string, IDbAccessProvider>();
            _Generators = new Dictionary<string, GeneratorCollection>();
            _ConnectionStringGenerators = new Dictionary<string, IDbSqlGenerator>();
            var assembly = typeof(Database).Assembly;
            var types = assembly.GetTypes();
            var empty = new Type[] { };
            foreach (var type in types.Where(a => a.IsClass && !a.IsAbstract))
            {
                if (typeof(DbAccessProvider).IsAssignableFrom(type))
                { //添加数据访问提供程序
                    if (type.GetConstructor(empty) != null)
                    {
                        var provider = (DbAccessProvider)Activator.CreateInstance(type);
                        RegisterOrReplace(provider);
                    }
                }
                else if (typeof(IDbSqlGenerator).IsAssignableFrom(type) && type.GetConstructor(empty) != null)
                { //添加 SQL 语句生成
                    if (type.GetConstructor(empty) != null)
                    {
                        var generator = (IDbSqlGenerator)Activator.CreateInstance(type);
                        RegisterOrReplace(generator);
                    }
                }
            }
        }
        private class GeneratorVersionComparer : IComparer<short>
        {
            public readonly static GeneratorVersionComparer Instance = new GeneratorVersionComparer();

            public int Compare(short x, short y)
            {
                return y - x;
            }
        }
        /// <summary>
        /// 判断传入的提供者名称是否为空，如果为空则通过数据库连接对象生成提供者名称。
        /// </summary>
        /// <param name="providerName">提供者名称。</param>
        /// <param name="connection">连接对象。</param>
        /// <returns>提供程序名称。</returns>
        private static string GetProviderNameByConnection(string providerName, DbConnection connection)
        {
            if (string.IsNullOrEmpty(providerName))
                return connection.GetType().Namespace;
            return providerName;
        }
        /// <summary>
        /// 注册或替换数据库访问提供者。
        /// </summary>
        /// <param name="provider">提供者实例。</param>
        public static void RegisterOrReplace(IDbAccessProvider provider)
        {
            _Providers.AddOrUpdate(provider.ProviderName.ToLower(), provider);
        }
        /// <summary>
        /// 注册或替换 SQL 生成器。
        /// </summary>
        /// <param name="generator">生成器实例。</param>
        public static void RegisterOrReplace(IDbSqlGenerator generator)
        {
            lock (_Generators)
            {
                var providerName = generator.ProviderName.ToLower();
                if (!_Generators.TryGetValue(providerName, out GeneratorCollection generators))
                {
                    generators = new GeneratorCollection(GeneratorVersionComparer.Instance);
                    _Generators.Add(providerName, generators);
                }
                generators.AddOrUpdate(generator.Version, generator);
            }
        }
        /// <summary>
        /// 通过给定的数据库连接字符串与提供者名称初始化<see cref="Database"/>实例。
        /// </summary>
        /// <param name="context">数据执行上下文。</param>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="providerName">提供者名称。</param>
        internal Database(DbContext context, string connectionString, string providerName)
            : this(context, providerName)
        {
            var currentConnection = _Provider.Factory.CreateConnection();
            currentConnection.ConnectionString = connectionString;
            _DatabaseExecutor = new DatabaseExecutor(_Provider, currentConnection, true);
        }
        /// <summary>
        /// 通过给定的数据库连接对象与提供者名称初始化<see cref="Database"/>实例。
        /// </summary>
        /// <param name="context">数据执行上下文。</param>
        /// <param name="existingConnection">存在的连接对象。</param>
        /// <param name="providerName">提供程序名称。</param>
        /// <param name="contextOwnsConnection">当前数据上下文是否拥有该连接。</param>
        internal Database(DbContext context, DbConnection existingConnection, string providerName, bool contextOwnsConnection)
            : this(context, GetProviderNameByConnection(providerName, existingConnection))
        {
            _DatabaseExecutor = new DatabaseExecutor(_Provider, existingConnection, contextOwnsConnection);
        }
        /// <summary>
        /// 初始化<see cref="Database"/>对象实例数据执行上下文。
        /// </summary>
        /// <param name="context">数据执行上下文。</param>
        /// <param name="providerName">提供程序名称。</param>
        private Database(DbContext context, string providerName)
        {
            Utility.NotNull(context, nameof(context));
            if (!_Providers.TryGetValue(providerName.ToLower(), out _Provider))
            {
                throw new ArgumentException(string.Format(Res.ExceptionNotFoundProvider, providerName), nameof(providerName));
            }
            _Context = context;
            _Metadata = context.Configuration.Metadata;
        }
        /// <summary>
        /// 当前数据库连接。
        /// </summary>
        public DbConnection Connection
        {
            get { return _DatabaseExecutor.Connection; }
        }
        /// <summary>
        /// 当前数据库访问提供者。
        /// </summary>
        public IDbAccessProvider Provider => _Provider;
        /// <summary>
        /// 当前数据库运行时特性，该特性决定了与数据库相关的运行行为，每个<see cref="DbContext"/>
        /// 实例会从<see cref="IDbSqlGenerator.Feature"/>创建一个复本。
        /// </summary>
        public DbFeature Feature
        {
            get
            {
                if (_Feature == null)
                {
                    _Feature = Generator.Feature.Clone();
                }
                return _Feature;
            }
        }
        /// <summary>
        /// SQL生成引擎。
        /// </summary>
        public IDbSqlGenerator Generator
        {
            get
            {
                if (_Generator == null)
                {
                    var connection = Connection;
                    if (!_ConnectionStringGenerators.TryGetValue(connection.ConnectionString, out _Generator))
                    {
                        if (!_Generators.TryGetValue(Provider.ProviderName.ToLower(), out GeneratorCollection generators))
                        {
                            throw new KeyNotFoundException(string.Format(Res.ExceptionNotFoundGeneratorCollection, Provider.ProviderName));
                        }
                        var version = _Provider.GetDatabaseVersion(Connection);
                        if (!generators.TryGetValue(version, out _Generator))
                        {
                            foreach (var generator in generators.Values)
                            {
                                if (generator.Version <= version)
                                {
                                    _Generator = generator;
                                    return _Generator;
                                }
                            }
                            throw new GeneratorNotFoundException(string.Format(Res.ExceptionNotFoundProvider, Provider.ProviderName, version));
                        }
                        _ConnectionStringGenerators.AddOrUpdate(connection.ConnectionString, _Generator);
                    }
                }
                return _Generator;
            }
            set
            {
                Utility.NotNull(value, nameof(value));
                _Generator = value;
            }
        }
        /// <summary>
        /// 当前数据库管理者。
        /// </summary>
        public DbManager Manager => _Manager ?? (_Manager = new DbManager(_Context, _Metadata));
        /// <summary>
        /// 数据库执行体。
        /// </summary>
        internal DatabaseExecutor Executor => _DatabaseExecutor;
        /// <summary>
        /// 使用指定当前的事务。
        /// </summary>
        /// <param name="transaction"></param>
        public void UseTransaction(DbTransaction transaction)
        {
            _DatabaseExecutor.UseTransaction(transaction);
        }
        /// <summary>
        ///  开始一个的事务。
        /// </summary>
        /// <returns></returns>
        public DbTransaction BeginTransaction() => _DatabaseExecutor.BeginTransaction();
        /// <summary>
        /// 对数据库执行给定的 SQL 命令。查询参数以 p 为前缀，从数字 0 开始计数。
        /// </summary>
        /// <param name="sql">命令字符串。</param>
        /// <param name="parameters">要应用于命令字符串的参数。</param>
        /// <returns>执行命令后由数据库返回的结果。</returns>
        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return _DatabaseExecutor.ExecuteInTransaction(() =>
            {
                return _DatabaseExecutor.CreateCommand(sql, parameters).ExecuteNonQuery();
            });
        }
        /// <summary>
        /// 通过给定的 SQL 命令执行查询，并将返回的结果转化为对象集合。查询参数以 p 为前缀，从数字 0 开始计数。
        /// </summary>
        /// <param name="elementType">结果元素类型。</param>
        /// <param name="sql">命令字符串。</param>
        /// <param name="parameters">要应用于命令字符串的参数。</param>
        /// <returns>查询结果集。</returns>
        public IEnumerable SqlQuery(Type elementType, string sql, params object[] parameters)
        {
            return _DatabaseExecutor.ExecuteInTransaction(() =>
            {
                var reader = _DatabaseExecutor.CreateCommand(sql, parameters).ExecuteReader();
                return CreateOutput(elementType, reader).GetResult(reader).ToList();
            });
        }
        /// <summary>
        /// 通过给定的 SQL 命令执行查询，并将返回的结果转化为对象集合。查询参数以 p 为前缀，从数字 0 开始计数。
        /// </summary>
        /// <typeparam name="TElement">结果元素类型。</typeparam>
        /// <param name="sql">命令字符串。</param>
        /// <param name="parameters">要应用于命令字符串的参数。</param>
        /// <returns>查询强类型结果集。</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return _DatabaseExecutor.ExecuteInTransaction(() =>
            {
                var elementType = typeof(TElement);
                var command = _DatabaseExecutor.CreateCommand(sql, parameters);
                using (var reader = command.ExecuteReader())
                {
                    var result = CreateOutput(elementType, reader).GetResult(reader);
                    if (elementType.IsPrimary())
                    {
                        return result.Select(a => (TElement)System.Convert.ChangeType(a, elementType)).ToArray();
                    }
                    return result.OfType<TElement>().ToArray();
                }
            });
        }
        /// <summary>
        /// <see cref="IDisposable"/>接口实现。
        /// </summary>
        public void Dispose()
        {
            _DatabaseExecutor.Dispose();
        }
        /// <summary>
        /// 用指定的对象类型创建<see cref="IMultiOutput"/>对象。
        /// </summary>
        /// <param name="elementType">目标元素类型。</param>
        /// <param name="reader">数据库访问<see cref="DbDataReader"/>对象。</param>
        /// <returns>查询输出对象。</returns>
        private IMultiOutput CreateOutput(Type elementType, DbDataReader reader)
        {
            if (elementType.IsPrimary())
                return new MultiValueOutputInfo();
            else if (elementType.IsObject())
                return new CollectionOutputInfo(reader, _Metadata.Type(elementType));
            throw new NotSupportedException(string.Format(Res.NotSupportedOutpputClrType, elementType));
        }
    }
}