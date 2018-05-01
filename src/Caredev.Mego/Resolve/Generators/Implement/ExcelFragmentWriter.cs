// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Excel 语句片段写入器。
    /// </summary>
    public partial class ExcelFragmentWriter : MSLocalDbFragmentWriter
    {
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeSimpleMapping = new Dictionary<Type, string>()
        {
            { typeof(byte) , "BYTE" },
            { typeof(int) , "INTEGER" },
            { typeof(short) , "SHORT" },
            { typeof(long) , "LONG" },
            { typeof(float) , "SINGLE" },
            { typeof(double) , "DOUBLE" },
            { typeof(bool) , "BIT" },
            { typeof(Guid) , "GUID" },
            { typeof(DateTime), "DATETIME" },
            { typeof(DateTimeOffset), "DATETIME" },
            { typeof(string) , "LONGTEXT" },
            { typeof(byte[]) , "LONGBINARY" },
        };
        private readonly static IDictionary<Type, string> _ClrTypeDbTypeDefaultMapping = new Dictionary<Type, string>(_ClrTypeDbTypeSimpleMapping)
        {
            { typeof(decimal), "CURRENCY" }
        };
        /// <summary>
        /// 创建 SQL Server 语句片段写入器。
        /// </summary>
        /// <param name="generator">片段生成器。</param>
        public ExcelFragmentWriter(SqlGeneratorBase generator)
            : base(generator)
        {
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