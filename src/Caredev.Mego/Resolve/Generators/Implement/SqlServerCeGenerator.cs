// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using System.Linq;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Contents;
    using Caredev.Mego.Resolve.Generators.Fragments;
    /// <summary>
    /// 针对 SQL Server Compact 数据库的代码生成器。
    /// </summary>
    public abstract class SqlServerCeBaseGenerator : MSLocalDbGenerator
    {
        /// <inheritdoc/>
        public override FragmentWriterBase FragmentWriter
        {
            get
            {
                if (_FragmentWriter == null)
                {
                    _FragmentWriter = new SqlServerCeFragmentWriter(this);
                }
                return _FragmentWriter;
            }
        }
        private FragmentWriterBase _FragmentWriter;
        /// <inheritdoc/>
        public override string ProviderName => "System.Data.SqlServerCe";
        /// <inheritdoc/>
        public override DbFeature Feature => _Feature;
        private readonly DbFeature _Feature = new DbFeature()
        {
            Capability = EDbCapable.Schema | EDbCapable.DataDefinition |
                EDbCapable.WindowFunction | EDbCapable.TableValuedFunction |
                EDbCapable.SubQuery | EDbCapable.BatchInsert |
                EDbCapable.Relation | EDbCapable.Identity
        };
    }
    /// <summary>
    /// 针对 SQL Server Compact 4.0 数据库的代码生成器。
    /// </summary>
    public class SqlServerCe40Generator : SqlServerCeBaseGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0400;
    }
}
