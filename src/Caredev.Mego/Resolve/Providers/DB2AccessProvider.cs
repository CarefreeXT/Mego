// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Providers
{
    using System;
    /// <summary>
    /// 访问Nuget组件:IBM.Data.DB2
    /// </summary>
    internal class DB2AccessProvider : DbAccessProvider
    {
        /// <inheritdoc/>
        public override string ProviderName => "IBM.Data.DB2";
        /// <inheritdoc/>
        public override EExecutionMode ExecutionMode => throw new NotImplementedException();
    }
}