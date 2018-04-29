// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Outputs;
    using Caredev.Mego.Resolve.ValueGenerates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Res = Properties.Resources;
    internal static class ContentExtensionMethods
    {
        /// <summary>
        /// 创建通过主键删除的语句片段。
        /// </summary>
        /// <param name="data">内容对象。</param>
        /// <param name="table">删除表的元数据。</param>
        /// <param name="name">指定名称。</param>
        /// <returns>语句片段。</returns>
        internal static DeleteFragment DeleteByKeys(this ContentBase data, TableMetadata table, DbName name = null)
        {
            var delete = new DeleteFragment(data.GenerateContext, table, name);
            delete.Where = delete.Target.JoinCondition(data.CommitObject,
                data.UnionConcurrencyMembers(table, table.Keys));
            return delete;
        }
        /// <summary>
        /// 创建通过 IN 主键删除的语句片段。
        /// </summary>
        /// <param name="data">内容对象。</param>
        /// <param name="table">删除表的元数据。</param>
        /// <param name="key">主键元数据。</param>
        /// <param name="values">值列表对象。</param>
        /// <param name="name">指定名称。</param>
        /// <returns>语句片段。</returns>
        internal static DeleteFragment DeleteInKey(this ContentBase data, TableMetadata table
            , ColumnMetadata key, ValueListFragment values, DbName name = null)
        {
            var context = data.GenerateContext;
            var delete = new DeleteFragment(context, table, data.TargetName);
            var keyMember = delete.Target.GetMember(key);
            delete.Where = new ScalarFragment(context, values, keyMember)
            {
                Function = SupportMembers.Enumerable.Contains
            };
            return delete;
        }
        /// <summary>
        /// 创建通过临时博删除的语句片段。
        /// </summary>
        /// <param name="data">内容对象。</param>
        /// <param name="metadata">删除表的元数据。</param>
        /// <param name="temptable">临时表。</param>
        /// <param name="name">指定名称。</param>
        /// <returns>语句片段。</returns>
        internal static DeleteFragment DeleteByTemptable(this ContentBase data
            , TableMetadata metadata, TemporaryTableFragment temptable, DbName name = null)
        {
            var context = data.GenerateContext;
            var filterMembers = data.UnionConcurrencyMembers(metadata, metadata.Keys);
            var delete = new DeleteFragment(context, metadata);
            var current = delete.Target;
            var currenttemptable = new TemporaryTableFragment(context, filterMembers, temptable.Name);
            delete.AddSource(currenttemptable);
            current.Join(currenttemptable, filterMembers);
            return delete;
        }

        internal static UpdateFragment UpdateByKeys(this ContentBase data, CommitUnitBase unit, DbName name = null)
        {
            var context = data.GenerateContext;
            var metadata = unit.Table;
            var update = new UpdateFragment(context, metadata, name);
            data.CommitObject.Parent = update;
            data.SetCommitMembers(update, unit);
            update.Where = update.Target.JoinCondition(data.CommitObject, data.UnionConcurrencyMembers(metadata, metadata.Keys));
            return update;
        }

        internal static UpdateFragment UpdateByTemptable(this ContentBase data, CommitUnitBase unit, SourceFragment current, DbName name = null)
        {
            var context = data.GenerateContext;
            var metadata = unit.Table;
            var update = new UpdateFragment(context, metadata, name);
            update.AddSource(update.Target, current);
            data.SetCommitMembers(update, unit, current);
            update.Target.Join(current, data.UnionConcurrencyMembers(metadata, metadata.Keys));
            return update;
        }

        internal static InsertValueFragment InsertKeyUnit(this ContentBase data, CommitKeyUnit unit, DbName name = null)
        {
            var context = data.GenerateContext;
            var target = new TableFragment(context, unit.Table, name);
            var insert = new InsertValueFragment(context, target, data.CommitObject, data.Items);
            data.CommitObject.Parent = insert;
            unit.Keys.Concat(unit.Members).ForEach(member => SetCommitMember(insert, member));
            return insert;
        }

        internal static InsertValueFragment InsertUnit(this ContentBase data, CommitIdentityUnit unit, DbName name = null)
        {
            var context = data.GenerateContext;
            var target = new TableFragment(context, unit.Table, name);
            var insert = new InsertValueFragment(context, target, data.CommitObject, data.Items);
            data.CommitObject.Parent = insert;
            unit.Members.ForEach(member => SetCommitMember(insert, member));
            return insert;
        }


        internal static InsertValueFragment InsertTemptable(this ContentBase data, TemporaryTableFragment temptable, IEnumerable<CommitMember> members)
        {
            var context = data.GenerateContext;
            var insert = new InsertValueFragment(context, temptable, data.CommitObject, data.Items);
            data.CommitObject.Parent = insert;
            members.ForEach(member => SetCommitMember(insert, member));
            return insert;
        }

        internal static InsertFragment InsertTemptable(this ContentBase data, TemporaryTableFragment temptable, CommitKeyUnit unit, DbName name = null)
        {
            var members = unit.Keys.Concat(unit.Members).Select(a => a.Metadata);
            var context = data.GenerateContext;
            var table = new TableFragment(context, unit.Table, name);
            var select = new SelectFragment(context, temptable);
            var current = new InsertFragment(context, table, select);
            foreach (var member in members)
            {
                select.CreateMember(null, temptable.GetMember(member));
                current.CreateMember(null, member);
            }
            return current;
        }
        /// <summary>
        /// 创建通过主键及值关联的返回数据语句片段。
        /// </summary>
        /// <param name="unit">内容对象。</param>
        /// <param name="target">目标对象。</param>
        /// <returns>语句片段。</returns>
        internal static SelectFragment SelectReturns(this IContentUnit unit, ISourceFragment target)
        {
            var data = (ContentBase)unit;
            var context = data.GenerateContext;
            var select = new SelectFragment(context, target);
            select.Members.AddRange(unit.ReturnMembers.Select(a => target.GetMember(a.Metadata)));
            select.Where = target.JoinCondition(data.CommitObject, data.Table.Keys);
            return select;
        }
        /// <summary>
        /// 创建通过临时表关联的返回数据语句片段。
        /// </summary>
        /// <param name="unit">内容对象。</param>
        /// <param name="target">目标对象。</param>
        /// <param name="temptable">临时表。</param>
        /// <returns>语句片段。</returns>
        internal static SelectFragment SelectReturns(this IContentUnit unit, ISourceFragment target, TemporaryTableFragment temptable)
        {
            var data = (ContentBase)unit;
            var context = data.GenerateContext;
            var select = new SelectFragment(context, target);
            select.Members.AddRange(unit.ReturnMembers.Select(a => target.GetMember(a.Metadata)));

            select.AddSource(temptable);
            target.Join(temptable, data.Table.Keys);
            return select;
        }

        internal static void GenerateOutput(this IContentUnit unit)
        {
            if (unit.ReturnMembers.Any())
            {
                var data = (ContentBase)unit;
                var returns = unit.ReturnMembers.Select(a => a.Metadata.Member).ToArray();
                var metadata = data.GenerateContext.Metadata.Type(data.Items.ClrType);
                var fields = Utility.Array<int>(metadata.PrimaryMembers.Count, -1);
                var output = new CollectionOutputInfo(metadata, fields);
                for (int i = 0; i < metadata.PrimaryMembers.Count; i++)
                {
                    var member = metadata.PrimaryMembers[i];
                    fields[i] = Array.IndexOf(returns, member.Member);
                }
                data.Items.Output = output;
            }
        }

        private static void SetCommitMembers(this ContentBase data, UpdateFragment update, CommitUnitBase unit, ISourceFragment source = null)
        {
            source = source ?? data.CommitObject;
            foreach (var member in unit.Members)
            {
                switch (member.ValueType)
                {
                    case ECommitValueType.Constant:
                        update.SetValue(member.Metadata, source.GetMember(member.Metadata));
                        break;
                    case ECommitValueType.Expression:
                        var exp = (CommitExpressionMember)member;
                        update.SetValue(member.Metadata, update.CreateExpression(exp.Expression));
                        break;
                }
            }
        }
        private static void SetCommitMember(this InsertValueFragment insert, CommitMember member)
        {
            switch (member.ValueType)
            {
                case ECommitValueType.Constant:
                    insert.SetValue(member.Metadata);
                    break;
                case ECommitValueType.Expression:
                    var exp = (CommitExpressionMember)member;
                    insert.SetValue(member.Metadata, insert.CreateExpression(exp.Expression));
                    break;
            }
        }
        /// <summary>
        /// 根据属性映射表达式创建提交单元。
        /// </summary>
        /// <param name="data">数据对象。</param>
        /// <param name="commitUnit">当前提交单元。</param>
        /// <param name="columns">指定的列元数据集合。</param>
        /// <param name="content">属性映射表达式。</param>
        /// <returns>当前提交单元。</returns>
        public static CommitUnitBase CreateCommitUnit(this IContentUnit data, CommitUnitBase commitUnit, IEnumerable<ColumnMetadata> columns, DbExpression content)
        {
            if (content is DbSelectExpression select && select.Item is DbNewExpression newExpression)
            {
                foreach (var column in columns)
                {
                    var generator = data.GetValueGenerator(column);
                    if (newExpression.Members.TryGetValue(column.Member, out DbExpression expression))
                    {
                        if (expression is DbMemberExpression memberAccess && memberAccess.Member == column.Member
                               && memberAccess.Expression == select.Source.Item)
                        {
                            commitUnit.Add(data.CreateCommitUnitMember(column, generator));
                        }
                        else
                        {
                            commitUnit.Add(data.CreateCommitUnitMember(column, generator, expression));
                        }
                    }
                    else if (generator != null && generator.GeneratedOption != EGeneratedOption.Ignore)
                    {
                        commitUnit.Add(data.CreateCommitUnitMember(column, generator));
                    }
                }
            }
            else
            {
                foreach (var column in columns)
                {
                    var generator = data.GetValueGenerator(column);
                    commitUnit.Add(data.CreateCommitUnitMember(column, generator));
                }
            }
            return commitUnit;
        }
        /// <summary>
        /// 创建指定数据列提交单元成员。
        /// </summary>
        /// <param name="data">数据对象。</param>
        /// <param name="column">指定数据列。</param>
        /// <param name="generator">值生成对象。</param>
        /// <param name="customExpression">自定义表达式。</param>
        /// <returns>创建结果。</returns>
        public static CommitMember CreateCommitUnitMember(this IContentUnit data, ColumnMetadata column, ValueGenerateBase generator, DbExpression customExpression = null)
        {
            var context = data.GenerateContext;
            if (generator == null)
            {
                if (customExpression != null)
                    return data.ReisterReturnMember(new CommitExpressionMember(column, generator, customExpression));
                else
                    return new CommitMember(column);
            }
            else
            {
                switch (generator.GeneratedOption)
                {
                    case EGeneratedOption.Expression:
                        var expression = generator as ValueGenerateExpression;
                        return data.ReisterReturnMember(new CommitExpressionMember(column, generator, context.Translate(expression.Expression)));
                    case EGeneratedOption.Database:
                    case EGeneratedOption.Identity:
                        return data.ReisterReturnMember(new CommitMember(column, generator, ECommitValueType.Database));
                    case EGeneratedOption.Ignore:
                    case EGeneratedOption.Memory:
                        if (customExpression != null)
                            return data.ReisterReturnMember(new CommitExpressionMember(column, generator, customExpression));
                        else
                            return new CommitMember(column);
                    default:
                        throw new NotSupportedException(string.Format(Res.NotSupportedGeneratedOption, generator.GeneratedOption));
                }
            }
        }
        /// <summary>
        /// 创建具有标识列的提交单元。
        /// </summary>
        /// <param name="data">数据对象。</param>
        /// <param name="metadata">表元数据。</param>
        /// <param name="content">内容表达式。</param>
        /// <param name="hasExpkey">是否包含表达式生成的主键。</param>
        /// <returns></returns>
        public static CommitUnitBase CreateUnitForIdentity(this IContentUnit data, TableMetadata metadata, DbExpression content, out bool hasExpkey)
        {
            var identity = metadata.Keys.FirstOrDefault(a => a.GeneratedForInsert != null && a.GeneratedForInsert.GeneratedOption == EGeneratedOption.Identity);
            if (identity != null)
            {
                var identityMember = new CommitMember(identity, identity.GeneratedForInsert, ECommitValueType.Database);
                var commitUnit = new CommitIdentityUnit(metadata, identityMember);
                data.ReisterReturnMember(identityMember);
                data.CreateCommitUnit(commitUnit, metadata.Members.Where(a => a != identity), content);
                hasExpkey = commitUnit.Members.Any(a => a.Metadata.IsKey && a.ValueType == ECommitValueType.Expression);
                return commitUnit;
            }
            else
            {
                var commitUnit = new CommitKeyUnit(metadata);
                data.CreateCommitUnit(commitUnit, metadata.Members, content);
                hasExpkey = commitUnit.Keys.Any(a => a.ValueType == ECommitValueType.Expression);
                return commitUnit;
            }
        }
    }
}
