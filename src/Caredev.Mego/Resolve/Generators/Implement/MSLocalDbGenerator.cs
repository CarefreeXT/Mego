// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using System.Linq;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Contents;
    using Caredev.Mego.Resolve.Generators.Fragments;
    /// <summary>
    /// 针对 Microsoft 本地数据库的代码生成器。
    /// </summary>
    public abstract class MSLocalDbGenerator : SqlGeneratorBase
    {
        /// <inheritdoc/>
        protected override SqlFragment GenerateForInsertContent(GenerateContext context, DbExpression content)
        {
            var data = (InsertContent)context.Data;
            SqlFragment result = null;
            RegisterExpressionForCommit(context, content, data.CommitObject);
            if (data.Unit is CommitKeyUnit keyunit)
            {
                result = base.GenerateForInsertSimple(context, data);
            }
            else
            {
                result = base.GenerateForInsertIdentitySingle(context, data);
            }
            InitialCommitObject(data);
            return result;
        }
        /// <inheritdoc/>
        protected override SqlFragment GenerateForUpdateContent(GenerateContext context, DbExpression content)
        {
            var data = (UpdateContent)context.Data;
            data.SetConcurrencyExpectCount(1);
            RegisterExpressionForCommit(context, content, data.CommitObject);
            var udpate = base.GenerateForUpdateSingle(context, data);
            InitialCommitObject(data);
            return udpate;
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
            SqlFragment result = null;
            if (!metadata.IsComposite)
            {
                result = GenerateForRelationUpdate(context);
            }
            else
            {
                if (data.IsAddRelation)
                {
                    result = GenerateForRelationInsert(context);
                }
                else
                {
                    result = GenerateForRelationDelete(context);
                }
            }
            foreach (var member in data.CommitObject.Members.OfType<CommitMemberFragment>())
            {
                if (metadata.Target.ClrType == member.Property.DeclaringType)
                {
                    member.Metadata = metadata.Target.Members[member.Property.Name];
                }
                else
                {
                    member.Metadata = metadata.Source.Members[member.Property.Name];
                }
            }
            return result;
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
}