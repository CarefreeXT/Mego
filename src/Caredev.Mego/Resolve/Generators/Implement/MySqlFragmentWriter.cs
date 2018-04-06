// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using System.Linq;
    using Caredev.Mego.Resolve.Metadatas;
    /// <summary>
    /// MySQL 语句片段写入器。
    /// </summary>
    public partial class MySqlFragmentWriter : FragmentWriterBase
    {
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeSimpleMapping = new Dictionary<Type, string>()
        {
            { typeof(bool) , "BIT(1)" },
            { typeof(int) , "INT" },
            { typeof(short) , "TINYINT" },
            { typeof(long) , "BIGINT" },
            { typeof(float) , "REAL" },
            { typeof(double) , "FLOAT" },
            { typeof(Guid) , "CHAR(36)" },
            { typeof(DateTime), "DATETIME" },
            { typeof(DateTimeOffset), "DATETIME" },
        };
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeDefaultMapping = new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping)
        {
            { typeof(string) , "LONGTEXT" },
            { typeof(byte[]) , "LONGBLOB" },
            { typeof(decimal), "DECIMAL(18,2)" }
        };
        /// <summary>
        /// 创建 MySQL 语句片段写入器。
        /// </summary>
        /// <param name="generator"></param>
        public MySqlFragmentWriter(MySqlBaseGenerator generator)
            : base(generator) { }
        [Obsolete]
        internal bool SupportComputedColumn { get; set; } = false;
        /// <inheritdoc/>
        public override void WriteDbObject(SqlWriter writer, string name, string schema)
        {
            WriteDbName(writer, name);
        }
        /// <inheritdoc/>
        public override void WriteDbName(SqlWriter writer, string name)
        {
            writer.Write('`');
            writer.Write(name);
            writer.Write('`');
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