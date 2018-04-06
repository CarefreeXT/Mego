// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    /// <summary>
    /// 针对 SQL Server 数据库的代码生成器。
    /// </summary>
    public abstract partial class SqlServerBaseGenerator : SqlGeneratorBase
    {
        /// <inheritdoc/>
        public override FragmentWriterBase FragmentWriter
        {
            get
            {
                if (_FragmentWriter == null)
                {
                    _FragmentWriter = new SqlServerFragmentWriter(this);
                }
                return _FragmentWriter;
            }
        }
        private FragmentWriterBase _FragmentWriter;
        /// <inheritdoc/>
        public override string ProviderName => "System.Data.SqlClient";
        /// <inheritdoc/>
        public override DbFeature Feature => _Feature;
        private readonly DbFeature _Feature = new DbFeature()
        {
            DefaultSchema = "dbo",
            MaxInsertRowCount = 1000,
            MaxParameterCount = 2098,
            Capability = EDbCapable.Schema | EDbCapable.DataDefinition |
                EDbCapable.TemporaryTable | EDbCapable.TableVariable |
                EDbCapable.WindowFunction | EDbCapable.TableValuedFunction |
                EDbCapable.ExternalCompoundStatement | EDbCapable.ExternalLocalVariable |
                EDbCapable.SubQuery | EDbCapable.BatchInsert | EDbCapable.ModifyReturning | EDbCapable.ModifyJoin
        };
    }
    /// <summary>
    /// 针对 SQL Server 2005 及后续版本数据库的代码生成器。
    /// </summary>
    public class SqlServer2005Generator : SqlServerBaseGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0900;
    }
    /// <summary>
    /// 针对 SQL Server 2008 及后续版本数据库的代码生成器。
    /// </summary>
    public class SqlServer2008Generator : SqlServerBaseGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0A00;
    }
    /// <summary>
    /// 针对 SQL Server 2012 及后续版本数据库的代码生成器。
    /// </summary>
    public class SqlServer2012Generator : SqlServerBaseGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0B00;
    }
}