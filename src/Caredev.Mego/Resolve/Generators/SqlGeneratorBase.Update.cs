// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成更新操作语句，对象更新的总入口。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdate(GenerateContext context, DbExpression content = null)
        {
            var data = (GenerateDataForUpdate)context.Data;

            if (data.Items.Count == 1)
            {
                return GenerateForUpdateSingle(context, content);
            }
            else
            {
                if (context.Feature.HasCapable(EDbCapable.TemporaryTable | EDbCapable.ModifyJoin))
                {
                    return GenerateForUpdateTempTable(context, content);
                }
                else
                {
                    return GenerateForUpdateRepeat(context, content);
                }
            }
        }
        /// <summary>
        /// 使用重复生成的方式，生成更新多个对象的语句
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateRepeat(GenerateContext context, DbExpression content)
        {
            var data = (GenerateDataForUpdate)context.Data;
            var block = (BlockFragment)GenerateForUpdateSingle(context, content);
            var repeat = new RepeatBlockFragment(context, data.Items, data.CommitObject.Loader, block);
            return repeat;
        }
        /// <summary>
        /// 使用临时表形式，生成更新若干数据对象语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateTempTable(GenerateContext context, DbExpression content)
        {
            var data = (GenerateDataForUpdate)context.Data;
            var name = data.TargetName;
            var block = new BlockFragment(context);
            UpdateFragment mainUpdate = null;
            TemporaryTableFragment temptable = null;

            var members = data.GetTables().SelectMany(a => a.Members);
            foreach (var unit in data.GetUnits())
            {
                TemporaryTableFragment current = null;
                if (temptable == null)
                {
                    current = new TemporaryTableFragment(context, members);
                }
                else
                {
                    current = new TemporaryTableFragment(context, temptable.Name, members);
                }
                GenerateForUpdateRegister(context, content, current);

                var metadata = unit.Table;
                var update = new UpdateFragment(context, metadata, name);
                update.AddSource(update.Target, current);
                GenerateForUpdateMembers(context, unit, update, current);
                update.Target.Join(current, data.UnionConcurrencyMembers(metadata, metadata.Keys));
                block.Add(update);
                if (unit == data.MainUnit) mainUpdate = update;
                if (temptable == null)
                {
                    temptable = current;
                }
                else
                {
                    current.Members.ForEach(m => temptable.GetMember(m.Property));
                }
            }
            block.Insert(0, GenerateCreateTemplateTable(context, temptable, data.CommitObject, data.Items));

            if (data.ReturnMembers.Any())
            {
                var target = data.SubUnits == null ? mainUpdate.Target : new InheritFragment(context, data.Table);
                var select = new SelectFragment(context, target);
                select.Members.AddRange(data.ReturnMembers.Select(a => target.GetMember(a.Metadata)));

                var datatable = temptable.Clone();
                select.AddSource(datatable);
                target.Join(datatable, data.Table.Keys);

                block.Add(select);
                data.GenerateOutput();
            }
            data.SetConcurrencyExpectCount(data.TableCount + 1);
            return block;
        }
        /// <summary>
        /// 生成更新单个数据对象语句。
        /// </summary>
        /// <param name="context">生成上下文</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateSingle(GenerateContext context, DbExpression content)
        {
            var data = (GenerateDataForUpdate)context.Data;
            var block = new BlockFragment(context);
            UpdateFragment mainUpdate = null;
            var name = data.TargetName;
            GenerateForUpdateRegister(context, content, data.CommitObject);
            foreach (var unit in data.GetUnits())
            {
                var metadata = unit.Table;
                var update = new UpdateFragment(context, metadata, name);
                data.CommitObject.Parent = update;
                GenerateForUpdateMembers(context, unit, update, data.CommitObject);
                update.Where = update.Target.JoinCondition(data.CommitObject, data.UnionConcurrencyMembers(metadata, metadata.Keys));
                block.Add(update);
                if (unit == data.MainUnit) mainUpdate = update;
            }
            if (data.ReturnMembers.Any())
            {
                var target = data.SubUnits == null ? mainUpdate.Target : new InheritFragment(context, data.Table);
                var select = new SelectFragment(context, target);
                select.Members.AddRange(data.ReturnMembers.Select(a => target.GetMember(a.Metadata)));
                select.Where = target.JoinCondition(data.CommitObject, data.Table.Keys);
                block.Add(select);
                data.GenerateOutput();
            }
            data.SetConcurrencyExpectCount(data.TableCount);
            return block;
        }
        //生成前注册相关表达式。
        private void GenerateForUpdateRegister(GenerateContext context, DbExpression content, ISourceFragment source)
        {
            if (content != null && content is DbSelectExpression select)
            {
                context.RegisterSource(select.Source, source, true);
                context.RegisterSource(select.Source.Item, source, true);
            }
        }
        //生成更新表达式。
        private void GenerateForUpdateMembers(GenerateContext context, CommitUnitBase unit, UpdateFragment update, ISourceFragment source)
        {
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
        /// <summary>
        /// 表达式生成更新语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">生成表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateStatement(GenerateContext context, DbExpression content)
        {
            SourceFragment GetRootSource(ISourceFragment query)
            {
                if (query is SelectFragment select)
                {
                    return GetRootSource(select.Sources.FirstOrDefault());
                }
                else if (query is TableFragment || query is InheritFragment)
                {
                    return (SourceFragment)query;
                }
                return null;
            }
            var data = (GenerateDataForStatement)context.Data;
            var newitem = (DbNewExpression)data.ItemEpxression;
            var source = CreateSource(context, content) as QueryBaseFragment;
            var metadata = data.Table;
            if (metadata.InheritSets.Length == 0)
            {
                TableFragment target = GetRootSource(source) as TableFragment;
                if (target == null)
                {
                    target = new TableFragment(context, metadata, data.TargetName);
                }
                var update = new UpdateFragment(context, target)
                {
                    Where = source.Where,
                    Take = source.Take
                };
                update.AddSource(source.Sources);
                foreach (var item in newitem.Members)
                {
                    update.SetValue(metadata.Members[(PropertyInfo)item.Key], update.CreateExpression(item.Value));
                }
                return update;
            }
            else
            {
                var allmembers = metadata.InheritSets.SelectMany(a => a.Members)
                    .Concat(metadata.Members).ToDictionary(a => a.Member, a => a);
                InheritFragment inherit = GetRootSource(source) as InheritFragment;
                if (inherit == null)
                {
                    inherit = new InheritFragment(context, metadata);
                }
                var updates = inherit.Tables.ToDictionary(a => a.Metadata, a =>
                {
                    var target = inherit.Tables.Single(b => b.Metadata == a.Metadata);
                    var update = new UpdateFragment(context, target)
                    {
                        Where = source.Where,
                        Take = source.Take
                    };
                    update.AddSource(source.Sources);
                    return update;
                });
                foreach (var item in newitem.Members)
                {
                    var column = allmembers[(PropertyInfo)item.Key];
                    var update = updates[column.Table];
                    foreach (var s in source.Sources)
                        s.Parent = update;
                    update.SetValue(column, update.CreateExpression(item.Value));
                }
                var block = new BlockFragment(context);
                block.Add(updates.Values.Where(a => a.Values.Any()).OfType<ISqlFragment>());
                return block;
            }
        }
    }
}