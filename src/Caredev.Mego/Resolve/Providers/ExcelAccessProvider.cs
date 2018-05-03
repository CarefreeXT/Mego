// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Providers
{
    using Caredev.Mego.Resolve.Commands;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Text.RegularExpressions;
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
        public override string ProviderName => "System.Data.OleDb.Excel";
        /// <inheritdoc/>
        public override DbProviderFactory Factory => DbAccessProvider.GetFactory("System.Data.OleDb");
        /// <inheritdoc/>
        public override EExecutionMode ExecutionMode => EExecutionMode.SingleStatement;
        /// <inheritdoc/>
        public override ICustomCommand CreateCustomCommand()
        {
            return new ExcelCustomCommand(Factory);
        }
        private class ExcelCustomCommand : MicrosoftCustomCommand
        {
            public ExcelCustomCommand(DbProviderFactory factory)
                : base(factory)
            {
            }

            private DbCommand _SelectCommand;
            private DbParameter[] _Parameters;

            protected override DbCommand RunSpliteCommand(DbCommand command, string[] strs, out int affectedCount)
            {
                var returnCommand = base.RunSpliteCommand(command, strs, out affectedCount);
                if (strs.Length > 1)
                {
                    var content = returnCommand.CommandText;
                    if (content.StartsWith("SELECT"))
                    {
                        if (_SelectCommand == null)
                        {
                            _Parameters = CreateSelect(returnCommand, out _SelectCommand);
                        }
                        else
                        {
                            CopyValue(_SelectCommand, _Parameters);
                        }
                        return _SelectCommand;
                    }
                }
                return returnCommand;
            }
        }
    }
}