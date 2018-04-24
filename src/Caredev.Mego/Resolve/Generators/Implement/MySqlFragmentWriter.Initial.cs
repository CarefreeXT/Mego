// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using System.Linq;
    using Caredev.Mego.Resolve.Metadatas;
    using Res = Properties.Resources;
    using Caredev.Mego.Resolve.Operates;

    partial class MySqlFragmentWriter
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
            writer.Write("SELECT CASE WHEN EXISTS (");
            switch (exist.Kind)
            {
                case Operates.EDatabaseObject.Table:
                    writer.Write("SELECT 1 FROM information_schema.TABLES t WHERE t.TABLE_SCHEMA=DATABASE() AND t.TABLE_NAME='");
                    writer.Write(exist.Name.Name);
                    writer.Write('\'');
                    break;
                case Operates.EDatabaseObject.View:
                    writer.Write("SELECT 1 FROM information_schema.VIEWS t WHERE t.TABLE_SCHEMA=DATABASE() AND t.TABLE_NAME='");
                    writer.Write(exist.Name.Name);
                    writer.Write('\'');
                    break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, exist.Kind));
            }
            writer.Write(") THEN TRUE ELSE FALSE END");
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForRenameObject(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (RenameObjectFragment)fragment;
            writer.Write("RENAME ");
            switch (content.Kind)
            {
                case EDatabaseObject.Table:
                case EDatabaseObject.View:
                    writer.Write("TABLE ");
                    break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, content.Kind));
            }
            content.Name.WriteSql(writer);
            writer.Write(" TO ");
            WriteDbName(writer, content.NewName);
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
                if (computed != null && create.Context.Feature.HasCapable(EDbCapable.ComputeColumn))
                {
                    writer.Write(" AS");
                    writer.Write(computed.Expression);
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
                        writer.Write(" AUTO_INCREMENT");
                    }
                    else if (def != null)
                    {
                        writer.Write(" DEFAULT " + def.Content);
                    }
                }
            }
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForUpdate(SqlWriter writer, ISqlFragment fragment)
        {
            var update = (UpdateFragment)fragment;
            writer.Enter(delegate ()
            {
                writer.Write("UPDATE ");
                if (update.Sources.Contains(update.Target) ||
                    update.Sources.OfType<InheritFragment>().Any(a => a.Tables.Contains(update.Target)))
                {
                    update.Sources.ForEach(() => writer.WriteLine(),
                    source => WriteFragmentForSource(writer, source));
                }
                else
                {
                    update.Target.WriteSql(writer);
                    writer.Write(" AS ");
                    writer.Write(update.Target.AliasName);
                    if (update.Sources.Any())
                    {
                        writer.Write(" CROSS JOIN ");
                        update.Sources.ForEach(() => writer.WriteLine(),
                            source => WriteFragmentForSource(writer, source));
                    }
                }
                writer.WriteLine();
                writer.Write("SET ");
                var target = update.Target;
                var values = update.Values.ToArray();
                for (int i = 0; i < update.Members.Count; i++)
                {
                    var member = update.Members[i];
                    var value = values[i];
                    if (i > 0)
                        writer.Write(", ");
                    member.WriteSql(writer);
                    writer.Write(" = ");
                    value.WriteSql(writer);
                }
                if (update.Where != null)
                {
                    writer.WriteLine();
                    writer.Write("WHERE ");
                    update.Where.WriteSql(writer);
                }
            }, update);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForSourceCondition(SqlWriter writer, ISourceFragment source)
        {
            if (source.Condition != null)
            {
                if (source is InheritFragment && source.Join.Value == EJoinType.InnerJoin)
                {
                    writer.Write(" AND ");
                }
                else
                {
                    writer.Write(" ON ");
                }
                source.Condition.WriteSql(writer);
            }
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForUnary(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (UnaryFragment)fragment;
            if (content.Kind == Expressions.EUnaryKind.Convert)
            {
                content.Expresssion.WriteSql(writer);
            }
            else
            {
                base.WriteFragmentForUnary(writer, fragment);
            }
        }
    }
    partial class MySqlFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<MemberInfo, WriteFragmentDelegate> InitialMethodsForWriteScalar()
        {
            void dateadd(SqlWriter writer, ISqlFragment fragment, string key)
            {
                var scalar = (ScalarFragment)fragment;
                writer.Write("ADDDATE(");
                scalar.Arguments[0].WriteSql(writer);
                writer.Write(",INTERVAL ");
                scalar.Arguments[1].WriteSql(writer);
                writer.Write(' ');
                writer.Write(key);
                writer.Write(')');
            }

            var result = base.InitialMethodsForWriteScalar();
            result.AddOrUpdate(SupportMembers.Guid.NewGuid, (w, e) => w.Write("UUID()"));
            result.AddOrUpdate(SupportMembers.DbFunctions.GetIdentity, (w, e) => w.Write("LAST_INSERT_ID()"));



            //DateTime函数初始化
            result.AddOrUpdate(SupportMembers.DateTime.Now, (w, e) => w.Write("NOW()"));
            result.AddOrUpdate(SupportMembers.DateTime.UtcNow, (w, e) => w.Write("UTC_DATE()"));
            result.AddOrUpdate(SupportMembers.DateTime.DayOfYear, (w, e) => WriteScalarForSystemFunction(w, e, "DAYOFYEAR"));
            result.AddOrUpdate(SupportMembers.DateTime.DayOfWeek, (w, e) => WriteScalarForSystemFunction(w, e, "DAYOFWEEK"));
            result.AddOrUpdate(SupportMembers.DateTime.Year, (w, e) => WriteScalarForSystemFunction(w, e, "YEAR"));
            result.AddOrUpdate(SupportMembers.DateTime.Month, (w, e) => WriteScalarForSystemFunction(w, e, "MONTH"));
            result.AddOrUpdate(SupportMembers.DateTime.Day, (w, e) => WriteScalarForSystemFunction(w, e, "DAY"));
            result.AddOrUpdate(SupportMembers.DateTime.Hour, (w, e) => WriteScalarForSystemFunction(w, e, "HOUR"));
            result.AddOrUpdate(SupportMembers.DateTime.Minute, (w, e) => WriteScalarForSystemFunction(w, e, "MINUTE"));
            result.AddOrUpdate(SupportMembers.DateTime.Second, (w, e) => WriteScalarForSystemFunction(w, e, "SECOND"));
            result.AddOrUpdate(SupportMembers.DateTime.Millisecond, (w, e) => WriteScalarForSystemFunction(w, e, "MILLISECOND"));
            result.AddOrUpdate(SupportMembers.DateTime.AddYears, (w, e) => dateadd(w, e, "YEAR"));
            result.AddOrUpdate(SupportMembers.DateTime.AddMonths, (w, e) => dateadd(w, e, "MONTH"));
            result.AddOrUpdate(SupportMembers.DateTime.AddDays, (w, e) => dateadd(w, e, "DAY"));
            result.AddOrUpdate(SupportMembers.DateTime.AddHours, (w, e) => dateadd(w, e, "HOUR"));
            result.AddOrUpdate(SupportMembers.DateTime.AddMinutes, (w, e) => dateadd(w, e, "MINUTE"));
            result.AddOrUpdate(SupportMembers.DateTime.AddSeconds, (w, e) => dateadd(w, e, "SECOND"));
            result.AddOrUpdate(SupportMembers.DateTime.AddMilliseconds, (w, e) => dateadd(w, e, "MILLISECOND"));
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
            result.AddOrUpdate(SupportMembers.String.Substring1, (w, e) => WriteScalarForSystemFunction(w, e, "SUBSTRING"));
            result.AddOrUpdate(SupportMembers.String.Substring2, (w, e) => WriteScalarForSystemFunction(w, e, "SUBSTRING"));
            result.AddOrUpdate(SupportMembers.String.Concat, (w, e) => WriteScalarForSystemFunction(w, e, "CONCAT"));
            result.AddOrUpdate(SupportMembers.String.Length, (w, e) => WriteScalarForSystemFunction(w, e, "LENGTH"));
            result.AddOrUpdate(SupportMembers.String.ToUpper, (w, e) => WriteScalarForSystemFunction(w, e, "UPPER"));
            result.AddOrUpdate(SupportMembers.String.ToLower, (w, e) => WriteScalarForSystemFunction(w, e, "LOWER"));
            result.AddOrUpdate(SupportMembers.String.TrimEnd, (w, e) => WriteScalarForSystemFunction(w, e, "RTRIM"));
            result.AddOrUpdate(SupportMembers.String.TrimStart, (w, e) => WriteScalarForSystemFunction(w, e, "LTRIM"));
            result.AddOrUpdate(SupportMembers.String.Trim, (w, e) => WriteScalarForSystemFunction(w, e, "TRIM"));
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
            return result;
        }
    }
}