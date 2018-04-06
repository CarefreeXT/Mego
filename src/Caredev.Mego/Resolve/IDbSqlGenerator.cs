// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Operates;
    using System;
    /// <summary>
    /// SQL 脚本生成器接口。
    /// </summary>
    public interface IDbSqlGenerator
    {
        /// <summary>
        /// 根据当前操作内容及表达式生成 SQL 脚本。
        /// </summary>
        /// <param name="operate">操作对象。</param>
        /// <param name="content">表达式对象。</param>
        /// <returns></returns>
        string Generate(DbOperateBase operate, DbExpression content);
        /// <summary>
        /// 数据访问提供者名称。
        /// </summary>
        string ProviderName { get; }
        /// <summary>
        /// 当前生成器适用的数据库版本，系统使用一个<see cref="Int16"/>数字表示一个
        /// 完整的版本号，其中高位字节表示主版本号，低位字节表示次版本号，例如0x0506，
        /// 表示数据库版本号为5.6，版本号获取由相应的<see cref="IDbAccessProvider"/>
        /// 实现。
        /// </summary>
        short Version { get; }
        /// <summary>
        /// 数据库特性（该特性为针对某类数据库的全局特性）。
        /// </summary>
        DbFeature Feature { get; }
    }
}