// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    partial class PostgreSQLFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<Type, WriteFragmentDelegate> InitialMethodsForWriteFragment()
        {
            var dictionary = base.InitialMethodsForWriteFragment();

            dictionary.AddOrUpdate(typeof(ValuesFragment), WriteFragmentForValues);
            dictionary.AddOrUpdate(typeof(CreateColumnFragment), WriteFragmentForCreateColumn);
            dictionary.AddOrUpdate(typeof(ObjectExsitFragment), WriteFragmentForObjectExsit);

            return dictionary;
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForCreateTable(SqlWriter writer, ISqlFragment fragment)
        {
            var create = (CreateTableFragment)fragment;
            var metadata = create.Metadata;

            writer.Write("CREATE TABLE ");
            create.Name.WriteSql(writer);
            writer.Write('(');
            create.Members.ForEach(() => writer.WriteLine(", "),
              m => m.WriteSql(writer));
            var keys = metadata.Keys;
            writer.WriteLine(",");
            writer.Write("PRIMARY KEY ( ");
            keys.ForEach(() => writer.Write(","), key => WriteDbName(writer, key.Name));
            writer.WriteLine(")");
            writer.Write(")");
            var length = metadata.InheritSets.Length;
            if (length > 0)
            {
                writer.Write(" INHERITS (");
                var inherit = metadata.InheritSets[length - 1];
                new ObjectNameFragment(create.Context, inherit.Name, inherit.Schema).WriteSql(writer);
                writer.Write(")");
            }
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForRenameObject(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (RenameObjectFragment)fragment;
            writer.Write("ALTER ");
            switch (content.Kind)
            {
                case EDatabaseObject.Table: writer.Write("TABLE "); break;
                case EDatabaseObject.View: writer.Write("VIEW "); break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, content.Kind));
            }
            content.Name.WriteSql(writer);
            writer.Write(" RENAME TO ");
            WriteDbName(writer, content.NewName);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForInsertValue(SqlWriter writer, ISqlFragment fragment)
        {
            base.WriteFragmentForInsertValue(writer, fragment);
            var insert = (InsertValueFragment)fragment;
            OutputReturnMembers(writer, insert.ReturnMembers);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForUpdate(SqlWriter writer, ISqlFragment fragment)
        {
            var update = (UpdateFragment)fragment;

            writer.Write("UPDATE ");
            WriteFragmentForSource(writer, update.Target);
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
            var sources = update.Sources.Where(a => a != update.Target).ToArray();
            IExpressionFragment joinfilter = null;
            if (sources.Length > 0)
            {
                writer.WriteLine();
                writer.Write("FROM ");
                if (sources[0].Join.HasValue)
                {
                    joinfilter = sources[0].Condition;
                    WriteFragmentForSourceContent(writer, sources[0]);
                    for (int i = 1; i < sources.Length; i++)
                    {
                        writer.WriteLine();
                        WriteFragmentForSource(writer, sources[i]);
                    }
                }
                else
                {
                    sources.ForEach(() => writer.WriteLine(),
                        source => WriteFragmentForSource(writer, source));
                }
            }
            if (joinfilter != null)
            {
                writer.WriteLine();
                writer.Write("WHERE ");
                if (update.Where != null)
                {
                    joinfilter.Merge(update.Where).WriteSql(writer);
                }
                else
                {
                    joinfilter.WriteSql(writer);
                }
            }
            else
            {
                WriteFragmentForWhere(writer, update.Where);
            }
            OutputReturnMembers(writer, update.ReturnMembers, update.Target);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForDelete(SqlWriter writer, ISqlFragment fragment)
        {
            var current = (DeleteFragment)fragment;
            writer.Write("DELETE FROM ");
            WriteFragmentForSourceContent(writer, current.Target);
            var sources = current.Sources.Where(a => a != current.Target).ToArray();
            if (sources.Length > 0)
            {
                writer.WriteLine();
                writer.Write("USING ");
                sources.ForEach(() => writer.Write(","), source => WriteFragmentForSourceContent(writer, source));

                var conditions = sources.Where(a => a.Condition != null).Select(a => a.Condition).ToList();
                if (conditions.Count > 0 || current.Where != null)
                {
                    writer.WriteLine();
                    writer.Write("WHERE ");
                    if (current.Where != null)
                    {
                        if (conditions.Count > 0)
                        {
                            current.Where.Merge(conditions).WriteSql(writer);
                        }
                        else
                        {
                            current.Where.WriteSql(writer);
                        }
                    }
                    else if (conditions.Count > 0)
                    {
                        conditions.Merge().WriteSql(writer);
                    }
                }
            }
            else
            {
                WriteFragmentForWhere(writer, current.Where);
            }
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForSelect(SqlWriter writer, ISqlFragment fragment)
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
                }
                if (select.Skip > 0)
                {
                    writer.WriteLine();
                    writer.Write("OFFSET ");
                    writer.Write(select.Skip);
                }
            }, select);
        }
        private void WriteFragmentForObjectExsit(SqlWriter writer, ISqlFragment fragment)
        {
            var exist = (ObjectExsitFragment)fragment;
            writer.Write("SELECT CASE WHEN EXISTS (");
            var schema = exist.Context.Feature.DefaultSchema;
            if (fragment is INameSchemaFragment nameschema && !string.IsNullOrEmpty(nameschema.Schema))
            {
                schema = nameschema.Schema;
            }
            writer.Write("SELECT 1 FROM information_schema.");
            switch (exist.Kind)
            {
                case EDatabaseObject.Table: writer.Write("TABLES"); break;
                case EDatabaseObject.View: writer.Write("VIEWS"); break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, exist.Kind));
            }
            writer.Write(" t WHERE t.TABLE_SCHEMA='");
            writer.Write(schema);
            writer.Write("' AND t.TABLE_NAME='");
            writer.Write(exist.Name.Name);
            writer.Write('\'');
            writer.Write(") THEN CAST(1 AS BOOLEAN) ELSE CAST(0 AS BOOLEAN) END");
        }
        private void WriteFragmentForCreateColumn(SqlWriter writer, ISqlFragment fragment)
        {
            var create = (CreateColumnFragment)fragment;
            WriteDbName(writer, create.Name);
            writer.Write(' ');
            var column = create.Metadata;
            WriteDbDataType(writer, column);
            if (!create.Table.IsTemporary)
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
                    writer.Write(" DEFAULT " + def.Content);
                }
            }
        }
        private void WriteFragmentForValues(SqlWriter writer, ISqlFragment fragment)
        {
            var values = (ValuesFragment)fragment;
            writer.Write("(VALUES");
            var loader = values.Source.Loader;
            values.Data.ForEach(() => writer.WriteLine(","),
                item =>
                {
                    loader.Load(item);
                    writer.Write('(');
                    values.Values.ForEach(() => writer.Write(","), val => val.WriteSql(writer));
                    writer.Write(')');
                });
            writer.Write(") AS ");
            writer.Write(values.AliasName);
            writer.Write(" (");
            values.Members.ForEach(
                () => writer.Write(","),
                column => this.WriteDbName(writer, column.OutputName));
            writer.WriteLine(")");
        }

        private void OutputReturnMembers(SqlWriter writer, List<IMemberFragment> members, ISourceFragment target = null)
        {
            if (members.Count > 0)
            {
                writer.WriteLine();
                writer.Write("RETURNING ");
                members.ForEach(
                    () => writer.Write(","),
                    column =>
                    {
                        if (target != null)
                        {
                            writer.Write(target.AliasName);
                            writer.Write('.');
                        }
                        this.WriteDbName(writer, column.OutputName);
                    });
            }
        }
    }
    partial class PostgreSQLFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<MemberInfo, WriteFragmentDelegate> InitialMethodsForWriteScalar()
        {
            void dateadd(SqlWriter writer, ISqlFragment fragment, string key)
            {
                var scalar = (ScalarFragment)fragment;
                writer.Write("DATEADD(");
                writer.Write(key);
                writer.Write(',');
                scalar.Arguments[1].WriteSql(writer);
                writer.Write(',');
                scalar.Arguments[0].WriteSql(writer);
                writer.Write(')');
            }
            void datepart(SqlWriter writer, ISqlFragment fragment, string key)
            {
                var scalar = (ScalarFragment)fragment;
                writer.Write("DATEPART(");
                writer.Write(key);
                writer.Write(',');
                scalar.Arguments[1].WriteSql(writer);
                writer.Write(',');
                scalar.Arguments[0].WriteSql(writer);
                writer.Write(')');
            }
            var result = base.InitialMethodsForWriteScalar();
            //DateTime函数初始化
            result.AddOrUpdate(SupportMembers.DateTime.Now, (w, e) => w.Write("LOCALTIMESTAMP"));
            result.AddOrUpdate(SupportMembers.DateTime.UtcNow, (w, e) => w.Write("CURRENT_TIMESTAMP"));
            result.AddOrUpdate(SupportMembers.DateTime.DayOfYear, (w, e) => datepart(w, e, "DAYOFYEAR"));
            result.AddOrUpdate(SupportMembers.DateTime.DayOfWeek, (w, e) => datepart(w, e, "WEEKDAY"));
            result.AddOrUpdate(SupportMembers.DateTime.Year, (w, e) => datepart(w, e, "YEAR"));
            result.AddOrUpdate(SupportMembers.DateTime.Month, (w, e) => datepart(w, e, "MONTH"));
            result.AddOrUpdate(SupportMembers.DateTime.Day, (w, e) => datepart(w, e, "DAY"));
            result.AddOrUpdate(SupportMembers.DateTime.Hour, (w, e) => datepart(w, e, "HOUR"));
            result.AddOrUpdate(SupportMembers.DateTime.Minute, (w, e) => datepart(w, e, "MINUTE"));
            result.AddOrUpdate(SupportMembers.DateTime.Second, (w, e) => datepart(w, e, "SECOND"));
            result.AddOrUpdate(SupportMembers.DateTime.Millisecond, (w, e) => datepart(w, e, "MILLISECOND"));
            result.AddOrUpdate(SupportMembers.DateTime.AddYears, (w, e) => dateadd(w, e, "YEAR"));
            result.AddOrUpdate(SupportMembers.DateTime.AddMonths, (w, e) => dateadd(w, e, "MONTH"));
            result.AddOrUpdate(SupportMembers.DateTime.AddDays, (w, e) => dateadd(w, e, "DAY"));
            result.AddOrUpdate(SupportMembers.DateTime.AddHours, (w, e) => dateadd(w, e, "HOUR"));
            result.AddOrUpdate(SupportMembers.DateTime.AddMinutes, (w, e) => dateadd(w, e, "MINUTE"));
            result.AddOrUpdate(SupportMembers.DateTime.AddSeconds, (w, e) => dateadd(w, e, "SECOND"));
            result.AddOrUpdate(SupportMembers.DateTime.AddMilliseconds, (w, e) => dateadd(w, e, "MILLISECOND"));
            //Math 相关函数初始化
            SupportMembers.Math.Abs.ForEach(m => result.AddOrUpdate(m, (w, e) => WriteScalarForSystemFunction(w, e, "ABS")));
            result.AddOrUpdate(SupportMembers.Math.Acos, (w, e) => WriteScalarForSystemFunction(w, e, "ACOS"));
            result.AddOrUpdate(SupportMembers.Math.Asin, (w, e) => WriteScalarForSystemFunction(w, e, "ASIN"));
            result.AddOrUpdate(SupportMembers.Math.Atan, (w, e) => WriteScalarForSystemFunction(w, e, "ATAN"));
            SupportMembers.Math.Ceiling.ForEach(m => result.AddOrUpdate(m, (w, e) => WriteScalarForSystemFunction(w, e, "CEILING")));
            result.AddOrUpdate(SupportMembers.Math.Cos, (w, e) => WriteScalarForSystemFunction(w, e, "COS"));
            result.AddOrUpdate(SupportMembers.Math.Exp, (w, e) => WriteScalarForSystemFunction(w, e, "EXP"));
            SupportMembers.Math.Floor.ForEach(m => result.AddOrUpdate(m, (w, e) => WriteScalarForSystemFunction(w, e, "FLOOR")));
            result.AddOrUpdate(SupportMembers.Math.Log[0], (w, e) => WriteScalarForSystemFunction(w, e, "LOG"));
            result.AddOrUpdate(SupportMembers.Math.Log[1], (w, e) => WriteScalarForSystemFunction(w, e, "LOG"));
            result.AddOrUpdate(SupportMembers.Math.Log10, (w, e) => WriteScalarForSystemFunction(w, e, "LOG10"));
            result.AddOrUpdate(SupportMembers.Math.Pow, (w, e) => WriteScalarForSystemFunction(w, e, "POWER"));
            SupportMembers.Math.Sign.ForEach(m => result.AddOrUpdate(m, (w, e) => WriteScalarForSystemFunction(w, e, "SIGN")));
            result.AddOrUpdate(SupportMembers.Math.Sin, (w, e) => WriteScalarForSystemFunction(w, e, "SIN"));
            result.AddOrUpdate(SupportMembers.Math.Tan, (w, e) => WriteScalarForSystemFunction(w, e, "TAN"));
            //String 相关函数初始化
            result.AddOrUpdate(SupportMembers.String.Contains, (w, e) =>
            {
                var scalar = (ScalarFragment)e;
                scalar.Arguments[0].WriteSql(w);
                w.Write(" LIKE '%' + ");
                scalar.Arguments[1].WriteSql(w);
                w.Write(" + '%'");
            });
            result.AddOrUpdate(SupportMembers.String.StartsWith, (w, e) =>
            {
                var scalar = (ScalarFragment)e;
                scalar.Arguments[0].WriteSql(w);
                w.Write(" LIKE + ");
                scalar.Arguments[1].WriteSql(w);
                w.Write(" + '%'");
            });
            result.AddOrUpdate(SupportMembers.String.EndsWith, (w, e) =>
            {
                var scalar = (ScalarFragment)e;
                scalar.Arguments[0].WriteSql(w);
                w.Write(" LIKE '%' + ");
                scalar.Arguments[1].WriteSql(w);
            });
            result.AddOrUpdate(SupportMembers.String.IsNullOrEmpty, (w, e) =>
            {
                var scalar = (ScalarFragment)e;
                w.Write("(");
                scalar.Arguments[0].WriteSql(w);
                w.Write(" IS NULL OR ");
                scalar.Arguments[0].WriteSql(w);
                w.Write(" = '')");
            });
            result.AddOrUpdate(SupportMembers.String.Substring1, (w, e) =>
            {
                var scalar = (ScalarFragment)e;
                w.Write("SUBSTRING(");
                scalar.Arguments[0].WriteSql(w);
                w.Write(',');
                scalar.Arguments[1].WriteSql(w);
                w.Write(',');
                w.Write(int.MaxValue - 1);
                w.Write(')');
            });
            result.AddOrUpdate(SupportMembers.String.Substring2, (w, e) => WriteScalarForSystemFunction(w, e, "SUBSTRING"));
            result.AddOrUpdate(SupportMembers.String.Concat, (w, e) => WriteScalarForSystemFunction(w, e, "CONCAT"));
            result.AddOrUpdate(SupportMembers.String.Length, (w, e) => WriteScalarForSystemFunction(w, e, "LEN"));
            result.AddOrUpdate(SupportMembers.String.ToUpper, (w, e) => WriteScalarForSystemFunction(w, e, "UPPER"));
            result.AddOrUpdate(SupportMembers.String.ToLower, (w, e) => WriteScalarForSystemFunction(w, e, "LOWER"));
            result.AddOrUpdate(SupportMembers.String.TrimEnd, (w, e) => WriteScalarForSystemFunction(w, e, "RTRIM"));
            result.AddOrUpdate(SupportMembers.String.TrimStart, (w, e) => WriteScalarForSystemFunction(w, e, "LTRIM"));
            if (Generator.Version >= 0x0B00)
            {
                result.AddOrUpdate(SupportMembers.String.Trim, (w, e) => WriteScalarForSystemFunction(w, e, "TRIM"));
            }
            else
            {
                result.AddOrUpdate(SupportMembers.String.Trim, (w, e) =>
                {
                    var scalar = (ScalarFragment)e;
                    w.Write("RTRIM(LTRIM(");
                    scalar.Arguments[0].WriteSql(w);
                    w.Write("))");
                });
            }
            //Guid 相关函数初始化
            result.AddOrUpdate(SupportMembers.Guid.NewGuid, (w, e) => w.Write("UUID_IN(MD5(RANDOM()::TEXT || NOW()::TEXT)::CSTRING)"));

            result.AddOrUpdate(SupportMembers.DbFunctions.GetIdentity, (w, e) => w.Write("SCOPE_IDENTITY()"));
            return result;
        }
    }
}
