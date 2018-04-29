// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Commands;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Providers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    partial class OracleFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<Type, WriteFragmentDelegate> InitialMethodsForWriteFragment()
        {
            var dictionary = base.InitialMethodsForWriteFragment();
            dictionary.AddOrUpdate(typeof(CreateColumnFragment), WriteFragmentForCreateColumn);
            dictionary.AddOrUpdate(typeof(ObjectExsitFragment), WriteFragmentForObjectExsit);
            return dictionary;
        }
        private void WriteFragmentForObjectExsit(SqlWriter writer, ISqlFragment fragment)
        {
            var exist = (ObjectExsitFragment)fragment;
            var schema = exist.Context.Feature.DefaultSchema;
            if (fragment is INameSchemaFragment nameschema && !string.IsNullOrEmpty(nameschema.Schema))
            {
                schema = nameschema.Schema;
            }
            schema = schema.ToUpper();
            writer.Write("SELECT CASE WHEN EXISTS (");
            switch (exist.Kind)
            {
                case Operates.EDatabaseObject.Table:
                    writer.Write("SELECT 1 FROM SYS.ALL_TABLES t WHERE t.OWNER = '");
                    writer.Write(schema);
                    writer.Write("' AND t.TABLE_NAME='");
                    writer.Write(exist.Name.Name);
                    writer.Write('\'');
                    break;
                case Operates.EDatabaseObject.View:
                    writer.Write("SELECT 1 FROM SYS.ALL_VIEWS t WHERE t.OWNER = '");
                    writer.Write(schema);
                    writer.Write("' AND t.VIEW_NAME='");
                    writer.Write(exist.Name.Name);
                    writer.Write('\'');
                    break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, exist.Kind));
            }
            writer.Write(") THEN 1 ELSE 0 END FROM DUAL");
        }
        private void WriteFragmentForCreateColumn(SqlWriter writer, ISqlFragment fragment)
        {
            var create = (CreateColumnFragment)fragment;
            WriteDbName(writer, create.Name);
            writer.Write(' ');
            var column = create.Metadata;
            WriteDbDataType(writer, column);
            if (create.Table.IsTemporary)
            {
                writer.Write(" NULL");
            }
            else
            {
                var computed = column.GetProperty<ComputedAttribute>();
                if (computed != null)
                {
                    writer.Write(" GENERATED ALWAYS");
                    writer.Write(" AS (");
                    writer.Write(computed.Expression);
                    writer.Write(") VIRTUAL");
                }
                else
                {
                    var def = column.GetProperty<DefaultAttribute>();
                    var nullable = column.GetProperty<NullableAttribute>();
                    if (nullable != null)
                    {
                        if (!nullable.Value) writer.Write(" NOT");
                    }
                    else
                    {
                        var type = column.Member.PropertyType;
                        if (type.IsValueType && !type.IsNullable())
                        {
                            writer.Write(" NOT");
                        }
                    }
                    writer.Write(" NULL");
                    if (def != null)
                    {
                        writer.Write(" DEFAULT (" + def.Content + ")");
                    }
                }
            }
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForCreateTemporaryTable(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (CreateTempTableFragment)fragment;
            if (content.IsVariable)
            {
                throw new NotSupportedException(Res.NotSupportedWriteTableVariable);
            }
            var create = (CreateTempTableFragment)fragment;
            writer.Write($"CREATE GLOBAL TEMPORARY TABLE ");
            create.Table.WriteSql(writer);
            writer.WriteLine("(");
            create.Members.ForEach(() => writer.WriteLine(", "),
                m => m.WriteSql(writer));
            writer.Write(')');
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForRenameObject(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (RenameObjectFragment)fragment;
            writer.Write("RENAME ");
            content.Name.WriteSql(writer);
            writer.Write(" TO ");
            WriteDbName(writer, content.NewName);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForSelect(SqlWriter writer, ISqlFragment fragment)
        {
            var select = (SelectFragment)fragment;
            writer.Enter(delegate ()
            {
                if (select.Skip > 0)
                {
                    var rownumber = select.Members.Select(a => a.OutputName).Unique("RowIndex"); ;
                    if (select.Take > 0)
                    {
                        WriteFragmentForSelectTake(writer, select, rownumber, delegate ()
                        {
                            WriteFragmentForSelectSkip(writer, select, rownumber, delegate ()
                            {
                                WriteFragmentForSelectRowNumber(writer, select, rownumber);
                            });
                        });
                    }
                    else
                    {
                        WriteFragmentForSelectSkip(writer, select, rownumber, delegate ()
                        {
                            WriteFragmentForSelectRowNumber(writer, select, rownumber);
                        }, true);
                    }
                }
                else
                {
                    if (select.Take > 0)
                    {
                        var alias = fragment.Context.GetDataSourceAlias();
                        writer.WriteLine("SELECT ");
                        writer.Write(alias);
                        writer.WriteLine(".* FROM (");
                        WriteFragmentForSelectSimple(writer, select);
                        writer.WriteLine();
                        writer.Write(") ");
                        writer.Write(alias);
                        writer.Write(" WHERE ROWNUM <= ");
                        writer.Write(select.Take);
                    }
                    else
                    {
                        WriteFragmentForSelectSimple(writer, select);
                    }
                }
            }, select);
        }
        private void WriteFragmentForSelectRowNumber(SqlWriter writer, SelectFragment select, string rownumber)
        {
            writer.Write("SELECT");
            if (select.Distinct) writer.Write(" DISTINCT");
            writer.WriteLine();
            WriteFragmentForSelectMembers(writer, select.Members);
            writer.Write(", ROW_NUMBER() OVER ( ");
            WriteFragmentForOrderBy(writer, select.Sorts);
            writer.Write(" ) ");
            writer.Write(rownumber);
            WriteFragmentForFrom(writer, select.Sources);
            WriteFragmentForWhere(writer, select.Where);
            WriteFragmentForGroupBy(writer, select.GroupBys);
        }
        private void WriteFragmentForSelectSkip(SqlWriter writer, SelectFragment select, string rownumber, Action action, bool islistmember = false)
        {
            var alias = select.Context.GetDataSourceAlias();
            if (islistmember)
            {
                writer.Write("SELECT ");
                select.Members.ForEach(() => writer.Write(", "), member =>
                {
                    writer.Write(alias);
                    writer.Write('.');
                    WriteDbName(writer, member.OutputName);
                });
                writer.Write(" FROM ( ");
            }
            else
            {
                writer.Write("SELECT * FROM ( ");
            }
            action();
            writer.Write(" ) ");
            writer.Write(alias);
            writer.Write(" WHERE ");
            writer.Write(alias);
            writer.Write('.');
            writer.Write(rownumber);
            writer.Write(" > ");
            writer.Write(select.Skip);
        }
        private void WriteFragmentForSelectTake(SqlWriter writer, SelectFragment select, string rownumber, Action action)
        {
            var alias2 = select.Context.GetDataSourceAlias();

            writer.Write("SELECT ");
            select.Members.ForEach(() => writer.Write(", "), member =>
            {
                writer.Write(alias2);
                writer.Write('.');
                WriteDbName(writer, member.OutputName);
            });
            writer.Write(" FROM ( ");
            action();
            writer.Write(" ) ");
            writer.Write(alias2);
            writer.Write(" WHERE ");
            writer.Write(alias2);
            writer.Write('.');
            writer.Write(rownumber);
            writer.Write(" <= ");
            writer.Write((select.Take + select.Skip));
        }
        private void WriteFragmentForSelectSimple(SqlWriter writer, SelectFragment select)
        {
            writer.Write("SELECT");
            if (select.Distinct) writer.Write(" DISTINCT");
            writer.WriteLine();
            WriteFragmentForSelectMembers(writer, select.Members);
            WriteFragmentForFrom(writer, select.Sources);
            WriteFragmentForWhere(writer, select.Where);
            WriteFragmentForGroupBy(writer, select.GroupBys);
            WriteFragmentForOrderBy(writer, select.Sorts);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForInsertValue(SqlWriter writer, ISqlFragment fragment)
        {
            var insert = (InsertValueFragment)fragment;
            writer.Write("INSERT INTO ");
            insert.Target.WriteSql(writer);
            writer.WriteLine(); writer.Write('(');
            insert.Members.ForEach(() => writer.Write(","),
                column => this.WriteDbName(writer, column.OutputName));
            writer.WriteLine(")"); writer.Write("VALUES");
            writer.Write('(');
            insert.Values.ForEach(() => writer.Write(","), val => val.WriteSql(writer));
            writer.Write(')');
            if (insert.ReturnMembers.Any())
            {
                var command = fragment.Context.Data.OperateCommand.GetCustomCommand<IOracleCustomCommand>();

                writer.Write(" RETURNING ");
                insert.ReturnMembers.OfType<CommitMemberFragment>().ForEach(
                    () => writer.Write(','),
                    m =>
                    {
                        var member = insert.Target.GetMember(m.Property);
                        WriteDbName(writer, member.OutputName);
                    });
                writer.Write(" INTO ");
                insert.ReturnMembers.ForEach(() => writer.Write(','),
                    m => WriteParameterName(writer, command.GetParameter(m.Property, true)));
            }
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForCommitMember(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (CommitMemberFragment)fragment;
            var command = fragment.Context.Data.OperateCommand.GetCustomCommand<IOracleCustomCommand>();
            WriteParameterName(writer, command.GetParameter(content.Property, false));
        }
    }
    partial class OracleFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<MemberInfo, WriteFragmentDelegate> InitialMethodsForWriteScalar()
        {
            var result = base.InitialMethodsForWriteScalar();
            result.AddOrUpdate(SupportMembers.Guid.NewGuid, (w, e) => w.Write("SYS_GUID()"));

            result.AddOrUpdate(SupportMembers.DateTime.Now, (w, e) => w.Write("SYSDATE"));

            result.AddOrUpdate(SupportMembers.DbFunctions.SequenceNext, (w, e) =>
            {
                var scalar = (ScalarFragment)e;
                var nameArgu = (ConstantFragment)scalar.Arguments[0];
                var schemaArgu = (ConstantFragment)scalar.Arguments[1];
                var schema = schemaArgu.Value == null ? string.Empty : schemaArgu.Value.ToString();
                WriteDbObject(w, nameArgu.Value.ToString(), schema);
                w.Write(".NEXTVAL");
            });
            result.AddOrUpdate(SupportMembers.DbFunctions.SequenceValue, (w, e) =>
            {
                var scalar = (ScalarFragment)e;
                var nameArgu = (ConstantFragment)scalar.Arguments[0];
                var schemaArgu = (ConstantFragment)scalar.Arguments[1];
                var schema = schemaArgu.Value == null ? string.Empty : schemaArgu.Value.ToString();
                WriteDbObject(w, nameArgu.Value.ToString(), schema);
                w.Write(".CURRVAL");
            });



            return result;
        }
    }
}
