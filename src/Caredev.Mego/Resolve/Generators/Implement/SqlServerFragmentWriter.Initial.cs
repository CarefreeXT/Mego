// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    partial class SqlServerFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<Type, WriteFragmentDelegate> InitialMethodsForWriteFragment()
        {
            var dictionary = base.InitialMethodsForWriteFragment();
            if (Generator.Version >= 0x0B00)
            {
                dictionary.AddOrUpdate(typeof(SelectFragment), WriteFragmentForSelect2012);
            }
            else
            {
                dictionary.AddOrUpdate(typeof(SelectFragment), WriteFragmentForSelect2005);
            }

            dictionary.AddOrUpdate(typeof(TempTableNameFragment), WriteFragmentForTempTableName);
            dictionary.AddOrUpdate(typeof(CreateColumnFragment), WriteFragmentForCreateColumn);
            dictionary.AddOrUpdate(typeof(CreateVariableFragment), WriteFragmentForCreateVariable);
            dictionary.AddOrUpdate(typeof(ObjectExsitFragment), WriteFragmentForObjectExsit);

            return dictionary;
        }

        /// <inheritdoc/>
        protected override void WriteFragmentForCreateTemporaryTable(SqlWriter writer, ISqlFragment fragment)
        {
            var create = (CreateTempTableFragment)fragment;
            if (create.IsVariable)
            {
                writer.Write($"DECLARE ");
                create.Table.WriteSql(writer);
                writer.Write(" AS TABLE");
            }
            else
            {
                writer.Write($"CREATE TABLE ");
                create.Table.WriteSql(writer);
            }
            writer.WriteLine("(");
            create.Members.ForEach(() => writer.WriteLine(", "),
                m => m.WriteSql(writer));
            writer.Write(')');
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForRenameObject(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (RenameObjectFragment)fragment;
            writer.Write("EXEC sp_rename '");
            content.Name.WriteSql(writer);
            writer.Write("', '");
            writer.Write(content.NewName);
            writer.Write('\'');
        }
        private void WriteFragmentForTempTableName(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (TempTableNameFragment)fragment;
            writer.Write('#');
            writer.Write(content.Name);
        }
        private void WriteFragmentForObjectExsit(SqlWriter writer, ISqlFragment fragment)
        {
            var exist = (ObjectExsitFragment)fragment;
            writer.Write("SELECT CASE WHEN OBJECT_ID('");
            exist.Name.WriteSql(writer);
            writer.Write("','");
            switch (exist.Kind)
            {
                case Operates.EDatabaseObject.CheckConstraint: writer.Write("C"); break;
                case Operates.EDatabaseObject.DefaultConstraint: writer.Write("D"); break;
                case Operates.EDatabaseObject.ForeignKey: writer.Write("F"); break;
                case Operates.EDatabaseObject.TableValuedFunction: writer.Write("IF"); break;
                case Operates.EDatabaseObject.StoredProcedure: writer.Write("P"); break;
                case Operates.EDatabaseObject.Sequence: writer.Write("SO"); break;
                case Operates.EDatabaseObject.Table: writer.Write("U"); break;
                case Operates.EDatabaseObject.View: writer.Write("V"); break;
                case Operates.EDatabaseObject.ScalarFunction: writer.Write("FN"); break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, exist.Kind));
            }
            writer.Write("') IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END");
        }
        private void WriteFragmentForCreateVariable(SqlWriter writer, ISqlFragment fragment)
        {
            var create = (CreateVariableFragment)fragment;
            writer.Write("DECLARE ");
            create.Name.WriteSql(writer);
            writer.Write(" AS ");
            WriteDbDataType(writer, create.ClrType);
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
                var computed = column.GetProperty<ComputedAttribute>();
                if (computed != null)
                {
                    if (computed.IsPersisted) writer.Write(" PERSISTED");
                }
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
        }

        private void WriteFragmentForSelect2005(SqlWriter writer, ISqlFragment fragment)
        {
            var select = (SelectFragment)fragment;
            writer.Enter(delegate ()
            {
                if (select.Skip > 0)
                {
                    var alias = fragment.Context.GetDataSourceAlias();
                    var rownumber = select.Members.Select(a => a.OutputName).Unique("RowIndex"); ;

                    writer.Write("SELECT ");
                    select.Members.ForEach(() => writer.Write(", "), member =>
                    {
                        writer.Write(alias);
                        writer.Write('.');
                        WriteDbName(writer, member.OutputName);
                    });
                    writer.Write(" FROM ( ");
                    WriteFragmentForSelectSimple(writer, select, rownumber);
                    writer.Write(" ) ");
                    writer.Write(alias);
                    writer.Write(" WHERE ");
                    writer.Write(alias);
                    writer.Write('.');
                    writer.Write(rownumber);
                    writer.Write(" > ");
                    writer.Write(select.Skip);
                    if (select.Take > 0)
                    {
                        writer.Write(" AND ");
                        writer.Write(alias);
                        writer.Write('.');
                        writer.Write(rownumber);
                        writer.Write(" <= ");
                        writer.Write((select.Take + select.Skip));
                    }
                }
                else
                {
                    WriteFragmentForSelectSimple(writer, select);
                }
            }, select);
        }
        private void WriteFragmentForSelect2012(SqlWriter writer, ISqlFragment fragment)
        {
            var select = (SelectFragment)fragment;
            writer.Enter(delegate ()
            {
                WriteFragmentForSelectSimple(writer, select);
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
        private void WriteFragmentForSelectSimple(SqlWriter writer, SelectFragment select, string rownumber = "")
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
            if (!string.IsNullOrEmpty(rownumber))
            {
                writer.Write(", ROW_NUMBER() OVER ( ");
                WriteFragmentForOrderBy(writer, select.Sorts);
                writer.Write(" ) AS ");
                writer.Write(rownumber);
            }
            WriteFragmentForFrom(writer, select.Sources);
            WriteFragmentForWhere(writer, select.Where);
            WriteFragmentForGroupBy(writer, select.GroupBys);
            if (string.IsNullOrEmpty(rownumber))
            {
                WriteFragmentForOrderBy(writer, select.Sorts);
            }
        }
    }
    partial class SqlServerFragmentWriter
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
            result.AddOrUpdate(SupportMembers.Guid.NewGuid, (w, e) => w.Write("NEWID()"));

            result.AddOrUpdate(SupportMembers.DbFunctions.GetIdentity, (w, e) => w.Write("SCOPE_IDENTITY()"));
            return result;
        }
    }
}