// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using Caredev.Mego.Resolve.Commands;
using System.Data.Common;

namespace Caredev.Mego.Resolve.Providers
{
    /// <summary>
    /// Office Access 访问提供程序。
    /// </summary>
    internal class AccessAccessProvider : DbAccessProvider
    {
        /// <inheritdoc/>
        public override bool IsExclusive => true;
        /// <inheritdoc/>
        public override bool SupportDistributedTransaction => false;
        /// <inheritdoc/>
        public override string ProviderName => "System.Data.OleDb.Access";
        /// <inheritdoc/>
        public override DbProviderFactory Factory => DbAccessProvider.GetFactory("System.Data.OleDb");
        /// <inheritdoc/>
        public override EExecutionMode ExecutionMode => EExecutionMode.SingleStatement;
        /// <inheritdoc/>
        public override ICustomCommand CreateCustomCommand()
        {
            return new AccessCustomCommand(Factory);
        }
        private class AccessCustomCommand : MicrosoftCustomCommand
        {
            public AccessCustomCommand(DbProviderFactory factory)
                : base(factory)
            {
                _Factory = factory;
            }
            private readonly DbProviderFactory _Factory;
            private DbCommand _SelectCommand;
            private DbParameter _IdentityParameter;
            private const string _IdentityParameterName = "@pIdentity";

            private DbParameter[] _SelectParameters;
            protected override DbCommand RunSpliteCommand(DbCommand command, string[] strs, out int affectedCount)
            {
                var returnCommand = base.RunSpliteCommand(command, strs, out affectedCount);
                if (strs.Length > 1)
                {
                    var content = returnCommand.CommandText;
                    if (content.Contains("@@IDENTITY"))
                    {
                        if (_SelectCommand == null)
                        {
                            _IdentityParameter = _Factory.CreateParameter();
                            _IdentityParameter.ParameterName = _IdentityParameterName;

                            _SelectCommand = Clone(returnCommand, content.Replace("@@IDENTITY", _IdentityParameterName), _IdentityParameter);
                        }
                        returnCommand.CommandText = "SELECT @@IDENTITY";
                        _IdentityParameter.Value = returnCommand.ExecuteScalar();
                        return _SelectCommand;
                    }
                    else if (content.StartsWith("SELECT"))
                    {
                        if (_SelectCommand == null)
                        {
                            _SelectParameters = CreateSelect(returnCommand, out _SelectCommand);
                        }
                        else
                        {
                            CopyValue(_SelectCommand, _SelectParameters);
                        }
                        return _SelectCommand;
                    }
                }
                return returnCommand;
            }
        }
    }
}