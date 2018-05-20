// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Collections.Generic;
    using System.Text;
    /// <summary>
    /// 针对 SQLite 数据库的代码生成器。
    /// </summary>
    public abstract class SQLiteGenerator : SqlGeneratorBase
    {
        /// <inheritdoc/>
        public override string ProviderName => "System.Data.SQLite";
        /// <inheritdoc/>
        public override FragmentWriterBase FragmentWriter
        {
            get
            {
                if (_FragmentWriter == null)
                {
                    _FragmentWriter = new SQLiteFragmentWriter(this);
                }
                return _FragmentWriter;
            }
        }
        private FragmentWriterBase _FragmentWriter;
        /// <inheritdoc/>
        public override DbFeature Feature => _Feature;
        private DbFeature _Feature = new DbFeature()
        {
            DefaultSchema = string.Empty,
            MaxInsertRowCount = 500,
            MaxParameterCount = 999,
            Capability = EDbCapable.DataDefinition |
                EDbCapable.TemporaryTable |
                EDbCapable.BatchInsert | EDbCapable.SubQuery |
                EDbCapable.Identity
        };
        /// <inheritdoc/>
        protected override IExpressionFragment CreateExpressionForBinary(GenerateContext context, DbExpression expression, ISourceFragment source)
        {
            var binary = (DbBinaryExpression)expression;
            if (binary.Kind == EBinaryKind.Add && binary.ClrType == typeof(string))
            {
                return new StringConcatFragment(context,
                    source.CreateExpression(binary.Left),
                    source.CreateExpression(binary.Right));
            }
            return base.CreateExpressionForBinary(context, expression, source);
        }
    }
    /// <summary>
    /// 针对 SQLite 3 及后续版本数据库的代码生成器。
    /// </summary>
    public class SQLite3Generator : SQLiteGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0300;
    }
}