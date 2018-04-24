// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// SQLite 语句片段写入器。
    /// </summary>
    public partial class SQLiteFragmentWriter : FragmentWriterBase
    {
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeSimpleMapping = new Dictionary<Type, string>()
        {
            { typeof(bool) , "NUMERIC" },
            { typeof(int) , "INTEGER" },
            { typeof(short) , "INTEGER" },
            { typeof(long) , "INTEGER" },
            { typeof(float) , "REAL" },
            { typeof(double) , "REAL" },
            { typeof(DateTime), "NUMERIC" },
            { typeof(DateTimeOffset), "NUMERIC" },
            { typeof(decimal), "NUMERIC" },
            { typeof(Guid) , "TEXT" },
            { typeof(string), "TEXT" }
        };
        /// <summary>
        /// 创建 SQLite 语句片段写入器。
        /// </summary>
        /// <param name="generator"></param>
        public SQLiteFragmentWriter(SqlGeneratorBase generator)
            : base(generator)
        {
        }
        /// <inheritdoc/>
        protected override IDictionary<Type, string> InitialClrTypeDefaultMapping()
        {
            return new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping);
        }
        /// <inheritdoc/>
        protected override IDictionary<Type, string> InitialClrTypeSimpleMapping()
        {
            return new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping);
        }
        /// <inheritdoc/>
        public override void WriteDbName(SqlWriter writer, string name)
        {
            writer.Write('[');
            writer.Write(name);
            writer.Write(']');
        }
    }
}