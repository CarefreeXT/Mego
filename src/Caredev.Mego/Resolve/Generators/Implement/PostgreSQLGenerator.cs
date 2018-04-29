// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Contents;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using System.Linq;

    /// <summary>
    /// 针对 PostgreSQL 数据库的代码生成器。
    /// </summary>
    public abstract partial class PostgreSQLBaseGenerator : SqlGeneratorBase
    {
        /// <inheritdoc/>
        public override FragmentWriterBase FragmentWriter
        {
            get
            {
                if (_FragmentWriter == null)
                {
                    _FragmentWriter = new PostgreSQLFragmentWriter(this);
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
        /// <inheritdoc/>
        protected override SqlFragment GenerateForUpdateContent(GenerateContext context, DbExpression content)
        {
            var data = (UpdateContent)context.Data;
            data.SetConcurrencyExpectCount(1);
            RegisterExpressionForCommit(context, content, data.CommitObject);
            UpdateFragment update = null;
            if (data.Items.Count == 1)
            {
                update = data.UpdateByKeys(data.Unit, data.TargetName);
            }
            else
            {
                var values = new ValuesFragment(context, data.CommitObject, data.Items, data.Table);
                RegisterExpressionForCommit(context, content, values);
                update = data.UpdateByTemptable(data.Unit, values, data.TargetName);
            }
            if (data.ReturnMembers.Any())
            {
                foreach (var member in data.ReturnMembers)
                {
                    var metadata = member.Metadata;
                    update.ReturnMembers.Add(update.Target.GetMember(metadata));
                }
                data.GenerateOutput();
            }
            return update;
        }
        /// <inheritdoc/>
        protected override SqlFragment GenerateForDeleteContent(GenerateContext context, DbExpression content)
        {
            var data = (DeleteContent)context.Data;
            data.SetConcurrencyExpectCount(1);
            var metadata = data.Table;
            if (data.Items.Count > 1 && metadata.Keys.Length > 1)
            {
                var values = new ValuesFragment(context, data.CommitObject, data.Items, data.Table);
                var filterMembers = data.UnionConcurrencyMembers(metadata, metadata.Keys);
                var delete = new DeleteFragment(context, metadata);
                var current = delete.Target;
                delete.AddSource(values);
                current.Join(values, filterMembers);
                return delete;
            }
            return base.GenerateForDeleteContent(context, content);
        }
        /// <inheritdoc/>
        protected override SqlFragment GenerateForRelation(GenerateContext context, DbExpression content)
        {
            var data = (RelationContent)context.Data;
            if (data.Items.Count > 1)
            {
                var metadata = data.Items.Navigate;
                if (data.IsAddRelation && !metadata.IsComposite)
                {
                    var columns = data.Source.Keys.Concat(metadata.Pairs.Select(a => a.ForeignKey));
                    var values = new ValuesFragment(context, data.CommitObject, data.Items);
                    data.Source.Keys.ForEach(key => values.SetValue(key));
                    if (data.IsAddRelation)
                    {
                        metadata.Pairs.ForEach(
                            pair => values.SetValue(pair.ForeignKey, data.CommitObject.GetMember(pair.PrincipalKey)));
                    }
                    var update = new UpdateFragment(context, data.Table);
                    update.AddSource(update.Target, values);
                    foreach (var pair in metadata.Pairs)
                    {
                        update.SetValue(pair.ForeignKey, values.GetMember(pair.ForeignKey));
                    }
                    update.Target.Join(values, data.Table.Keys);
                    return update;
                }
                else if (!data.IsAddRelation && metadata.IsComposite)
                {
                    var composite = (CompositeNavigateMetadata)data.Items.Navigate;
                    var values = new ValuesFragment(context, data.CommitObject, data.Items);
                    composite.Pairs.ForEach(pair => values.SetValue(pair.ForeignKey, data.CommitObject.GetMember(pair.PrincipalKey)));
                    composite.CompositePairs.ForEach(pair => values.SetValue(pair.ForeignKey, data.CommitObject.GetMember(pair.PrincipalKey)));
                    var target = new TableFragment(context, data.Table);
                    var delete = new DeleteFragment(context, target);
                    delete.AddSource(values);
                    target.Join(values, composite.Pairs.Select(a => a.ForeignKey).Concat(composite.CompositePairs.Select(a => a.ForeignKey)));
                    return delete;
                }
            }
            return base.GenerateForRelation(context, content);
        }
    }
    /// <summary>
    /// 针对 PostgreSQL 9.3 数据库的代码生成器。
    /// </summary>
    public class PostgreSQL93Generator : PostgreSQLBaseGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0903;
    }
}