// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using Caredev.Mego.Common;
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成关系操作语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>生成语句片段。</returns>
        protected virtual SqlFragment GenerateForRelation(GenerateContext context)
        {
            var data = (GenerateDataForRelation)context.Data;
            var metadata = data.Items.Navigate;
            if (!metadata.IsComposite)
            {
                if (data.Items.Count == 1)
                {
                    return GenerateForRelationUpdate(context);
                }
                else if (!data.IsAddRelation && data.Table.Keys.Length == 1)
                {
                    return GenerateForRelationDeleteKey(context);
                }
                else if (context.Feature.HasCapable(EDbCapable.TemporaryTable | EDbCapable.ModifyJoin))
                {
                    return GenerateForRelationUpdateTempTable(context);
                }
                else
                {
                    return GenerateForRelationUpdateRepeat(context);
                }
            }
            else
            {
                if (data.IsAddRelation)
                {
                    return GenerateForRelationInsert(context);
                }
                else
                {
                    if (data.Items.Count == 1)
                    {
                        return GenerateForRelationDelete(context);
                    }
                    else if (context.Feature.HasCapable(EDbCapable.TemporaryTable | EDbCapable.ModifyJoin))
                    {
                        return GenerateForRelationDeleteTempTable(context);
                    }
                    else
                    {
                        return GenerateForRelationDeleteRepeat(context);
                    }
                }
            }
        }
        /// <summary>
        /// 生成添加若干复合关系对象语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForRelationInsert(GenerateContext context)
        {
            var data = (GenerateDataForRelation)context.Data;
            var metadata = (CompositeNavigateMetadata)data.Items.Navigate;
            var target = new TableFragment(context, data.Table);
            var insert = new InsertValueFragment(context, target, data.CommitObject, data.Items);
            metadata.Pairs.ForEach(pair => insert.SetValue(pair.ForeignKey, data.CommitObject.GetMember(pair.PrincipalKey)));
            metadata.CompositePairs.ForEach(pair => insert.SetValue(pair.ForeignKey, data.CommitObject.GetMember(pair.PrincipalKey)));
            return insert;
        }
        /// <summary>
        /// 生成更新单个关系对象语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForRelationUpdate(GenerateContext context)
        {
            var data = (GenerateDataForRelation)context.Data;
            var metadata = data.Items.Navigate;
            var update = new UpdateFragment(context, data.Table);
            update.AddSource(update.Target);
            if (data.IsAddRelation)
            {
                var source = data.CommitObject;
                foreach (var pair in metadata.Pairs)
                {
                    update.SetValue(pair.ForeignKey, source.GetMember(pair.PrincipalKey));
                }
            }
            else
            {
                var nullfargment = new SimpleFragment(context, "NULL");
                foreach (var pair in metadata.Pairs)
                {
                    update.SetValue(pair.ForeignKey, nullfargment);
                }
            }
            update.Where = update.Target.JoinCondition(data.CommitObject, data.Table.Keys);
            return update;
        }
        /// <summary>
        /// 生成更新若干关系对象（单个主键）语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForRelationDeleteKey(GenerateContext context)
        {
            var data = (GenerateDataForRelation)context.Data;
            var metadata = data.Items.Navigate;
            var update = new UpdateFragment(context, data.Table);
            update.AddSource(update.Target);
            metadata.Pairs.ForEach(
                pair => update.SetValue(pair.ForeignKey, new SimpleFragment(context, "NULL")));
            var member = new CommitMemberFragment(context, data.Loader, update, data.Table.Keys[0].Member);
            var values = new ValueListFragment(context, member, data.Items);
            var keyMember = update.Target.GetMember(data.Table.Keys[0]);
            update.Where = new ScalarFragment(context, values, keyMember)
            {
                Function = SupportMembers.Enumerable.Contains
            };
            return update;
        }
        /// <summary>
        /// 使用重复生成的方式，生成更新多个关系的语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForRelationUpdateRepeat(GenerateContext context)
        {
            var data = (GenerateDataForRelation)context.Data;
            var repeat = new RepeatBlockFragment(context, data.Items, data.CommitObject.Loader);
            repeat.Block.Add(GenerateForRelationUpdate(context));
            return repeat;
        }
        /// <summary>
        /// 使用临时表形式，生成更新关系语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForRelationUpdateTempTable(GenerateContext context)
        {
            var data = (GenerateDataForRelation)context.Data;
            var metadata = data.Items.Navigate;
            var block = new BlockFragment(context);
            var createtemptable = new CreateTempTableFragment(context,
                data.IsAddRelation ? data.Source.Keys.Concat(metadata.Pairs.Select(a => a.ForeignKey)) : data.Source.Keys);
            var insert = new InsertValueFragment(context, createtemptable.Table, data.CommitObject, data.Items);
            data.Source.Keys.ForEach(key => insert.SetValue(key));
            if (data.IsAddRelation)
            {
                metadata.Pairs.ForEach(
                    pair => insert.SetValue(pair.ForeignKey, data.CommitObject.GetMember(pair.PrincipalKey)));
            }
            var temptable = createtemptable.Table;
            var update = new UpdateFragment(context, data.Table);
            update.AddSource(update.Target, temptable);
            if (data.IsAddRelation)
            {
                foreach (var pair in metadata.Pairs)
                {
                    update.SetValue(pair.ForeignKey, temptable.GetMember(pair.ForeignKey));
                }
            }
            else
            {
                var nullfargment = new SimpleFragment(context, "NULL");
                foreach (var pair in metadata.Pairs)
                {
                    update.SetValue(pair.ForeignKey, nullfargment);
                }
            }
            update.Target.Join(temptable, data.Table.Keys);
            return new BlockFragment(context, createtemptable, insert, update);
        }
        /// <summary>
        /// 生成删除单个复合关系对象语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForRelationDelete(GenerateContext context)
        {
            var data = (GenerateDataForRelation)context.Data;
            var metadata = (CompositeNavigateMetadata)data.Items.Navigate;
            var target = new TableFragment(context, data.Table);
            var source = data.CommitObject;
            var delete = new DeleteFragment(context, target);

            delete.Where = metadata.Pairs.Concat(metadata.CompositePairs).Select(
                pair => new BinaryFragment(context, Expressions.EBinaryKind.Equal)
                {
                    Left = target.GetMember(pair.ForeignKey),
                    Right = source.GetMember(pair.PrincipalKey)
                }).Merge();
            return delete;
        }
        /// <summary>
        /// 使用重复生成的方式，生成删除多个复合关系对象的语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForRelationDeleteRepeat(GenerateContext context)
        {
            var data = (GenerateDataForRelation)context.Data;
            var repeat = new RepeatBlockFragment(context, data.Items, data.CommitObject.Loader);
            repeat.Block.Add(GenerateForRelationDelete(context));
            return repeat;
        }
        /// <summary>
        /// 使用临时表形式，生成删除多个复合关系的语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForRelationDeleteTempTable(GenerateContext context)
        {
            var data = (GenerateDataForRelation)context.Data;
            var composite = (CompositeNavigateMetadata)data.Items.Navigate;
            var createtemptable = new CreateTempTableFragment(context,
                composite.Pairs.Select(a => a.ForeignKey).Concat(composite.CompositePairs.Select(a => a.ForeignKey)));

            var insert = new InsertValueFragment(context, createtemptable.Table, data.CommitObject, data.Items);
            composite.Pairs.ForEach(pair => insert.SetValue(pair.ForeignKey, data.CommitObject.GetMember(pair.PrincipalKey)));
            composite.CompositePairs.ForEach(pair => insert.SetValue(pair.ForeignKey, data.CommitObject.GetMember(pair.PrincipalKey)));

            var temptable = createtemptable.Table;
            var target = new TableFragment(context, data.Table);
            var delete = new DeleteFragment(context, target);
            delete.AddSource(temptable);
            target.Join(temptable, composite.Pairs.Select(a => a.ForeignKey).Concat(composite.CompositePairs.Select(a => a.ForeignKey)));
            return new BlockFragment(context, createtemptable, insert, delete);
        }
    }
}