// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.Providers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    partial class SqlServerCeFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<Type, WriteFragmentDelegate> InitialMethodsForWriteFragment()
        {
            var dictionary = base.InitialMethodsForWriteFragment();

            dictionary.AddOrUpdate(typeof(CreateColumnFragment), WriteFragmentForCreateColumn);
            dictionary.AddOrUpdate(typeof(ObjectExsitFragment), WriteFragmentForObjectExsit);

            dictionary.Remove(typeof(CreateViewFragment));

            return dictionary;
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForRenameObject(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (RenameObjectFragment)fragment;
            if (content.Kind != Operates.EDatabaseObject.Table)
            {
                throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, content.Kind));
            }
            writer.Write("EXEC sp_rename '");
            writer.Write(content.Name.Name);
            writer.Write("', '");
            writer.Write(content.NewName);
            writer.Write('\'');
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForDropObject(SqlWriter writer, ISqlFragment fragment)
        {
            var relation = (DropObjectFragment)fragment;
            writer.Write("DROP ");
            switch (relation.Kind)
            {
                case EDatabaseObject.Table: writer.Write("TABLE "); break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, relation.Kind));
            }
            relation.Name.WriteSql(writer);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForSelect(SqlWriter writer, ISqlFragment fragment)
        {
            var select = (SelectFragment)fragment;
            writer.Enter(delegate ()
            {
                writer.Write("SELECT");
                if (select.Distinct) writer.Write(" DISTINCT");
                if (select.Take > 0 && select.Skip <= 0)
                {
                    writer.Write(" TOP ");
                    writer.Write(select.Take);
                }
                writer.WriteLine();
                WriteFragmentForSelectMembers(writer, select.Members);
                WriteFragmentForFrom(writer, select.Sources);
                WriteFragmentForWhere(writer, select.Where);
                WriteFragmentForGroupBy(writer, select.GroupBys);
                WriteFragmentForOrderBy(writer, select.Sorts);
                if (select.Skip > 0)
                {
                    writer.WriteLine();
                    writer.Write("OFFSET " + select.Skip.ToString() + " ROWS");
                    if (select.Take > 0)
                    {
                        writer.WriteLine();
                        writer.Write("FETCH NEXT " + select.Take.ToString() + " ROWS ONLY");
                    }
                }
            }, select);
        }
        private void WriteFragmentForCreateColumn(SqlWriter writer, ISqlFragment fragment)
        {
            var create = (CreateColumnFragment)fragment;
            WriteDbName(writer, create.Name);
            writer.Write(' ');
            var column = create.Metadata;
            WriteDbDataType(writer, column);
            if (create.Table.IsTemporary)
                writer.Write(" NULL");
            else
            {
                var identity = column.GetProperty<IdentityAttribute>();
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
                if (identity != null)
                {
                    writer.Write(" IDENTITY (1, 1)");
                }
                else if (def != null)
                {
                    writer.Write(" DEFAULT (" + def.Content + ")");
                }
            }
        }
        private void WriteFragmentForObjectExsit(SqlWriter writer, ISqlFragment fragment)
        {
            var exist = (ObjectExsitFragment)fragment;
            writer.Write("SELECT CASE WHEN EXISTS (");
            switch (exist.Kind)
            {
                case EDatabaseObject.Table:
                    writer.Write("SELECT 1 FROM INFORMATION_SCHEMA.TABLES t WHERE t.TABLE_TYPE='TABLE' AND t.TABLE_NAME='");
                    writer.Write(exist.Name.Name);
                    writer.Write('\'');
                    break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, exist.Kind));
            }
            writer.Write(") THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END");
        }
    }
    partial class SqlServerCeFragmentWriter
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
            result.AddOrUpdate(SupportMembers.DateTime.Now, (w, e) => w.Write("GETDATE()"));
            result.AddOrUpdate(SupportMembers.DateTime.UtcNow, (w, e) => w.Write("GETUTCDATE()"));
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
            result.AddOrUpdate(SupportMembers.String.Trim, (w, e) =>
            {
                var scalar = (ScalarFragment)e;
                w.Write("RTRIM(LTRIM(");
                scalar.Arguments[0].WriteSql(w);
                w.Write("))");
            });
            //Guid 相关函数初始化
            result.AddOrUpdate(SupportMembers.Guid.NewGuid, (w, e) => w.Write("NEWID()"));

            result.AddOrUpdate(SupportMembers.DbFunctions.GetIdentity, (w, e) => w.Write("@@IDENTITY"));
            return result;
        }
    }
}