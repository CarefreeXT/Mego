// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Contents;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Res = Properties.Resources;
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成用于插入的语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">插入表达式。</param>
        /// <returns>生成结果。</returns>
        protected virtual SqlFragment GenerateForInsertStatement(GenerateContext context, DbExpression content)
        {
            var data = (StatementContent)context.Data;
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
                    var current = new TemporaryTableFragment(context, currentMembers, createtable.Table.Name);

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
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成用于插入数据项的语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">插入表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertContent(GenerateContext context, DbExpression content)
        {
            var data = (InsertContent)context.Data;
            RegisterExpressionForCommit(context, content, data.CommitObject);
            if (context.Feature.HasCapable(EDbCapable.ModifyReturning))
            {
                return GenerateForInsertReturnMembers(context, data);
            }
            if (data.Unit is CommitKeyUnit keyunit)
            {//没有标识列
                if (data.Items.Count > 1)
                {//多个
                    if (data.HasExpressionKey || data.ReturnMembers.Any())
                    {
                        return GenerateForInsertTempTable(context, data);
                    }
                    else
                    {
                        return GenerateForInsertSimple(context, data);
                    }
                }
                else
                {//单个
                    if (data.HasExpressionKey)
                    {
                        return GenerateForInsertTempTable(context, data);
                    }
                    else
                    {
                        return GenerateForInsertSimple(context, data);
                    }
                }
            }
            else
            {
                if (data.Items.Count > 1 || !context.Feature.HasCapable(EDbCapable.ExternalLocalVariable))
                {//多个
                    return GenerateForInsertIdentityTempTable(context, data);
                }
                else
                {//单个
                    return GenerateForInsertIdentitySingle(context, data);
                }
            }
        }
        /// <summary>
        /// 使用INSERT返回语句生成插入数据对象语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">内容对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertReturnMembers(GenerateContext context, InsertContent data)
        {
            InsertValueFragment insert = null;
            if (data.Unit is CommitKeyUnit keyunit)
            {
                insert = data.InsertKeyUnit(keyunit, data.TargetName);
            }
            else if (data.Unit is CommitIdentityUnit identityunit)
            {
                insert = data.InsertUnit(identityunit, data.TargetName);
            }
            if (data.ReturnMembers.Any())
            {
            foreach (var returnMember in data.ReturnMembers)
            {
                insert.ReturnMembers.Add(insert.Target.GetMember(returnMember.Metadata));
            }
                data.GenerateOutput();
            }
            return insert;
        }
        /// <summary>
        /// 生成插入若干数据对象语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">内容对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertSimple(GenerateContext context, InsertContent data)
        {
            var keyunit = (CommitKeyUnit)data.Unit;
            var insert = data.InsertKeyUnit(keyunit, data.TargetName);
            if (!data.ReturnMembers.Any())
            {
                return insert;
            }
            var select = data.SelectReturns(insert.Target);
            data.GenerateOutput();
            return new BlockFragment(context, insert, select);
        }
        /// <summary>
        /// 使用临时表形式，生成删除若干数据对象（多个主键）语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">内容对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertTempTable(GenerateContext context, InsertContent data)
        {
            var keyunit = (CommitKeyUnit)data.Unit;
            var createtable = new CreateTempTableFragment(context, data.Columns);
            var temptable = createtable.Table;
            var block = new BlockFragment(context, createtable);
            block.Add(data.InsertTemptable(temptable, keyunit.Keys.Concat(keyunit.Members)));
            var insert = data.InsertTemptable(temptable, keyunit, data.TargetName);
            block.Add(insert);
            if (data.ReturnMembers.Any())
            {
                block.Add(data.SelectReturns(insert.Target, temptable.Clone()));
                data.GenerateOutput();
            }
            return block;
        }
        /// <summary>
        /// 生成插入单个数据对象语句，对象的主键是标识列。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">内容对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertIdentitySingle(GenerateContext context, InsertContent data)
        {
            var identityunit = (CommitIdentityUnit)data.Unit;
            var insert = data.InsertUnit(identityunit, data.TargetName);
            if (!data.ReturnMembers.Any())
            {
                return insert;
            }
            var returnSelect = data.SelectReturns(insert.Target);
            returnSelect.Where = context.Equal(
                insert.Target.GetMember(identityunit.Identity.Metadata),
                CreateIdentityFragment(context, returnSelect));
            data.GenerateOutput();
            return new BlockFragment(context, insert, returnSelect);
        }
        /// <summary>
        /// 使用临时表形式，生成插入单个数据对象语句，对象的主键是标识列，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">内容对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertIdentityTempTable(GenerateContext context, InsertContent data)
        {
            var identityUnit = (CommitIdentityUnit)data.Unit;
            var rowindex = new ColumnMetadata(data.Table, SupportMembers.DbMembers.CustomRowIndex,
                data.Columns.Select(a => a.Name).Unique("RowIndex"));
            var members = data.Columns.ToList();
            members.Add(rowindex);

            var createtable = new CreateTempTableFragment(context, members);
            var temptable = createtable.Table;

            var tempInsert = data.InsertTemptable(temptable, identityUnit.Members);
            tempInsert.SetValue(identityUnit.Identity.Metadata, new SimpleFragment(context, "0"));
            var rowindexfragment = new RowIndexFragment(context);
            tempInsert.SetValue(rowindex, rowindexfragment);

            TableFragment whileTarget = null;
            BlockFragment block = null;
            if (context.Feature.HasCapable(EDbCapable.ExternalCompoundStatement))
            {
                var index = new CreateVariableFragment(context, typeof(int));
                var whilefragment = GenerateForInsertIdentityMultipleWhile(context, identityUnit, index,
                    rowindex, createtable.Table, rowindexfragment, out whileTarget);
                block = new BlockFragment(context, index, createtable, tempInsert,
                    context.Assign(index.Name, context.StatementString("1")), whilefragment);
            }
            else
            {
                var repeatfragment = GenerateForInsertIdentityMultipleRepeat(context, identityUnit,
                  rowindex, createtable.Table, rowindexfragment, data.Items, out whileTarget);
                block = new BlockFragment(context, createtable, tempInsert, repeatfragment);
            }
            if (data.ReturnMembers.Any())
            {
                var returnSelect = data.SelectReturns(whileTarget, temptable.Clone());
                data.GenerateOutput();
                block.Add(returnSelect);
            }
            return block;
        }
    }
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成用于插入继承数据对象的语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">插入表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInheritInsert(GenerateContext context, DbExpression content)
        {
            var data = (InheritInsertContent)context.Data;
            RegisterExpressionForCommit(context, content, data.CommitObject);
            if (data.Units[0] is CommitKeyUnit keyunit)
            {//没有标识列
                if (data.Items.Count > 1)
                {//多个
                    if (data.HasExpressionKey || data.ReturnMembers.Any())
                    {
                        return GenerateForInsertTempTable(context, data);
                    }
                    else
                    {
                        return GenerateForInsertSimple(context, data);
                    }
                }
                else
                {//单个
                    if (data.HasExpressionKey)
                    {
                        return GenerateForInsertTempTable(context, data);
                    }
                    else
                    {
                        return GenerateForInsertSimple(context, data);
                    }
                }
            }
            else
            {
                if (data.Items.Count > 1 || !context.Feature.HasCapable(EDbCapable.ExternalLocalVariable))
                {//多个
                    return GenerateForInsertIdentityTempTable(context, data);
                }
                else
                {//单个
                    return GenerateForInsertIdentitySingle(context, data);
                }
            }
        }
        /// <summary>
        /// 生成插入若干数据对象语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">内容对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertSimple(GenerateContext context, InheritInsertContent data)
        {
            var block = new BlockFragment(context);
            foreach (var unit in data.Units.OfType<CommitKeyUnit>())
            {
                block.Add(data.InsertKeyUnit(unit));
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
        /// 使用临时表形式，生成删除若干数据对象（多个主键）语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">内容对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertTempTable(GenerateContext context, InheritInsertContent data)
        {
            var keyunits = data.Units.OfType<CommitKeyUnit>().ToArray();
            var createtable = new CreateTempTableFragment(context, data.Columns);
            var temptable = createtable.Table;
            var members = keyunits[0].Keys.Concat(keyunits.SelectMany(a => a.Members));
            var insert = data.InsertTemptable(temptable, members);

            var block = new BlockFragment(context, createtable, insert);
            foreach (var unit in keyunits)
            {
                block.Add(data.InsertTemptable(temptable, unit));
            }
            if (data.ReturnMembers.Any())
            {
                var target = new InheritFragment(context, data.Table);
                block.Add(data.SelectReturns(target, temptable.Clone()));
                data.GenerateOutput();
            }
            return block;
        }
        /// <summary>
        /// 生成插入单个数据对象语句，对象的主键是标识列。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">内容对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertIdentitySingle(GenerateContext context, InheritInsertContent data)
        {
            var identityUnit = (CommitIdentityUnit)data.Units[0];
            var keyMembers = identityUnit.Members.Where(a => a.Metadata.IsKey && a.ValueType == ECommitValueType.Expression).ToList();
            keyMembers.Add(identityUnit.Identity);
            var keyVariables = keyMembers.ToDictionary(a => a.Metadata,
                a => new CreateVariableFragment(context, a.Metadata.Member.PropertyType));
            var identityName = keyVariables[identityUnit.Identity.Metadata].Name;
            var block = new BlockFragment(context);
            if (!context.Feature.HasCapable(EDbCapable.ImplicitDeclareVariable))
            {
                block.Add(keyVariables.Values.OfType<ISqlFragment>());
            }

            var mainInsert = data.InsertUnit(identityUnit);
            foreach (var member in keyMembers.OfType<CommitExpressionMember>())
            {
                var name = keyVariables[member.Metadata].Name;
                block.Add(context.Assign(name, mainInsert.CreateExpression(member.Expression)));
                mainInsert.SetValue(member.Metadata, name);
            }
            block.Add(mainInsert);
            block.Add(context.Assign(identityName
                , CreateIdentityFragment(context, mainInsert)));
            for (int i = 1; i < data.Units.Length; i++)
            {
                var unit = (CommitKeyUnit)data.Units[i];
                var current = data.InsertKeyUnit(unit);
                foreach (var kv in keyVariables)
                {
                    current.SetValue(kv.Key, kv.Value.Name);
                }
                block.Add(current);
            }
            if (data.ReturnMembers.Any())
            {
                var target = new InheritFragment(context, data.Table);
                var returnSelect = data.SelectReturns(target);
                returnSelect.Where = context.Equal(
                    target.GetMember(identityUnit.Identity.Metadata),
                    identityName);
                block.Add(returnSelect);
                data.GenerateOutput();
            }
            return block;
        }
        /// <summary>
        /// 使用临时表形式，生成插入单个数据对象语句，对象的主键是标识列，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">内容对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForInsertIdentityTempTable(GenerateContext context, InheritInsertContent data)
        {
            var identityUnit = (CommitIdentityUnit)data.Units[0];
            var rowindex = new ColumnMetadata(data.Table, SupportMembers.DbMembers.CustomRowIndex,
                data.Columns.Select(a => a.Name).Unique("RowIndex"));
            var members = data.Columns.ToList();
            members.Add(rowindex);

            var createtable = new CreateTempTableFragment(context, members);
            var temptable = createtable.Table;

            var tempInsert = data.InsertTemptable(temptable, data.Units.SelectMany(a => a.Members));
            tempInsert.SetValue(identityUnit.Identity.Metadata, new SimpleFragment(context, "0"));
            var rowindexfragment = new RowIndexFragment(context);
            tempInsert.SetValue(rowindex, rowindexfragment);

            TableFragment whileTarget = null;
            BlockFragment block = null;
            if (context.Feature.HasCapable(EDbCapable.ExternalCompoundStatement))
            {
                var index = new CreateVariableFragment(context, typeof(int));
                var whilefragment = GenerateForInsertIdentityMultipleWhile(context, identityUnit, index,
                    rowindex, createtable.Table, rowindexfragment, out whileTarget);
                block = new BlockFragment(context, index, createtable, tempInsert,
                    context.Assign(index.Name, context.StatementString("1")), whilefragment);
            }
            else
            {
                var repeatfragment = GenerateForInsertIdentityMultipleRepeat(context, identityUnit,
                  rowindex, createtable.Table, rowindexfragment, data.Items, out whileTarget);
                block = new BlockFragment(context, createtable, tempInsert, repeatfragment);
            }

            foreach (var unit in data.Units.OfType<CommitKeyUnit>())
            {
                block.Add(data.InsertTemptable(temptable, unit));
            }

            if (data.ReturnMembers.Any())
            {
                var target = new InheritFragment(context, data.Table);
                var returnSelect = data.SelectReturns(target, temptable.Clone());
                data.GenerateOutput();
                block.Add(returnSelect);
            }
            return block;
        }
    }
    public partial class SqlGeneratorBase
    {
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
            TemporaryTableFragment temporaryTable, RowIndexFragment rowindexfragment, DbObjectsOperateBase operate, out TableFragment whileTarget)
        {
            var repeatfragment = new RepeatBlockFragment(context, operate);
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
        private IExpressionFragment CreateIdentityFragment(GenerateContext context, ISourceFragment fragment)
        {
            var getidentity = context.Translate(Expression.Call(null, SupportMembers.DbFunctions.GetIdentity));
            return fragment.CreateExpression(getidentity);
        }
    }
}