// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// SQL Server Compact 语句片段写入器。
    /// </summary>
    public partial class SqlServerCeFragmentWriter : FragmentWriterBase
    {
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeSimpleMapping = new Dictionary<Type, string>()
        {
            { typeof(bool), "BIT" },
            { typeof(int), "INT" },
            { typeof(short), "TINYINT" },
            { typeof(long), "BIGINT" },
            { typeof(float), "REAL" },
            { typeof(double), "FLOAT" },
            { typeof(Guid), "UNIQUEIDENTIFIER" },
            { typeof(SByte), "TINYINT" },
            { typeof(DateTimeOffset), "DATETIME" },
            { typeof(DateTime), "DATETIME" }
        };
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeDefaultMapping = new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping)
        {
            { typeof(string) , "NVARCHAR(MAX)" },
            { typeof(byte[]) , "VARBINARY(MAX)" },
            { typeof(decimal), "MONEY" }
        };
        /// <summary>
        /// 创建 SQL Server 语句片段写入器。
        /// </summary>
        /// <param name="generator">片段生成器。</param>
        public SqlServerCeFragmentWriter(SqlGeneratorBase generator)
            : base(generator)
        {
        }
        /// <inheritdoc/>
        public override void WriteDbObject(SqlWriter writer, string name, string schema)
        {
            WriteDbName(writer, name);
        }
        /// <inheritdoc/>
        public override void WriteDbName(SqlWriter writer, string name)
        {
            writer.Write('[');
            writer.Write(name);
            writer.Write(']');
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