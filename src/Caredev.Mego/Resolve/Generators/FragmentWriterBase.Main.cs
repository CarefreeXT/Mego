// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    //输出 ISqlFragment 最基础的方法
    public partial class FragmentWriterBase
    {
        private readonly IDictionary<Type, WriteFragmentDelegate> _WriteFragmentMethods;
        /// <summary>
        /// 初始化写入语句片段方法集合。
        /// </summary>
        /// <returns>语句片段与写入方法的映射字典。</returns>
        protected virtual IDictionary<Type, WriteFragmentDelegate> InitialMethodsForWriteFragment()
        {
            return new Dictionary<Type, WriteFragmentDelegate>()
            {
                { typeof(ScalarFragment), WriteFragmentForScalar },
                { typeof(BinaryLogicalFragment), WriteFragmentForBinaryLogical },
                { typeof(BinaryFragment), WriteFragmentForBinary },
                { typeof(ConstantFragment), WriteFragmentForConstant },
                { typeof(ConstantListFragment), WriteFragmentForConstantList },
                { typeof(ValueListFragment), WriteFragmentForValueList },
                { typeof(SortFragment), WriteFragmentForSort },
                { typeof(DefaultFragment), WriteFragmentForDefault },
                { typeof(UnaryFragment), WriteFragmentForUnary },
                { typeof(RowIndexFragment), WriteFragmentForRowIndex },
                { typeof(WhileFragment), WriteFragmentForWhile },
                { typeof(RepeatBlockFragment), WriteFragmentForRepeatBlock },
                { typeof(StringConcatFragment), WriteFragmentForStringConcat },

                { typeof(AggregateFragment), WriteFragmentForAggregate },
                { typeof(ColumnFragment),WriteFragmentForColumnMember },
                { typeof(ExpressionMemberFragment),WriteFragmentForExpressionMember },
                { typeof(ReferenceMemberFragment),WriteFragmentForReferenceMember },

                { typeof(VariableFragment), WriteFragmentForVariable },
                { typeof(SimpleFragment), WriteFragmentForSimple },
                { typeof(ObjectNameFragment), WriteFragmentForObjectName },
                { typeof(TempTableNameFragment), WriteFragmentForDbName },
                { typeof(DbNameFragment), WriteFragmentForDbName },
                { typeof(TemporaryTableFragment), WriteFragmentForTemporaryTable },

                { typeof(TableFragment),WriteFragmentForTable },
                { typeof(InheritFragment),WriteFragmentForInherit },
                { typeof(SetFragment),WriteFragmentForSet },

                { typeof(InsertValueFragment), WriteFragmentForInsertValue },
                { typeof(InsertFragment), WriteFragmentForInsert },
                { typeof(UpdateFragment), WriteFragmentForUpdate },
                { typeof(DeleteFragment), WriteFragmentForDelete },
                { typeof(SelectFragment), WriteFragmentForSelect },
                { typeof(CreateTableFragment), WriteFragmentForCreateTable },
                { typeof(CreateTempTableFragment), WriteFragmentForCreateTemporaryTable },
                { typeof(CreateViewFragment), WriteFragmentForCreateView },
                { typeof(CreateRelationFragment), WriteFragmentForCreateRelation },
                { typeof(DropRelationFragment), WriteFragmentForDropRelation },
                { typeof(DropObjectFragment), WriteFragmentForDropObject },
                { typeof(RenameObjectFragment), WriteFragmentForRenameObject },

                { typeof(CommitMemberFragment), WriteFragmentForCommitMember },

                { typeof(BlockFragment),WriteFragmentForBlock },
            };
        }
        /// <summary>
        /// 写入语句片段公共方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        public void WriteFragment(SqlWriter writer, ISqlFragment fragment)
        {
            if (!_WriteFragmentMethods.TryGetValue(fragment.GetType(), out WriteFragmentDelegate method))
            {
                throw new NotSupportedException(string.Format(Res.NotSupportedWriteFragment, fragment.GetType()));
            }
            method(writer, fragment);
        }
        /// <summary>
        /// 写入<see cref="ScalarFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        private void WriteFragmentForScalar(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (ScalarFragment)fragment;
            if (_WriteScalarMethods.TryGetValue(content.Function, out WriteFragmentDelegate action))
            {
                action(writer, content);
                return;
            }
            var attribute = content.Function.GetCustomAttribute<DbFunctionAttribute>();
            if (attribute != null)
            {
                if (attribute.IsSystemFunction)
                {
                    writer.Write(attribute.Name);
                }
                else
                {
                    WriteDbObject(writer, attribute.Name, attribute.Schema);
                }
                writer.Write('(');
                content.Arguments.ForEach(() => writer.Write(','), a => a.WriteSql(writer));
                writer.Write(')');
                return;
            }
            throw new NotSupportedException(string.Format(Res.NotSupportedWriteFragmentForScalar, content.Function));
        }
        /// <summary>
        /// 写入<see cref="BinaryFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        private void WriteFragmentForBinary(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (BinaryFragment)fragment;
            if (!_WriteBinaryMethods.TryGetValue(content.Kind, out WriteFragmentDelegate action))
            {
                throw new NotSupportedException(string.Format(Res.NotSupportedWriteFragmentForBinary, content.Kind));
            }
            action(writer, content);
        }
        /// <summary>
        /// 写入<see cref="AggregateFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        private void WriteFragmentForAggregate(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (AggregateFragment)fragment;
            if (!_WriteAggregateMethods.TryGetValue(content.Function, out WriteFragmentDelegate action))
            {
                throw new NotSupportedException(string.Format(Res.NotSupportedWriteFragmentForAggregate, content.Function));
            }
            action(writer, content);
        }
    }
    //Name Write Method
    public partial class FragmentWriterBase
    {
        /// <summary>
        /// 写入<see cref="ObjectNameFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForObjectName(SqlWriter writer, ISqlFragment fragment)
        {
            var name = (ObjectNameFragment)fragment;
            WriteDbObject(writer, name.Name, name.Schema);
        }
        /// <summary>
        /// 写入<see cref="TempTableNameFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForDbName(SqlWriter writer, ISqlFragment fragment)
        {
            var name = (INameFragment)fragment;
            WriteDbName(writer, name.Name);
        }
        /// <summary>
        /// 写入<see cref="VariableFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForVariable(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (VariableFragment)fragment;
            writer.Write('@');
            writer.Write(content.Name);
        }
        /// <summary>
        /// 写入<see cref="SimpleFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForSimple(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (SimpleFragment)fragment;
            writer.Write(content.Content);
        }
    }
    //Insert Update Delete 语句块写入方法
    public partial class FragmentWriterBase
    {
        /// <summary>
        /// 写入<see cref="InsertValueFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForInsertValue(SqlWriter writer, ISqlFragment fragment)
        {
            var insert = (InsertValueFragment)fragment;
            writer.Write("INSERT INTO ");
            insert.Target.WriteSql(writer);
            writer.WriteLine();
            writer.Write('(');
            insert.Members.ForEach(
                () => writer.Write(","),
                column => this.WriteDbName(writer, column.OutputName));
            writer.WriteLine(")");
            writer.Write("VALUES");
            var loader = insert.Source.Loader;
            insert.Data.ForEach(() => writer.WriteLine(","),
                item =>
                {
                    loader.Load(item);
                    writer.Write('(');
                    insert.Values.ForEach(() => writer.Write(","), val => val.WriteSql(writer));
                    writer.Write(')');
                });
        }
        /// <summary>
        /// 写入<see cref="InsertFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForInsert(SqlWriter writer, ISqlFragment fragment)
        {
            var insert = (InsertFragment)fragment;
            writer.Write("INSERT INTO ");
            insert.Target.WriteSql(writer);
            writer.WriteLine();
            writer.Write('(');
            insert.Members.ForEach(
                () => writer.Write(","),
                column => this.WriteDbName(writer, column.OutputName));
            writer.WriteLine(")");
            insert.Query.WriteSql(writer);
        }
        /// <summary>
        /// 写入<see cref="UpdateFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForUpdate(SqlWriter writer, ISqlFragment fragment)
        {
            var update = (UpdateFragment)fragment;
            writer.Write("UPDATE ");
            writer.Write(update.Target.AliasName);
            writer.WriteLine();
            writer.Write("SET ");
            var values = update.Values.ToArray();
            for (int i = 0; i < update.Members.Count; i++)
            {
                var member = update.Members[i];
                var value = values[i];
                if (i > 0)
                    writer.Write(", ");
                WriteDbName(writer, member.OutputName);
                writer.Write(" = ");
                value.WriteSql(writer);
            }
            writer.WriteLine();
            writer.Write("FROM ");
            if (update.Sources.Contains(update.Target) ||
                update.Sources.OfType<InheritFragment>()
                .Any(a => a.Tables.Contains(update.Target)))
            {
                update.Sources.ForEach(() => writer.WriteLine(),
                    source => WriteFragmentForSource(writer, source));
            }
            else
            {
                update.Target.WriteSql(writer);
                WriteAliasName(writer, update.Target.AliasName);
                if (update.Sources.Any())
                {
                    writer.Write(" CROSS JOIN ");
                    update.Sources.ForEach(() => writer.WriteLine(),
                        source => WriteFragmentForSource(writer, source));
                }
            }
            WriteFragmentForWhere(writer, update.Where);
        }
        /// <summary>
        /// 写入<see cref="DeleteFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForDelete(SqlWriter writer, ISqlFragment fragment)
        {
            var current = (DeleteFragment)fragment;
            writer.Write("DELETE ");
            if (current.Take > 0)
            {
                writer.Write("TOP(");
                writer.Write(current.Take);
                writer.Write(") ");
            }
            writer.Write(current.Target.AliasName);
            WriteFragmentForFrom(writer, current.Sources);
            WriteFragmentForWhere(writer, current.Where);
        }
        /* SQL92 Syntax Command: SELECT query
         * SELECT [ ALL | DISTINCT [ ON ( expression [, ...] ) ] ]
	     *      expression [ <![AS]> name ] [,...]
	     *      [ INTO [ TEMPORARY | TEMP ] [ TABLE ] new_table ]
	     *      [ FROM {table | (select query)} [ alias ] [,...] ]
	     *      [ {{LEFT | RIGHT} [OUTER] | NATURAL |[FULL] OUTER} JOIN table alias
		 *      {ON condition | USING(col1,col2,...)} ]
	     *      [ WHERE {condition | EXISTS (correlated subquery)} ]
	     *      [ GROUP BY column [,...] ]
	     *      [ HAVING condition [,...] ]
	     *      [ { UNION [ ALL ] | INTERSECT | EXCEPT | MINUS } select ]
	     *      [ ORDER BY {column | int} [ ASC | DESC | USING operator ] [,...] ]
	     *      [ FOR UPDATE [ OF class_name [,...] ] ]
	     *      LIMIT { count | ALL } [ { OFFSET | ,} start ]
             */
        /// <summary>
        /// 写入<see cref="SelectFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForSelect(SqlWriter writer, ISqlFragment fragment)
        {
            var select = (SelectFragment)fragment;
            writer.Enter(delegate ()
            {
                writer.Write("SELECT");
                if (select.Distinct) writer.Write(" DISTINCT");
                writer.WriteLine();
                WriteFragmentForSelectMembers(writer, select.Members);
                WriteFragmentForFrom(writer, select.Sources);
                WriteFragmentForWhere(writer, select.Where);
                WriteFragmentForGroupBy(writer, select.GroupBys);
                WriteFragmentForOrderBy(writer, select.Sorts);
                if (select.Take > 0)
                {
                    writer.WriteLine();
                    writer.Write("LIMIT ");
                    writer.Write(select.Take);
                    if (select.Skip > 0)
                    {
                        writer.Write(" OFFSET ");
                        writer.Write(select.Skip);
                    }
                }
                else if (select.Skip > 0)
                {
                    writer.WriteLine();
                    writer.Write("LIMIT ");
                    writer.Write(int.MaxValue);
                    writer.Write(" OFFSET ");
                    writer.Write(select.Skip);
                }
            }, select);
        }
        /// <summary>
        /// 写入<see cref="CreateTableFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForCreateTable(SqlWriter writer, ISqlFragment fragment)
        {
            var create = (CreateTableFragment)fragment;
            var metadata = create.Metadata;

            writer.Write("CREATE TABLE ");
            create.Name.WriteSql(writer);
            writer.Write('(');
            if (metadata.InheritSets.Length > 0)
            {
                metadata.Keys.ForEach(key =>
                {
                    WriteDbName(writer, key.Name);
                    WriteDbDataType(writer, key);
                    writer.Write(" NOT NULL");
                    writer.WriteLine(", ");
                });
            }
            create.Members.ForEach(() => writer.WriteLine(", "),
              m => m.WriteSql(writer));
            var keys = metadata.Keys;
            writer.WriteLine(",");
            writer.Write("PRIMARY KEY ( ");
            keys.ForEach(() => writer.Write(","), key => WriteDbName(writer, key.Name));
            writer.WriteLine(")");
            writer.WriteLine(")");
        }
        /// <summary>
        /// 写入<see cref="CreateTempTableFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForCreateTemporaryTable(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (CreateTempTableFragment)fragment;
            if (content.IsVariable)
            {
                throw new NotSupportedException(Res.NotSupportedWriteTableVariable);
            }
            var create = (CreateTempTableFragment)fragment;
            writer.Write($"CREATE TEMPORARY TABLE ");
            create.Table.WriteSql(writer);
            writer.WriteLine("(");
            create.Members.ForEach(() => writer.WriteLine(", "),
                m => m.WriteSql(writer));
            writer.Write(')');
        }
        /// <summary>
        /// 写入<see cref="CreateViewFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForCreateView(SqlWriter writer, ISqlFragment fragment)
        {
            var relation = (CreateViewFragment)fragment;
            writer.Write("CREATE VIEW ");
            relation.Name.WriteSql(writer);
            writer.WriteLine(" AS ");
            writer.Write(relation.Content);
        }
        /// <summary>
        /// 写入<see cref="CreateRelationFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForCreateRelation(SqlWriter writer, ISqlFragment fragment)
        {
            void writerelationaction(EReferenceAction action, SqlWriter w)
            {
                switch (action)
                {
                    case EReferenceAction.Cascade: w.Write("CASCADE"); break;
                    case EReferenceAction.SetNull: w.Write("SET NULL"); break;
                    case EReferenceAction.SetDefault: w.Write("SET DEFAULT"); break;
                    case EReferenceAction.NoAction: w.Write("NO ACTION"); break;
                }
            }
            var relation = (CreateRelationFragment)fragment;

            writer.Write("ALTER TABLE ");
            relation.Foreign.WriteSql(writer);
            writer.Write(" ADD CONSTRAINT ");
            relation.Name.WriteSql(writer);
            writer.Write(" FOREIGN KEY(");
            relation.ForeignKeys.ForEach(() => writer.Write(','), a => WriteDbName(writer, a));
            writer.Write(") REFERENCES ");
            relation.Principal.WriteSql(writer);

            writer.Write(" (");
            relation.PrincipalKeys.ForEach(() => writer.Write(','), a => WriteDbName(writer, a));
            writer.Write(')');

            if (relation.Delete.HasValue)
            {
                writer.Write(" ON DELETE");
                writerelationaction(relation.Delete.Value, writer);
            }
            if (relation.Update.HasValue)
            {
                writer.Write(" ON UPDATE");
                writerelationaction(relation.Update.Value, writer);
            }
        }
        /// <summary>
        /// 写入<see cref="CreateRelationFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForDropRelation(SqlWriter writer, ISqlFragment fragment)
        {
            var relation = (DropRelationFragment)fragment;
            writer.Write("ALTER TABLE ");
            relation.Foreign.WriteSql(writer);
            writer.Write(" DROP CONSTRAINT ");
            relation.Name.WriteSql(writer);
        }
        /// <summary>
        /// 写入<see cref="DropObjectFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForDropObject(SqlWriter writer, ISqlFragment fragment)
        {
            var relation = (DropObjectFragment)fragment;
            writer.Write("DROP ");
            switch (relation.Kind)
            {
                case EDatabaseObject.Table: writer.Write("TABLE "); break;
                case EDatabaseObject.View: writer.Write("VIEW "); break;
                case EDatabaseObject.Index: writer.Write("INDEX "); break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, relation.Kind));
            }
            relation.Name.WriteSql(writer);
        }
        /// <summary>
        /// 写入<see cref="RenameObjectFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForRenameObject(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (RenameObjectFragment)fragment;
            writer.Write("RENAME ");
            switch (content.Kind)
            {
                case EDatabaseObject.Table: writer.Write("TABLE "); break;
                case EDatabaseObject.View: writer.Write("VIEW "); break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, content.Kind));
            }
            content.Name.WriteSql(writer);
            writer.Write(" TO ");
            WriteDbName(writer, content.NewName);
        }
    }
    public partial class FragmentWriterBase
    {
        /// <summary>
        /// 写入<see cref="BinaryLogicalFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForBinaryLogical(SqlWriter writer, ISqlFragment fragment)
        {
            var logic = (BinaryLogicalFragment)fragment;
            var key = logic.Kind == EBinaryKind.AndAlso ? " AND " : " OR ";
            logic.Expressions.ForEach(() => writer.Write(key),
                exp =>
                {
                    if (exp is BinaryLogicalFragment)
                    {
                        writer.Write('(');
                        WriteFragmentForBinaryLogical(writer, exp);
                        writer.Write(')');
                    }
                    else
                    {
                        WriteFragment(writer, exp);
                    }
                });
        }
        /// <summary>
        /// 写入<see cref="ConstantFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForConstant(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (ConstantFragment)fragment;
            WriteParameterName(writer, content.Context.GetParameter(content.Value));
        }
        /// <summary>
        /// 写入<see cref="ConstantListFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForConstantList(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (ConstantListFragment)fragment;
            var context = content.Context;
            content.Value.ForEach(() => writer.Write(", "), val => WriteParameterName(writer, context.GetParameter(val)));
        }
        /// <summary>
        /// 写入<see cref="ValueListFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForValueList(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (ValueListFragment)fragment;
            var context = content.Context;
            var loader = content.Member.Loader;
            content.Value.ForEach(() => writer.Write(", "),
                val =>
                {
                    loader.Load(val);
                    content.Member.WriteSql(writer);
                });
        }
        /// <summary>
        /// 写入<see cref="SortFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForSort(SqlWriter writer, ISqlFragment fragment)
        {
            var sort = (SortFragment)fragment;
            sort.Content.WriteSql(writer);
            if (sort.Kind == EOrderKind.Ascending)
                writer.Write(" ASC");
            else
                writer.Write(" DESC");
        }
        /// <summary>
        /// 写入<see cref="ColumnFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForColumnMember(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (ColumnFragment)fragment;
            writer.Write(content.Owner.AliasName);
            writer.Write('.');
            WriteDbName(writer, content.ColumnName);
        }
        /// <summary>
        /// 写入<see cref="ExpressionMemberFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForExpressionMember(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (ExpressionMemberFragment)fragment;
            content.Expression.WriteSql(writer);
            if (string.IsNullOrEmpty(content.AliasName))
            {
                writer.Write(" AS ");
                WriteDbName(writer, content.Property.Name);
            }
        }
        /// <summary>
        /// 写入<see cref="ReferenceMemberFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForReferenceMember(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (ReferenceMemberFragment)fragment;
            if (content.Owner == writer.Current)
                writer.Write(content.Reference.Owner.AliasName);
            else
                writer.Write(content.Owner.AliasName);
            writer.Write('.');
            WriteDbName(writer, content.Reference.OutputName);
        }
        /// <summary>
        /// 写入<see cref="CommitMemberFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForCommitMember(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (CommitMemberFragment)fragment;
            WriteParameterName(writer, content.Context.GetParameter(content.Loader[content.Index]));
        }
        /// <summary>
        /// 写入<see cref="TableFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForTable(SqlWriter writer, ISqlFragment fragment)
        {
            var table = (TableFragment)fragment;
            if (table.Name != null)
            {
                table.Name.WriteSql(writer);
            }
            else
            {
                WriteDbObject(writer, table.Metadata.Name, table.Metadata.Schema);
            }
        }
        /// <summary>
        /// 写入<see cref="InheritFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForInherit(SqlWriter writer, ISqlFragment fragment)
        {
            var table = (InheritFragment)fragment;
            table.Tables.ForEach(() => writer.WriteLine(),
                source => WriteFragmentForSource(writer, source));
        }
        /// <summary>
        /// 写入<see cref="SetFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForSet(SqlWriter writer, ISqlFragment fragment)
        {
            var select = (SetFragment)fragment;
            select.Sources.ForEach(
                source => source.WriteSql(writer),
                source =>
                {
                    writer.WriteLine();
                    switch (select[source])
                    {
                        case EConnectKind.Concat:
                            writer.WriteLine("UNION ALL");
                            break;
                        case EConnectKind.Except:
                            writer.WriteLine("EXCEPT");
                            break;
                        case EConnectKind.Intersect:
                            writer.WriteLine("INTERSECT");
                            break;
                        case EConnectKind.Union:
                            writer.WriteLine("UNION");
                            break;
                    }
                    source.WriteSql(writer);
                });
        }
        /// <summary>
        /// 写入<see cref="BlockFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForBlock(SqlWriter writer, ISqlFragment fragment)
        {
            var block = (BlockFragment)fragment;
            block.ForEach(writer.WriteLine, a =>
            {
                a.WriteSql(writer);
                if (a.HasTerminator)
                {
                    WriteTerminator(writer, a);
                }
            });
        }
        /// <summary>
        /// 写入<see cref="TemporaryTableFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForTemporaryTable(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (TemporaryTableFragment)fragment;
            content.Name.WriteSql(writer);
        }
        /// <summary>
        /// 写入<see cref="RowIndexFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForRowIndex(SqlWriter writer, ISqlFragment fragment)
        {
            var rowIndex = (RowIndexFragment)fragment;
            writer.Write((rowIndex.Index++));
        }
        /// <summary>
        /// 写入<see cref="DefaultFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForDefault(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (DefaultFragment)fragment;
            writer.Write("DEFAULT");
        }
        /// <summary>
        /// 写入<see cref="StringConcatFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForStringConcat(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (StringConcatFragment)fragment;
            content.Left.WriteSql(writer);
            writer.Write(" || ");
            content.Right.WriteSql(writer);
        }
        /// <summary>
        /// 写入<see cref="UnaryFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForUnary(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (UnaryFragment)fragment;
            switch (content.Kind)
            {
                case Expressions.EUnaryKind.UnaryPlus:
                    writer.Write('-');
                    content.Expresssion.WriteSql(writer);
                    break;
                case Expressions.EUnaryKind.Negate:
                    writer.Write('+');
                    content.Expresssion.WriteSql(writer);
                    break;
                case Expressions.EUnaryKind.Not:
                    writer.Write("NOT(");
                    content.Expresssion.WriteSql(writer);
                    writer.Write(')');
                    break;
                case Expressions.EUnaryKind.Convert:
                    writer.Write("CAST(");
                    content.Expresssion.WriteSql(writer);
                    writer.Write(" AS ");
                    WriteDbDataType(writer, content.Type);
                    writer.Write(')');
                    break;
            }
        }
        /// <summary>
        /// 写入<see cref="RepeatBlockFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForWhile(SqlWriter writer, ISqlFragment fragment)
        {
            var whileFragment = (WhileFragment)fragment;
            writer.Write("WHILE ");
            writer.Write('(');
            whileFragment.Condition.WriteSql(writer);
            writer.WriteLine(")");
            writer.WriteLine("BEGIN");
            whileFragment.Block.WriteSql(writer);
            writer.WriteLine();
            writer.Write("END");
        }
        /// <summary>
        /// 写入<see cref="RepeatBlockFragment"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteFragmentForRepeatBlock(SqlWriter writer, ISqlFragment fragment)
        {
            var repeat = (RepeatBlockFragment)fragment;
            var loader = repeat.Loader;
            if (loader == null)
            {
                foreach (var item in repeat.Items)
                {
                    repeat.Block.WriteSql(writer);
                }
            }
            else
            {
                foreach (var item in repeat.Items)
                {
                    loader.Load(item);
                    repeat.Block.WriteSql(writer);
                }
            }
        }
    }
}