// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using System.Data.Common;

namespace Caredev.Mego.Resolve.Providers
{
    /// <summary>
    /// Office Access 访问提供程序。
    /// </summary>
    internal class AccessAccessProvider : DbAccessProvider
    {
        /// <inheritdoc/>
        public override string ProviderName => "System.Data.Access";
        /// <inheritdoc/>
        public override DbProviderFactory Factory => DbAccessProvider.GetFactory("System.Data.OleDb");
        /// <inheritdoc/>
        public override EExecutionMode ExecutionMode => EExecutionMode.SingleStatement;
    }
}