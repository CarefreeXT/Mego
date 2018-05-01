// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// SQL Server Compact 语句片段写入器。
    /// </summary>
    public partial class SqlServerCeFragmentWriter : MSLocalDbFragmentWriter
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
            { typeof(string) , "NVARCHAR(4000)" },
            { typeof(byte[]) , "IMAGE" },
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