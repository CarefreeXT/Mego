// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Res = Properties.Resources;
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成用于插入数据项的语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">插入表达式。</param>
        /// <returns>生成结果。</returns>
        protected virtual SqlFragment GenerateForInsert(GenerateContext context, DbExpression content = null)
        {
            var data = (GenerateDataForInsert)context.Data;
            if (content != null && content is DbSelectExpression select)
            {
                context.RegisterSource(select.Source, data.CommitObject);
                context.RegisterSource(select.Source.Item, data.CommitObject);
            }
            try
            {
                if (data.MainUnit is CommitKeyUnit keyunit)
                {//没有标识列
                    if (data.Items.Count > 1)
                    {//多个
                        if (data.HasExpressionKey || data.ReturnMembers.Any())
                        {
                            return GenerateForInsertTempTable(context);
                        }
                        else
                        {
                            return GenerateForInsertSimple(context);
                        }
                    }
                    else
                    {//单个
                        if (data.HasExpressionKey)
                        {
                            return GenerateForInsertTempTable(context);
                        }
                        else
                        {
                            return GenerateForInsertSimple(context);
                        }
                    }
                }
                else
                {//标识列
                    if (data.Items.Count > 1 || !context.Feature.HasCapable(EDbCapable.ExternalLocalVariable))
                    {//多个
                        return GenerateForInsertIdentityTempTable(context);
                    }
                    else
                    {//单个
                        return GenerateForInsertIdentitySingle(context);
                    }
                }
            }
            finally
            {
                data.GenerateOutput();
            }
        }
        /// <summary>
        /// 生成插入若干数据对象语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertSimple(GenerateContext context)
        {
            var data = (GenerateDataForInsert)context.Data;
            var name = data.TargetName;
            var block = new BlockFragment(context);
            var mainunit = (CommitKeyUnit)data.MainUnit;
            InsertValueFragment mainInsert = null;
            foreach (var unit in data.GetUnits().OfType<CommitKeyUnit>())
            {
                var target = new TableFragment(context, unit.Table, name);
                var insert = new InsertValueFragment(context, target, data.CommitObject, data.Items);
                data.CommitObject.Parent = insert;
                unit.Keys.Concat(unit.Members).ForEach(member => GenerateForInsertMembers(insert, member));
                block.Add(insert);
                mainInsert = insert;
            }
            GenerateForInsertReturnStatement(context, block, mainInsert.Target, null);
            return block;
        }
        /// <summary>
        /// 使用临时表形式，生成删除若干数据对象（多个主键）语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertTempTable(GenerateContext context)
        {
            var data = (GenerateDataForInsert)context.Data;
            var members = data.GetTables().SelectMany(a => a.Members);
            var createtable = new CreateTempTableFragment(context, members);
            var temptable = createtable.Table;

            var insert = new InsertValueFragment(context, temptable, data.CommitObject, data.Items);
            var mainunit = (CommitKeyUnit)data.MainUnit;
            mainunit.Keys.ForEach(member => GenerateForInsertMembers(insert, member));
            data.CommitObject.Parent = insert;
            foreach (var unit in data.GetUnits())
            {
                unit.Members.ForEach(member => GenerateForInsertMembers(insert, member));
            }
            var block = new BlockFragment(context, createtable, insert);

            InsertFragment mainInsert = null;
            foreach (var unit in data.GetUnits().OfType<CommitKeyUnit>())
            {
                mainInsert = context.Insert(temptable, unit);
                block.Add(mainInsert);
            }
            GenerateForInsertReturnStatement(context, block, mainInsert.Target, temptable);
            return block;
        }
        /// <summary>
        /// 生成插入单个数据对象语句，对象的主键是标识列。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertIdentitySingle(GenerateContext context)
        {
            var getidentity = context.Translate(Expression.Call(null, SupportMembers.DbFunctions.GetIdentity));

            var data = (GenerateDataForInsert)context.Data;
            var identityunit = (CommitIdentityUnit)data.MainUnit;
            var keyVars = new Dictionary<ColumnMetadata, VariableFragment>();
            var identityCreate = new CreateVariableFragment(context, identityunit.Identity.Metadata.Member.PropertyType);
            keyVars.Add(identityunit.Identity.Metadata, identityCreate.Name);
            var block = new BlockFragment(context);
            if (!context.Feature.HasCapable(EDbCapable.ImplicitDeclareVariable))
            {
                block.Add(identityCreate);
            }
            var name = data.TargetName;
            var maintarget = new TableFragment(context, identityunit.Table, name);
            {
                var insert = new InsertValueFragment(context, maintarget, data.CommitObject, data.Items);
                data.CommitObject.Parent = insert;
                identityunit.Members.ForEach(member =>
                {
                    if (member.Metadata.IsKey && member.ValueType == ECommitValueType.Expression)
                    {
                        var expMember = (CommitExpressionMember)member;
                        var exp = insert.CreateExpression(expMember.Expression);
                        var expVar = new CreateVariableFragment(context, member.Metadata.Member.PropertyType);
                        block.Add(expVar);
                        block.Add(context.Assign(expVar.Name, exp));
                        keyVars.Add(member.Metadata, expVar.Name);
                    }
                    else
                    {
                        GenerateForInsertMembers(insert, member);
                    }
                });
                block.Add(insert);
                block.Add(context.Assign(identityCreate.Name, insert.CreateExpression(getidentity)));
            }
            if (data.SubUnits != null)
            {
                foreach (var unit in data.SubUnits)
                {
                    var target = new TableFragment(context, unit.Table);
                    var insert = new InsertValueFragment(context, target, data.CommitObject, data.Items);
                    data.CommitObject.Parent = insert;
                    unit.Keys.ForEach(member =>
                    {
                        if (keyVars.TryGetValue(member.Metadata, out VariableFragment value))
                        {
                            insert.SetValue(member.Metadata, value);
                        }
                        else
                        {
                            GenerateForInsertMembers(insert, member);
                        }
                    });
                    unit.Members.ForEach(member => GenerateForInsertMembers(insert, member));
                    block.Add(insert);
                }
            }
            if (data.ReturnMembers.Any())
            {
                ISourceFragment target = maintarget;
                if (data.SubUnits != null) target = new InheritFragment(context, data.Table);
                var select = new SelectFragment(context, target);
                select.Members.AddRange(data.ReturnMembers.Select(a => target.GetMember(a.Metadata)));
                select.Where = data.Table.Keys.Select(key => context.Equal(target.GetMember(key),
                    keyVars.ContainsKey(key) ? (IExpressionFragment)keyVars[key] : data.CommitObject.GetMember(key))).Merge();
                block.Add(select);
            }
            return block;
        }
        /// <summary>
        /// 使用临时表形式，生成插入单个数据对象语句，对象的主键是标识列，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertIdentityTempTable(GenerateContext context)
        {
            var data = (GenerateDataForInsert)context.Data;
            var mainunit = (CommitIdentityUnit)data.MainUnit;
            var members = data.GetTables().SelectMany(a => a.Members).ToList();
            var rowindex = new ColumnMetadata(data.Table, SupportMembers.DbMembers.CustomRowIndex,
                members.Select(a => a.Name).Unique("RowIndex"));
            members.Add(rowindex);

            var createtable = new CreateTempTableFragment(context, members);
            var temptable = createtable.Table;

            var insert = new InsertValueFragment(context, temptable, data.CommitObject, data.Items);
            data.CommitObject.Parent = insert;
            foreach (var unit in data.GetUnits())
            {
                unit.Members.ForEach(member => GenerateForInsertMembers(insert, member));
            }
            insert.SetValue(mainunit.Identity.Metadata, new SimpleFragment(context, "0"));
            var rowindexfragment = new RowIndexFragment(context);
            insert.SetValue(rowindex, rowindexfragment);


            TableFragment whileTarget = null;
            BlockFragment block = null;
            if (context.Feature.HasCapable(EDbCapable.ExternalCompoundStatement))
            {
                var index = new CreateVariableFragment(context, typeof(int));
                var whilefragment = GenerateForInsertIdentityMultipleWhile(context, mainunit, index,
                    rowindex, createtable.Table, rowindexfragment, out whileTarget);
                block = new BlockFragment(context, index, createtable, insert,
                    context.Assign(index.Name, context.StatementString("1")), whilefragment);
            }
            else
            {
                var repeatfragment = GenerateForInsertIdentityMultipleRepeat(context, mainunit,
                  rowindex, createtable.Table, rowindexfragment, data, out whileTarget);
                block = new BlockFragment(context, createtable, insert, repeatfragment);
            }

            if (data.SubUnits != null)
            {
                foreach (var unit in data.SubUnits)
                {
                    block.Add(context.Insert(createtable.Table, unit));
                }
            }
            GenerateForInsertReturnStatement(context, block, whileTarget, createtable.Table);
            return block;
        }

        private WhileFragment GenerateForInsertIdentityMultipleWhile(GenerateContext context, CommitIdentityUnit mainunit, CreateVariableFragment index,
            ColumnMetadata rowindex, TemporaryTableFragment temporaryTable, RowIndexFragment rowindexfragment, out TableFragment whileTarget)
        {
            var whilefragment = new WhileFragment(context, context.LessThan(index.Name, rowindexfragment));
            var block = whilefragment.Block;
            var whileInsert = context.Insert(temporaryTable, mainunit.Table,
               mainunit.Members.Where(a => a.ValueType != ECommitValueType.Database).Select(a => a.Metadata));
            var whileSelect = (QueryBaseFragment)whileInsert.Query;
            whileSelect.Where = context.Equal(temporaryTable.GetMember(rowindex), index.Name);
            block.Add(whileInsert);
            var whileUpdate = new UpdateFragment(context, temporaryTable);
            var getidentity = whileUpdate.CreateExpression(
                context.Translate(Expression.Call(null, SupportMembers.DbFunctions.GetIdentity)));
            whileUpdate.SetValue(mainunit.Identity.Metadata, getidentity);
            whileUpdate.Where = context.Equal(temporaryTable.GetMember(rowindex), index.Name);
            block.Add(whileUpdate);
            block.Add(context.Assign(index.Name, context.Add(index.Name, context.StatementString("1"))));
            whileTarget = (TableFragment)whileInsert.Target;
            return whilefragment;
        }

        private SqlFragment GenerateForInsertIdentityMultipleRepeat(GenerateContext context, CommitIdentityUnit mainunit, ColumnMetadata rowindex,
            TemporaryTableFragment temporaryTable, RowIndexFragment rowindexfragment, GenerateDataForInsert data, out TableFragment whileTarget)
        {
            var repeatfragment = new RepeatBlockFragment(context, data.Items);
            var block = repeatfragment.Block;
            var whileInsert = context.Insert(temporaryTable, mainunit.Table,
               mainunit.Members.Where(a => a.ValueType != ECommitValueType.Database).Select(a => a.Metadata));
            var whileSelect = (QueryBaseFragment)whileInsert.Query;
            whileSelect.Where = context.Equal(temporaryTable.GetMember(rowindex), new RowIndexFragment(context));
            block.Add(whileInsert);
            var whileUpdate = new UpdateFragment(context, temporaryTable);
            var getidentity = whileUpdate.CreateExpression(
                context.Translate(Expression.Call(null, SupportMembers.DbFunctions.GetIdentity)));
            whileUpdate.SetValue(mainunit.Identity.Metadata, getidentity);
            whileUpdate.Where = context.Equal(temporaryTable.GetMember(rowindex), new RowIndexFragment(context));
            block.Add(whileUpdate);
            whileTarget = (TableFragment)whileInsert.Target;
            return repeatfragment;
        }

        private void GenerateForInsertMembers(InsertValueFragment insert, CommitMember member)
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

        private void GenerateForInsertReturnStatement(GenerateContext context,
            BlockFragment block, ISourceFragment target, TemporaryTableFragment temptable)
        {
            var data = (GenerateDataForInsert)context.Data;
            if (data.ReturnMembers.Any())
            {
                if (data.SubUnits != null) target = new InheritFragment(context, data.Table);
                var select = new SelectFragment(context, target);
                select.Members.AddRange(data.ReturnMembers.Select(a => target.GetMember(a.Metadata)));
                if (temptable == null)
                {
                    select.Where = target.JoinCondition(data.CommitObject, data.Table.Keys);
                }
                else
                {
                    var datatable = temptable.Clone();
                    select.AddSource(datatable);
                    target.Join(datatable, data.Table.Keys);
                }
                block.Add(select);
            }
        }
        /// <summary>
        /// 生成用于插入的语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">插入表达式。</param>
        /// <returns>生成结果。</returns>
        protected virtual SqlFragment GenerateForInsertStatement(GenerateContext context, DbExpression content)
        {
            var data = (GenerateDataForStatement)context.Data;
            var newitem = (DbNewExpression)data.ItemEpxression;
            var source = CreateSource(context, content) as QueryBaseFragment;
            var metadata = data.Table;
            if (metadata.InheritSets.Length == 0)
            {
                var target = new TableFragment(context, metadata, data.TargetName);
                var insert = new InsertFragment(context, target, source);
                GenerateForInsertStatementMembers(context, newitem, source, insert, member => metadata.Members[member]);
                return insert;
            }
            else
            {
                var allsource = metadata.InheritSets.Concat(new TableMetadata[] { metadata });
                var allmembers = allsource.SelectMany(a => a.Members).ToDictionary(a => a.Member, a => a);
                var createtable = new CreateTempTableFragment(context, newitem.Members.Keys.OfType<PropertyInfo>().Select(a => allmembers[a]));
                var insert = new InsertFragment(context, createtable.Table, source);
                GenerateForInsertStatementMembers(context, newitem, source, insert, member => allmembers[member]);
                var block = new BlockFragment(context, createtable, insert);
                var query = from a in newitem.Members.Keys.OfType<PropertyInfo>().Select(m => allmembers[m])
                            group a by a.Table into g
                            select new { g.Key, Members = g };
                var prefixMembers = query.First().Members.Where(a => a.IsKey);
                foreach (var g in query)
                {
                    IEnumerable<ColumnMetadata> currentMembers = g.Members;
                    if (g.Key.InheritSets.Length > 0)
                    {
                        currentMembers = prefixMembers.Concat(g.Members);
                    }
                    var current = new TemporaryTableFragment(context, createtable.Table.Name, currentMembers);

                    block.Add(context.Insert(current, g.Key, currentMembers));
                }
                return block;
            }
        }
        //生成语句插入的成员。
        private void GenerateForInsertStatementMembers(GenerateContext context, DbNewExpression newitem,
            QueryBaseFragment source, InsertFragment insert, Func<PropertyInfo, ColumnMetadata> getmember)
        {
            int count = 0;
            foreach (var item in newitem.Members)
            {
                count++;
                insert.CreateMember(null, getmember((PropertyInfo)item.Key));
                IMemberFragment member = null;
                if (_CreateExpressionMethods.TryGetValue(item.Value.ExpressionType, out CreateExpressionDelegate method))
                {
                    member = RetrievalMemberForExpression(context, source, item.Value, item.Key, false);
                }
                else if (_RetrievalMemberMethods.TryGetValue(item.Value.ExpressionType, out RetrievalMemberDelegate method1))
                {
                    member = method1(context, source, item.Value, item.Key, false);
                }
                else
                {
                    throw new NotSupportedException(Res.NotSupportedCreateInsertMember);
                }
                if (source.Members.Count != count)
                {
                    source.Members.Add(member);
                }
            }
        }
    }
}