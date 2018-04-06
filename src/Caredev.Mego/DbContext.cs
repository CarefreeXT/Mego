// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    using Caredev.Mego.Resolve;
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;
    using Caredev.Mego.Common;
    /// <summary>
    /// 数据执行上下文。
    /// </summary>
    public abstract class DbContext : IDisposable
    {
        //TODO: 临时处理方案，下个版本修改为EMIT创建。
        private struct PropertyConstructor
        {
            public PropertyInfo Property;
            public ConstructorInfo Constructor;
        }
        private static readonly Dictionary<Type, PropertyConstructor[]> _PropertyInfoCache
            = new Dictionary<Type, PropertyConstructor[]>();
#if !NETSTANDARD2_0
        /// <summary>
        /// 根据指定连接字符串配置名称，初始化<see cref="DbContext"/>实例对象。
        /// </summary>
        /// <param name="name">连接字符串配置名称。</param>
        public DbContext(string name)
            : this()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[name];
            Database = new Database(this, connectionString.ConnectionString, connectionString.ProviderName);
        }
#endif
        /// <summary>
        /// 使用指定的连接字符串及提供程序，初始化<see cref="DbContext"/>实例对象。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="providerName">数据提供者名称。</param>
        public DbContext(string connectionString, string providerName)
            : this()
        {
            Database = new Database(this, connectionString, providerName);
        }
        /// <summary>
        /// 使用指定的连接对象及提供程序，初始化<see cref="DbContext"/>实例对象。
        /// </summary>
        /// <param name="existingConnection">存在连接。</param>
        /// <param name="contextOwnsConnection">是否拥有该连接。</param>
        /// <param name="providerName">数据提供者名称，如果为空则由<see cref="DbConnection"/>对象判断。</param>
        public DbContext(DbConnection existingConnection, bool contextOwnsConnection, string providerName = "")
            : this()
        {
            Database = new Database(this, existingConnection, providerName, contextOwnsConnection);
        }
        /// <summary>
        /// 初始化<see cref="DbContext"/>实例对象。
        /// </summary>
        private DbContext()
        {
            Executor = new DbOperateContext(this);
            Configuration = new DbContextConfiguration(this);

            var propertys = _PropertyInfoCache.GetOrAdd(GetType(), type =>
            {
                return type.GetProperties()
                    .Where(a => a.PropertyType.IsGenericType && a.CanWrite &&
                        a.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .Select(a => new PropertyConstructor()
                    {
                        Property = a,
                        Constructor = a.PropertyType.GetConstructors().First()
                    })
                    .ToArray();
            });
            var para = new object[] { this, null };
            foreach (var pro in propertys)
            {
                pro.Property.SetValue(this, pro.Constructor.Invoke(para));
            }
        }
        /// <summary>
        /// 数据库访问对象，通过该对象可以进行数据库底层操作。
        /// </summary>
        public Database Database { get; }
        /// <summary>
        /// 配置对象，可以对当前上下文运行行为进行配置。
        /// </summary>
        public DbContextConfiguration Configuration { get; }
        /// <summary>
        /// 操作执行者，负责执行所有针对数据库的操作。
        /// </summary>
        public DbOperateContext Executor { get; }
        /// <summary>
        /// 创建普通对象数据集。
        /// </summary>
        /// <typeparam name="T">数据对象类型。</typeparam>
        /// <returns>数据集。</returns>
        public DbSet<T> Set<T>() where T : class
        {
            return new DbSet<T>(this);
        }
        /// <summary>
        /// 使用指定名称，创建普通对象数据集。
        /// </summary>
        /// <typeparam name="T">数据对象类型。</typeparam>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>数据集。</returns>
        public DbSet<T> Set<T>(string name, string schema = "") where T : class
        {
            return new DbSet<T>(this, DbName.Create(name, schema));
        }
        /// <summary>
        /// 创建匿名对象数据集，名称对象为<see cref="EDbNameKind.Contact"/>。
        /// </summary>
        /// <typeparam name="T">数据对象类型。</typeparam>
        /// <param name="item">匿名对象，该对象仅做类型推导用。</param>
        /// <param name="name">名称。</param>
        /// <returns>数据集。</returns>
        public DbSet<T> Set<T>(T item, string name) where T : class
        {
            return new DbSet<T>(this, DbName.Contact(name));
        }
        /// <summary>
        /// 创建匿名对象数据集，名称对象为<see cref="EDbNameKind.Contact"/>。
        /// </summary>
        /// <typeparam name="T">数据对象类型。</typeparam>
        /// <param name="items">匿名对象集合，该对象仅做类型推导用。</param>
        /// <param name="name">名称。</param>
        /// <returns>数据集。</returns>
        public DbSet<T> Set<T>(IEnumerable<T> items, string name) where T : class
        {
            return new DbSet<T>(this, DbName.Contact(name));
        }
        /// <summary>
        /// <see cref="IDisposable"/>接口实现。
        /// </summary>
        public void Dispose() => Database.Dispose();
    }
}