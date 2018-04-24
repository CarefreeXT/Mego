// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Firebird 语句片段写入器。
    /// </summary>
    public partial class FirebirdFragmentWriter : FragmentWriterBase
    {
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeSimpleMapping = new Dictionary<Type, string>()
        {
            { typeof(bool), "SMALLINT" },
            { typeof(int), "INTEGER" },
            { typeof(short), "SMALLINT" },
            { typeof(long), "BIGINT" },
            { typeof(float), "REAL" },
            { typeof(double), "DOUBLE PRECISION" },
            { typeof(Guid), "CHAR(36)" },
            { typeof(SByte), "SMALLINT" },
            { typeof(TimeSpan), "TIME" },
            { typeof(DateTime), "TIMESTAMP" },
            { typeof(DateTimeOffset), "TIMESTAMP" }
        };
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeDefaultMapping = new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping)
        {
            { typeof(string) , "BLOB SUB_TYPE 1" },
            { typeof(byte[]) , "BLOB" },
            { typeof(decimal), "DECIMAL" }
        };
        /// <summary>
        /// 创建 SQL Server 语句片段写入器。
        /// </summary>
        /// <param name="generator">片段生成器。</param>
        public FirebirdFragmentWriter(SqlGeneratorBase generator)
            : base(generator)
        {
        }
        /// <inheritdoc/>
        public override void WriteDbName(SqlWriter writer, string name)
        {
            writer.Write('\"');
            writer.Write(name);
            writer.Write('\"');
        }
        /// <inheritdoc/>
        public override void WriteParameterName(SqlWriter writer, string name)
        {
            writer.Write(':');
            writer.Write(name);
        }
        /// <inheritdoc/>
        protected override IDictionary<Type, string> InitialClrTypeDefaultMapping()
        {
            return new Dictionary<Type, string>(_ClrTypeDbTypeDefaultMapping);
        }
        /// <inheritdoc/>
        protected override IDictionary<Type, string> InitialClrTypeSimpleMapping()
        {
            return new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForBlock(SqlWriter writer, ISqlFragment fragment)
        {
            writer.WriteLine("EXECUTE BLOCK AS ");
            base.WriteFragmentForBlock(writer, fragment);
            writer.WriteLine();
            writer.Write(" END");
        }
    }
}