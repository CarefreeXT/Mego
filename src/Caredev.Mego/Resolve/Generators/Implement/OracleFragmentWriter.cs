// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Oracle 语句片段写入器。
    /// </summary>
    public partial class OracleFragmentWriter : FragmentWriterBase
    {
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeSimpleMapping = new Dictionary<Type, string>()
        {
            { typeof(bool), "BIT" },
            { typeof(int), "INTEGER" },
            { typeof(short), "SMALLINT" },
            { typeof(long), "LONG" },
            { typeof(float), "REAL" },
            { typeof(double), "FLOAT" },
            { typeof(Guid), "CHAR(36)" },
            { typeof(SByte), "TINYINT" }
        };
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeDefaultMapping = new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping)
        {
            { typeof(string) , "NVARCHAR2(MAX)" },
            { typeof(byte[]) , "BLOB" },
            { typeof(decimal), "NUMERIC(18,4)" },
            { typeof(DateTime), "TIMESTAMP" },
            { typeof(DateTimeOffset), "TIMESTAMP" }
        };
        /// <summary>
        /// 创建 SQL Server 语句片段写入器。
        /// </summary>
        /// <param name="generator">片段生成器。</param>
        public OracleFragmentWriter(SqlGeneratorBase generator)
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
        public override void WriteDbObject(SqlWriter writer, string name, string schema)
        {
            if (!string.IsNullOrEmpty(schema))
            {
                WriteDbName(writer, schema);
                writer.Write('.');
            }
            WriteDbName(writer, name);
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
    }
}