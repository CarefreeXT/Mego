// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    using System;
    using System.Data.Common;
    /// <summary>
    /// 数据库访问提供者接口。
    /// </summary>
    public interface IDbAccessProvider
    {
        /// <summary>
        /// 数据库提供者工厂类。
        /// </summary>
        DbProviderFactory Factory { get; }
        /// <summary>
        /// 数据库访问提供者的名称，通常以实现类的根命名空间为准。
        /// </summary>
        string ProviderName { get; }
        /// <summary>
        /// 根据指定的<see cref="DbConnection"/>连接对象获取当前数据库版本号，系统
        /// 使用一个<see cref="Int16"/>数字表示一个完整的版本号，其中高位字节表示
        /// 主版本号，低位字节表示次版本号，例如0x0506，表示数据库版本号为5.6，该
        /// 该版本号主要用于系统查找适合的<see cref="IDbSqlGenerator"/>对象。
        /// </summary>
        /// <param name="connection">当前连接对象。</param>
        /// <returns></returns>
        short GetDatabaseVersion(DbConnection connection);
        /// <summary>
        /// 是否独占模式运行。
        /// </summary>
        bool IsExclusive { get; }
    }
}