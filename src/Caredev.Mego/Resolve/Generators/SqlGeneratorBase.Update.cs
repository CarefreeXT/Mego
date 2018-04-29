// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Contents;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    public partial class SqlGeneratorBase
    {
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
            var data = (StatementContent)context.Data;
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
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成更新操作语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateContent(GenerateContext context, DbExpression content)
        {
            var data = (UpdateContent)context.Data;
            if (data.Items.Count == 1)
            {
                data.SetConcurrencyExpectCount(1);
                RegisterExpressionForCommit(context, content, data.CommitObject);
                return GenerateForUpdateSingle(context, data);
            }
            else if (context.Feature.HasCapable(EDbCapable.TemporaryTable))
            {
                data.SetConcurrencyExpectCount(2);
                if (context.Feature.HasCapable(EDbCapable.ModifyJoin))
                {
                    return GenerateForUpdateTempTable(context, data, content);
                }
                else
                {
                    return GenerateForUpdateTempTableRepeat(context, data, content);
                }
            }
            else
            {
                data.SetConcurrencyExpectCount(1);
                RegisterExpressionForCommit(context, content, data.CommitObject);
                return GenerateForUpdateRepeat(context, data, content);
            }
            throw new NotImplementedException();
        }
        /// <summary>
        /// 生成更新单个数据对象语句。
        /// </summary>
        /// <param name="context">生成上下文</param>
        /// <param name="data">数据对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateSingle(GenerateContext context, UpdateContent data)
        {
            var update = data.UpdateByKeys(data.Unit, data.TargetName);
            if (!data.ReturnMembers.Any())
            {
                return update;
            }
            var select = data.SelectReturns(update.Target);
            data.GenerateOutput();
            return new BlockFragment(context, update, select);
        }
        /// <summary>
        /// 使用临时表形式，生成更新若干数据对象语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateTempTable(GenerateContext context, UpdateContent data, DbExpression content)
        {
            var temptable = new TemporaryTableFragment(context, data.Columns);
            RegisterExpressionForCommit(context, content, temptable);
            var update = data.UpdateByTemptable(data.Unit, temptable, data.TargetName);

            var block = new BlockFragment(context, update);
            block.Insert(0, GenerateCreateTemplateTable(context, temptable, data.CommitObject, data.Items));
            if (data.ReturnMembers.Any())
            {
                block.Add(data.SelectReturns(update.Target, temptable.Clone()));
                data.GenerateOutput();
            }
            return block;
        }
        /// <summary>
        /// 使用临时表形式，生成更新若干数据对象语句，更新语句将是独立的，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>特性
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateTempTableRepeat(GenerateContext context, UpdateContent data, DbExpression content)
        {
            RegisterExpressionForCommit(context, content, data.CommitObject);
            var temptable = new TemporaryTableFragment(context, data.Columns);
            var block = new BlockFragment(context);
            foreach (var key in data.Table.Keys)
            {
                temptable.GetMember(key);
            }
            block.Add(GenerateCreateTemplateTable(context, temptable, data.CommitObject, data.Items));

            var updateblock = new BlockFragment(context, data.UpdateByKeys(data.Unit, data.TargetName));
            var repeat = new RepeatBlockFragment(context, data.Items, data.CommitObject.Loader, updateblock);
            block.Add(repeat);

            if (data.ReturnMembers.Any())
            {
                block.Add(data.SelectReturns(new TableFragment(context, data.Table), temptable.Clone()));
                data.GenerateOutput();
            }
            return block;
        }
        /// <summary>
        /// 使用重复生成的方式，生成更新多个对象的语句
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateRepeat(GenerateContext context, UpdateContent data, DbExpression content)
        {
            var block = (BlockFragment)GenerateForUpdateSingle(context, data);
            var repeat = new RepeatBlockFragment(context, data.Items, data.CommitObject.Loader, block);
            return repeat;
        }
    }
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成继承数据对象的更新操作语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInheritUpdate(GenerateContext context, DbExpression content)
        {
            var data = (InheritUpdateContent)context.Data;
            if (data.Items.Count == 1)
            {
                data.SetConcurrencyExpectCount(data.Tables.Length);
                RegisterExpressionForCommit(context, content, data.CommitObject);
                return GenerateForUpdateSingle(context, data);
            }
            else if (context.Feature.HasCapable(EDbCapable.TemporaryTable))
            {
                data.SetConcurrencyExpectCount(data.Tables.Length + 1);
                if (context.Feature.HasCapable(EDbCapable.ModifyJoin))
                {
                    return GenerateForUpdateTempTable(context, data, content);
                }
                else
                {
                    return GenerateForUpdateTempTableRepeat(context, data, content);
                }
            }
            else
            {
                data.SetConcurrencyExpectCount(data.Tables.Length);
                RegisterExpressionForCommit(context, content, data.CommitObject);
                return GenerateForUpdateRepeat(context, data, content);
            }
            throw new NotImplementedException();
        }
        /// <summary>
        /// 生成更新单个数据对象语句。
        /// </summary>
        /// <param name="context">生成上下文</param>
        /// <param name="data">数据对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateSingle(GenerateContext context, InheritUpdateContent data)
        {
            var block = new BlockFragment(context);
            foreach (var unit in data.Units)
            {
                block.Add(data.UpdateByKeys(unit));
            }
            if (data.ReturnMembers.Any())
            {
                var target = new InheritFragment(context, data.Table);
                block.Add(data.SelectReturns(target));
                data.GenerateOutput();
            }
            return block;
        }
        /// <summary>
        /// 使用临时表形式，生成更新若干数据对象语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateTempTable(GenerateContext context, InheritUpdateContent data, DbExpression content)
        {
            var block = new BlockFragment(context);
            var temptable = new TemporaryTableFragment(context, data.Columns);

            foreach (var unit in data.Units)
            {
                var current = new TemporaryTableFragment(context, data.Columns, temptable.Name);
                RegisterExpressionForCommit(context, content, current);
                var update = data.UpdateByTemptable(unit, current, data.TargetName);
                block.Add(update);
                current.Members.ForEach(m => temptable.GetMember(m.Property));
            }
            block.Insert(0, GenerateCreateTemplateTable(context, temptable, data.CommitObject, data.Items));
            if (data.ReturnMembers.Any())
            {
                var target = new InheritFragment(context, data.Table);
                block.Add(data.SelectReturns(target, temptable.Clone()));
                data.GenerateOutput();
            }
            return block;
        }
        /// <summary>
        /// 使用临时表形式，生成更新若干数据对象语句，更新语句将是独立的，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>特性
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateTempTableRepeat(GenerateContext context, InheritUpdateContent data, DbExpression content)
        {
            RegisterExpressionForCommit(context, content, data.CommitObject);
            var temptable = new TemporaryTableFragment(context, data.Columns);
            var block = new BlockFragment(context);
            foreach (var key in data.Table.Keys)
            {
                temptable.GetMember(key);
            }
            block.Add(GenerateCreateTemplateTable(context, temptable, data.CommitObject, data.Items));

            var updateblock = new BlockFragment(context);
            foreach (var unit in data.Units)
            {
                updateblock.Add(data.UpdateByKeys(unit));
            }
            var repeat = new RepeatBlockFragment(context, data.Items, data.CommitObject.Loader, updateblock);
            block.Add(repeat);

            if (data.ReturnMembers.Any())
            {
                block.Add(data.SelectReturns(new TableFragment(context, data.Table), temptable.Clone()));
                data.GenerateOutput();
            }
            return block;
        }
        /// <summary>
        /// 使用重复生成的方式，生成更新多个对象的语句
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <param name="content">更新表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForUpdateRepeat(GenerateContext context, InheritUpdateContent data, DbExpression content)
        {
            var block = (BlockFragment)GenerateForUpdateSingle(context, data);
            var repeat = new RepeatBlockFragment(context, data.Items, data.CommitObject.Loader, block);
            return repeat;
        }
    }
}