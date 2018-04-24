// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using Caredev.Mego.Common;
using Caredev.Mego.Resolve.Expressions;
using Caredev.Mego.Resolve.Generators.Fragments;

namespace Caredev.Mego.Resolve.Generators.Implement
{
    /// <summary>
    /// 针对 PostgresSQL 数据库的代码生成器。
    /// </summary>
    public abstract partial class PostgresSQLBaseGenerator : SqlGeneratorBase
    {
        /// <inheritdoc/>
        public override FragmentWriterBase FragmentWriter
        {
            get
            {
                if (_FragmentWriter == null)
                {
                    _FragmentWriter = new PostgresSQLFragmentWriter(this);
                }
                return _FragmentWriter;
            }
        }
        private FragmentWriterBase _FragmentWriter;
        /// <inheritdoc/>
        public override string ProviderName => "Npgsql";
        /// <inheritdoc/>
        public override DbFeature Feature => _Feature;
        private readonly DbFeature _Feature = new DbFeature()
        {
            DefaultSchema = "public",
            MaxParameterCountForOperate = 200,
            MaxParameterCount = 2500,
            Capability = EDbCapable.Schema | EDbCapable.DataDefinition |
                EDbCapable.TableInherit | EDbCapable.TemporaryTable |
                EDbCapable.WindowFunction | EDbCapable.TableValuedFunction |
                EDbCapable.ExternalCompoundStatement | EDbCapable.ExternalLocalVariable |
                EDbCapable.SubQuery | EDbCapable.BatchInsert | EDbCapable.ModifyReturning | EDbCapable.ModifyJoin |
                EDbCapable.Relation | EDbCapable.Identity | EDbCapable.Sequence
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
    /// 针对 PostgresSQL 9.3 数据库的代码生成器。
    /// </summary>
    public class PostgresSQL93Generator : PostgresSQLBaseGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0903;
    }
}