// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    /// <summary>
    /// 针对 MySQL 数据库的代码生成器。
    /// </summary>
    public abstract partial class MySqlBaseGenerator : SqlGeneratorBase
    {
        /// <inheritdoc/>
        public override string ProviderName => "MySql.Data.MySqlClient";
        /// <inheritdoc/>
        public override FragmentWriterBase FragmentWriter
        {
            get
            {
                if (_FragmentWriter == null)
                {
                    _FragmentWriter = new MySqlFragmentWriter(this);
                }
                return _FragmentWriter;
            }
        }
        private FragmentWriterBase _FragmentWriter;
        /// <inheritdoc/>
        protected override IExpressionFragment CreateExpressionForBinary(GenerateContext context, DbExpression expression, ISourceFragment source)
        {
            var binary = (DbBinaryExpression)expression;
            if (binary.Kind == EBinaryKind.Add && binary.ClrType == typeof(string))
            {
                return new ScalarFragment(context,
                    source.CreateExpression(binary.Left),
                    source.CreateExpression(binary.Right))
                {
                    Function = SupportMembers.String.Concat
                };
            }
            return base.CreateExpressionForBinary(context, expression, source);
        }
        /// <inheritdoc/>
        public override DbFeature Feature => _Feature;
        private DbFeature _Feature = new DbFeature()
        {
            MaxParameterCountForOperate = 200,
            MaxParameterCount = 2500,
            Capability = EDbCapable.DataDefinition |
                EDbCapable.TemporaryTable |
                EDbCapable.TableValuedFunction |
                EDbCapable.ImplicitDeclareVariable | EDbCapable.ExternalLocalVariable |
                EDbCapable.SubQuery | EDbCapable.BatchInsert | EDbCapable.ModifyJoin |
                EDbCapable.Relation | EDbCapable.Identity
        };
    }
    /// <summary>
    /// 针对 MySQL 5.5 及后续版本数据库的代码生成器。
    /// </summary>
    public partial class MySql55Generator : MySqlBaseGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0505;
    }
    /// <summary>
    /// 针对 MySQL 5.7 及后续版本数据库的代码生成器。
    /// </summary>
    public partial class MySql57Generator : MySqlBaseGenerator
    {
        /// <summary>
        /// 创建生成器。
        /// </summary>
        public MySql57Generator()
        {
            this.Feature.Capability |= EDbCapable.ComputeColumn;
        }
        /// <inheritdoc/>
        public override short Version => 0x0507;
    }
}