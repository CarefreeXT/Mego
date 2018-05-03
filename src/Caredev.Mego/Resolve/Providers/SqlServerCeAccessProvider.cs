// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Providers
{
    using Caredev.Mego.Resolve.Commands;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Reflection;
    /// <summary>
    /// 访问Nuget组件: Microsoft.SqlServer.Compact
    /// </summary>
    internal class SqlServerCeAccessProvider : DbAccessProvider
    {
        /// <inheritdoc/>
        public override bool IsExclusive => true;
        /// <inheritdoc/>
        public override string ProviderName => "System.Data.SqlServerCe";
        /// <inheritdoc/>
        public override EExecutionMode ExecutionMode => EExecutionMode.SingleStatement;
        /// <inheritdoc/>
        public override ICustomCommand CreateCustomCommand()
        {
            return new MicrosoftCustomCommand(Factory);
        }
    }
}