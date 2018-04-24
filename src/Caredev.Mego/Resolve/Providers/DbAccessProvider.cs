// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Providers
{
    using Caredev.Mego.Common;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Text.RegularExpressions;
    using Res = Properties.Resources;
    /// <summary>
    /// 获取<see cref="DbProviderFactory"/>对象的<see cref="Delegate"/>。
    /// </summary>
    /// <param name="providerName">提供程序名称。</param>
    /// <returns>数据访问提供工厂类对象。</returns>
    public delegate DbProviderFactory GetDbProviderFactoryDelegate(string providerName);
    /// <summary>
    /// 数据库访问提供程序。
    /// </summary>
    public abstract class DbAccessProvider : IDbAccessProvider
    {
        internal static GetDbProviderFactoryDelegate GetFactory;
        static DbAccessProvider()
        {
#if !NETSTANDARD2_0
            GetFactory = DbProviderFactories.GetFactory;
#endif
        }
        /// <summary>
        /// 设置获取<see cref="DbProviderFactory"/>工厂方法。
        /// </summary>
        /// <param name="method"></param>
        public static void SetGetFactory(GetDbProviderFactoryDelegate method)
        {
            Utility.NotNull(method, nameof(method));
            GetFactory = method;
        }
        /// <summary>
        /// 获取<see cref="DbProviderFactory"/>对象。
        /// </summary>
        public virtual DbProviderFactory Factory
        {
            get { return GetFactory(ProviderName); }
        }
        /// <summary>
        /// 数据库访问提供程序名称。
        /// </summary>
        public abstract string ProviderName { get; }
        /// <summary>
        /// 是否独占模式运行。
        /// </summary>
        public virtual bool IsExclusive => false;
        /// <summary>
        /// 是否支持分布式事务。
        /// </summary>
        public virtual bool SupportDistributedTransaction => true;
        /// <summary>
        /// 获取数据库版本号。
        /// </summary>
        /// <param name="connection">连接对象。</param>
        /// <returns>当前数据库版本号。</returns>
        public virtual short GetDatabaseVersion(DbConnection connection)
        {
            string serverVersion = string.Empty;
            if (connection.State != ConnectionState.Open)
            {
                if (IsExclusive)
                {
                    try
                    {
                        serverVersion = connection.ServerVersion;
                    }
                    catch
                    {
                        connection.Open();
                        serverVersion = connection.ServerVersion;
                    }
                }
                else
                {
                    connection.Open();
                    serverVersion = connection.ServerVersion;
                    connection.Close();
                }
            }
            else
            {
                serverVersion = connection.ServerVersion;
            }
            var match = Regex.Match(serverVersion, VersionPattern);
            if (!match.Success)
            {
                throw new InvalidCastException(string.Format(Res.ExceptionInvalidCastServerVersion, serverVersion));
            }
            return Convert.ToInt16((int.Parse(match.Groups["main"].Value) << 8) + int.Parse(match.Groups["minor"].Value));
        }
        /// <summary>
        /// 生成版本号的正则表达式。
        /// </summary>
        protected virtual string VersionPattern => @"^(?<main>\d{1,2})\.(?<minor>\d{1,2})";
        /// <summary>
        /// <see cref="IDbAccessProvider.ExecutionMode"/>
        /// </summary>
        public abstract EExecutionMode ExecutionMode { get; }
    }
}