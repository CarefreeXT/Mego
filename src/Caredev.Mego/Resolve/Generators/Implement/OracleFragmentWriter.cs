// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using System;
    using System.Collections.Generic;
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;

    /// <summary>
    /// Oracle 语句片段写入器。
    /// </summary>
    public partial class OracleFragmentWriter : FragmentWriterBase
    {
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeSimpleMapping = new Dictionary<Type, string>()
        {
            { typeof(bool), "NUMBER(1)" },
            { typeof(int), "NUMBER" },
            { typeof(short), "NUMBER" },
            { typeof(long), "NUMBER" },
            { typeof(float), "BINARY_FLOAT" },
            { typeof(double), "BINARY_DOUBLE" },
            { typeof(Guid), "CHAR(36)" },
            { typeof(SByte), "TINYINT" },
            { typeof(DateTime), "TIMESTAMP" },
            { typeof(DateTimeOffset), "TIMESTAMP WITH TIME ZONE" }
        };
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeDefaultMapping = new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping)
        {
            { typeof(string) , "NCLOB" },
            { typeof(byte[]) , "BLOB" },
            { typeof(decimal), "NUMERIC(18,4)" },
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
        protected override void WriteDbDataTypeForString(SqlWriter writer, ColumnMetadata column)
        {
            var strattr = column.GetProperty<StringAttribute>();
            if (strattr == null)
            {
                WriteDbDataType(writer, typeof(string));
            }
            else
            {
                if (strattr.IsUnicode) writer.Write('N');
                if (strattr.Length > 0)
                {
                    if (strattr.IsFixed)
                    {
                        writer.Write("CHAR");
                    }
                    else
                    {
                        writer.Write("VARCHAR2");
                    }
                    writer.Write('(');
                    writer.Write(strattr.Length);
                    writer.Write(')');
                }
                else
                {
                    writer.Write("CLOB");
                }
            }
        }
        /// <inheritdoc/>
        public override void WriteTerminator(SqlWriter writer, ISqlFragment fragment)
        {
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForBlock(SqlWriter writer, ISqlFragment fragment)
        {
            writer.WriteLine("BEGIN");
            var block = (BlockFragment)fragment;
            block.ForEach(writer.WriteLine, a =>
            {
                a.WriteSql(writer);
                if (a.HasTerminator)
                {
                    writer.Write(';');
                }
            });
            writer.WriteLine();
            writer.Write("END;");
        }
    }
}