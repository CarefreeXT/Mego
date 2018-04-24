// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    /// <summary>
    /// 针对 Oracle 数据库的代码生成器。
    /// </summary>
    public abstract class OracleBaseGenerator : SqlGeneratorBase
    {
        /// <inheritdoc/>
        public override FragmentWriterBase FragmentWriter
        {
            get
            {
                if (_FragmentWriter == null)
                {
                    _FragmentWriter = new OracleFragmentWriter(this);
                }
                return _FragmentWriter;
            }
        }
        private FragmentWriterBase _FragmentWriter;
        /// <inheritdoc/>
        public override string ProviderName => "Oracle.ManagedDataAccess.Client";
        /// <inheritdoc/>
        public override DbFeature Feature => _Feature;
        private readonly DbFeature _Feature = new DbFeature()
        {
            DefaultSchema = "system",
            MaxIdentifierLength = 30,
            Capability = EDbCapable.Schema | EDbCapable.DataDefinition |
                EDbCapable.WindowFunction | EDbCapable.TableValuedFunction |
                EDbCapable.ExternalCompoundStatement | EDbCapable.ExternalLocalVariable |
                EDbCapable.SubQuery | EDbCapable.BatchInsert | EDbCapable.ModifyJoin |
                EDbCapable.Relation | EDbCapable.Sequence
        };
    }
    /// <summary>
    /// Oracle 11g 生成器。
    /// </summary>
    public class Oracle11Generator : OracleBaseGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0B00;
    }
}
