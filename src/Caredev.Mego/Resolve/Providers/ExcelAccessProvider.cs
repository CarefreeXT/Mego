// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using Caredev.Mego.Resolve.Commands;
using System.Data.Common;

namespace Caredev.Mego.Resolve.Providers
{
    /// <summary>
    /// Office Excel 访问提供程序。
    /// </summary>
    internal class ExcelAccessProvider : DbAccessProvider
    {
        /// <inheritdoc/>
        public override bool IsExclusive => true;
        /// <inheritdoc/>
        public override bool SupportDistributedTransaction => false;
        /// <inheritdoc/>
        public override string ProviderName => "System.Data.Excel";
        /// <inheritdoc/>
        public override DbProviderFactory Factory => DbAccessProvider.GetFactory("System.Data.OleDb");
        /// <inheritdoc/>
        public override EExecutionMode ExecutionMode => EExecutionMode.SingleStatement;
        /// <inheritdoc/>
        public override ICustomCommand CreateCustomCommand()
        {
            return new MicrosoftCustomCommand(Factory);
        }
    }
}