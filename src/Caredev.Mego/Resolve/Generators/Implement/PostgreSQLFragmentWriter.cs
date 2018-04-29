// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using System;
    using System.Collections.Generic;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Metadatas;

    /// <summary>
    /// PostgreSQL 语句片段写入器。
    /// </summary>
    public partial class PostgreSQLFragmentWriter : FragmentWriterBase
    {
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeSimpleMapping = new Dictionary<Type, string>()
        {
            { typeof(bool), "BOOLEAN" },
            { typeof(int), "INTEGER" },
            { typeof(short), "SMALLINT" },
            { typeof(long), "BIGINT" },
            { typeof(float), "REAL" },
            { typeof(double), "DOUBLE" },
            { typeof(Guid), "UUID" },
            { typeof(SByte), "SMALLINT" },
            { typeof(DateTime), "TIMESTAMP" },
            { typeof(DateTimeOffset), "TIMESTAMP" }
        };
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeDefaultMapping = new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping)
        {
            { typeof(string) , "TEXT" },
            { typeof(byte[]) , "BYTEA" },
            { typeof(decimal), "MONEY" }
        };
        /// <summary>
        /// 创建 SQL Server 语句片段写入器。
        /// </summary>
        /// <param name="generator">片段生成器。</param>
        public PostgreSQLFragmentWriter(SqlGeneratorBase generator)
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
        public override void WriteDbDataType(SqlWriter writer, ColumnMetadata column)
        {
            var identity = column.GetProperty<IdentityAttribute>();
            if (identity != null)
            {
                var type = column.Member.PropertyType;
                if (type == typeof(int) || type == typeof(int?))
                {
                    writer.Write("SERIAL");
                }
                else if (type == typeof(short) || type == typeof(short?))
                {
                    writer.Write("SMALLSERIAL");
                }
                else if (type == typeof(long) || type == typeof(long?))
                {
                    writer.Write("BIGSERIAL");
                }
                else
                {
                    base.WriteDbDataType(writer, column);
                }
            }
            else
            {
                base.WriteDbDataType(writer, column);
            }
        }
        /// <inheritdoc/>
        protected override void WriteDbDataTypeForString(SqlWriter writer, ColumnMetadata column)
        {
            var strattr = column.GetProperty<StringAttribute>();
            if (strattr == null)
            {
                WriteDbDataType(writer, typeof(string));
            }
            else
            {
                if (strattr.Length > 0)
                {
                    if (!strattr.IsFixed) writer.Write("VAR");
                    writer.Write("CHAR(");
                    writer.Write(strattr.Length);
                    writer.Write(')');
                }
                else
                {
                    WriteDbDataType(writer, typeof(string));
                }
            }
        }
    }
}