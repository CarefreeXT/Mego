// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Providers
{
    /// <summary>
    /// SQL Server 数据访问提供程序。
    /// </summary>
    internal class SqlServerAccessProvider : DbAccessProvider
    {
        /// <inheritdoc/>
        public override string ProviderName => "System.Data.SqlClient";
    }
}