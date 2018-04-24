// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Providers
{
    /// <summary>
    /// 访问Nuget组件:Npgsql
    /// </summary>
    internal class PostgreSQLAccessProvider : DbAccessProvider
    {
        /// <inheritdoc/>
        public override bool SupportDistributedTransaction => false;
        /// <inheritdoc/>
        public override string ProviderName => "Npgsql";
        /// <inheritdoc/>
        public override EExecutionMode ExecutionMode => EExecutionMode.MergeOperations;
    }
}