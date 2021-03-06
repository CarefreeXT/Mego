﻿// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Resolve.Commands;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Contents;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Providers;
    using System.Linq;
    /// <summary>
    /// 针对 Oracle 数据库的代码生成器。
    /// </summary>
    public abstract class OracleBaseGenerator : SqlGeneratorBase
    {
        /// <inheritdoc/>
        public override FragmentWriterBase FragmentWriter
        {
            get
            {
                if (_FragmentWriter == null)
                {
                    _FragmentWriter = new OracleFragmentWriter(this);
                }
                return _FragmentWriter;
            }
        }
        private FragmentWriterBase _FragmentWriter;
        /// <inheritdoc/>
        public override string ProviderName => "Oracle.ManagedDataAccess.Client";
        /// <inheritdoc/>
        public override DbFeature Feature => _Feature;
        private readonly DbFeature _Feature = new DbFeature()
        {
            DefaultSchema = "system",
            MaxIdentifierLength = 30,
            Capability = EDbCapable.Schema | EDbCapable.DataDefinition |
                EDbCapable.WindowFunction | EDbCapable.TableValuedFunction |
                EDbCapable.ExternalCompoundStatement | EDbCapable.ExternalLocalVariable |
                EDbCapable.SubQuery | EDbCapable.BatchInsert | EDbCapable.ModifyJoin |
                EDbCapable.Relation | EDbCapable.Sequence
        };
        /// <inheritdoc/>
        protected override SqlFragment GenerateForInsertContent(GenerateContext context, DbExpression content)
        {
            var data = (InsertContent)context.Data;
            RegisterExpressionForCommit(context, content, data.CommitObject);
            var insert = data.InsertKeyUnit((CommitKeyUnit)data.Unit, data.TargetName);
            foreach (var member in data.ReturnMembers)
            {
                var metadata = member.Metadata;
                insert.ReturnMembers.Add(data.CommitObject.GetMember(metadata));
            }
            data.GenerateOutput();
            InitialCommitObject(data);
            return insert;
        }
        /// <inheritdoc/>
        protected override SqlFragment GenerateForUpdateContent(GenerateContext context, DbExpression content)
        {
            var data = (UpdateContent)context.Data;
            data.SetConcurrencyExpectCount(1);
            RegisterExpressionForCommit(context, content, data.CommitObject);
            var update = data.UpdateByKeys(data.Unit, data.TargetName);
            foreach (var member in data.ReturnMembers)
            {
                var metadata = member.Metadata;
                update.ReturnMembers.Add(data.CommitObject.GetMember(metadata));
            }
            data.GenerateOutput();
            InitialCommitObject(data);
            return update;
        }
        /// <inheritdoc/>
        protected override SqlFragment GenerateForDeleteContent(GenerateContext context, DbExpression content)
        {
            var data = (DeleteContent)context.Data;
            data.SetConcurrencyExpectCount(1);
            var delete = data.DeleteByKeys(data.Table, data.TargetName);
            InitialCommitObject(data);
            return delete;
        }
        /// <inheritdoc/>
        protected override SqlFragment GenerateForRelation(GenerateContext context, DbExpression content)
        {
            var data = (RelationContent)context.Data;
            var metadata = data.Items.Navigate;
            if (!metadata.IsComposite)
            {
                return GenerateForRelationUpdate(context);
            }
            else
            {
                if (data.IsAddRelation)
                {
                    return GenerateForRelationInsert(context);
                }
                else
                {
                    return GenerateForRelationDelete(context);
                }
            }
        }
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
        private void InitialCommitObject(ContentBase data)
        {
            var commitObject = data.CommitObject;
            var table = data.Table;
            foreach (var member in commitObject.Members.OfType<CommitMemberFragment>())
            {
                member.Metadata = table.Members[member.Property.Name];
            }
        }
    }
    /// <summary>
    /// Oracle 11g 生成器。
    /// </summary>
    public class Oracle11Generator : OracleBaseGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0B00;
    }
}
