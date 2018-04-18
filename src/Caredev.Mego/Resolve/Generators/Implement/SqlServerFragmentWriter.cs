// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections.Generic;
    using System.Text;
    /// <summary>
    /// SQL Server 语句片段写入器。
    /// </summary>
    public partial class SqlServerFragmentWriter : FragmentWriterBase
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
            { typeof(SByte), "TINYINT" }
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
        public SqlServerFragmentWriter(SqlGeneratorBase generator)
            : base(generator)
        {
        }
        /// <inheritdoc/>
        protected override IDictionary<Type, string> InitialClrTypeDefaultMapping()
        {
            var result = new Dictionary<Type, string>(_ClrTypeDbTypeDefaultMapping);
            if (this.Generator.Version >= 0x0A00)
            {
                result.Add(typeof(TimeSpan), "TIME(7)");
                result.Add(typeof(DateTime), "DATETIME2(7)");
                result.Add(typeof(DateTimeOffset), "DATETIMEOFFSET(7)");
            }
            else
            {
                result.Add(typeof(DateTime), "DATETIME");
                result.Add(typeof(DateTimeOffset), "DATETIME");
            }
            return result;
        }
        /// <inheritdoc/>
        protected override IDictionary<Type, string> InitialClrTypeSimpleMapping()
        {
            var result = new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping);
            if (this.Generator.Version < 0x0A00)
            {
                result.Add(typeof(DateTime), "DATETIME");
                result.Add(typeof(DateTimeOffset), "DATETIME");
            }
            return result;
        }
        /// <inheritdoc/>
        protected override IDictionary<Type, Action<SqlWriter, ColumnMetadata>> InitialCustomWriteDbDataTypeMethods()
        {
            var result = base.InitialCustomWriteDbDataTypeMethods();
            if (this.Generator.Version >= 0x0A00)
            {
                result.AddOrUpdate(typeof(TimeSpan), (w, c) => WriteDbDataTypeForScale(w, c, typeof(TimeSpan), "TIME"));
                result.AddOrUpdate(typeof(DateTime), (w, c) => WriteDbDataTypeForScale(w, c, typeof(DateTime), "DATETIME2"));
                result.AddOrUpdate(typeof(DateTimeOffset), (w, c) => WriteDbDataTypeForScale(w, c, typeof(DateTimeOffset), "DATETIMEOFFSET"));
            }
            return result;
        }
        /// <inheritdoc/>
        public override void WriteDbObject(SqlWriter writer, string name, string schema)
        {
            if (!string.IsNullOrEmpty(schema))
            {
                WriteDbName(writer, schema);
            }
            else
            {
                WriteDbName(writer, "dbo");
            }
            writer.Write('.');
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
        public override void WriteDbDataType(SqlWriter writer, ColumnMetadata column)
        {
            var computed = column.GetProperty<ComputedAttribute>();
            if (computed != null)
            {
                writer.Write(" AS ");
                writer.Write(computed.Expression);
                return;
            }
            base.WriteDbDataType(writer, column);
        }
        //写入带精度的数据类型。
        private void WriteDbDataTypeForScale(SqlWriter writer, ColumnMetadata column, Type type, string name)
        {
            var precision = column.GetProperty<PrecisionAttribute>();
            if (precision != null)
            {
                writer.Write(name);
                writer.Write('(');
                writer.Write(precision.Scale);
                writer.Write(')');
            }
            else
            {
                WriteDbDataType(writer, type);
            }
        }
    }
}