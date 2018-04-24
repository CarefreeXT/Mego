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
    partial class SQLiteFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<Type, WriteFragmentDelegate> InitialMethodsForWriteFragment()
        {
            var dictionary = base.InitialMethodsForWriteFragment();
            
            dictionary.AddOrUpdate(typeof(CreateTableFragment), WriteFragmentForCreateTable);
            dictionary.AddOrUpdate(typeof(CreateColumnFragment), WriteFragmentForCreateColumn);
            dictionary.AddOrUpdate(typeof(CreateTempTableFragment), WriteFragmentForCreateTemporaryTable);
            dictionary.AddOrUpdate(typeof(ObjectExsitFragment), WriteFragmentForObjectExsit);
            dictionary.AddOrUpdate(typeof(CreateRelationFragment), WriteFragmentForCreateRelation);
            return dictionary;
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForRenameObject(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (RenameObjectFragment)fragment;
            if(content.Kind!=EDatabaseObject.Table)
            {
                throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, content.Kind));
            }
            writer.Write("ALTER TABLE ");
            content.Name.WriteSql(writer);
            writer.Write(" RENAME TO ");
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
        private void WriteFragmentForObjectExsit(SqlWriter writer, ISqlFragment fragment)
        {
            var exist = (ObjectExsitFragment)fragment;
            writer.Write("SELECT CASE WHEN EXISTS (");
            switch (exist.Kind)
            {
                case Operates.EDatabaseObject.Table:
                    writer.Write("SELECT 1 FROM [sqlite_master] t WHERE t.type='table' AND t.name='");
                    writer.Write(exist.Name.Name);
                    writer.Write('\'');
                    break;
                case Operates.EDatabaseObject.View:
                    writer.Write("SELECT 1 FROM [sqlite_master] t WHERE t.type='view' AND t.name='");
                    writer.Write(exist.Name.Name);
                    writer.Write('\'');
                    break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, exist.Kind));
            }
            writer.Write(") THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END");
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForDelete(SqlWriter writer, ISqlFragment fragment)
        {
            var current = (DeleteFragment)fragment;
            writer.Write("DELETE FROM ");
            current.Target.WriteSql(writer);
            HaveNotMemberAliasName(writer, delegate ()
            {
                WriteFragmentForWhere(writer, current.Where);
            });
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForUpdate(SqlWriter writer, ISqlFragment fragment)
        {
            var update = (UpdateFragment)fragment;
            writer.Write("UPDATE ");
            update.Target.WriteSql(writer);
            writer.WriteLine();
            writer.Write("SET ");
            var values = update.Values.ToArray();
            HaveNotMemberAliasName(writer, delegate ()
            {
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
                WriteFragmentForWhere(writer, update.Where);
            });
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForColumnMember(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (ColumnFragment)fragment;
            if (!writer.GetProperty<bool>(nameof(HaveNotMemberAliasName)))
            {
                writer.Write(content.Owner.AliasName);
                writer.Write('.');
            }
            WriteDbName(writer, content.ColumnName);
        }
        private void HaveNotMemberAliasName(SqlWriter writer, Action action)
        {
            try
            {
                writer.SetProperty(nameof(HaveNotMemberAliasName), true);
                action();
            }
            finally
            {
                writer.SetProperty(nameof(HaveNotMemberAliasName), null);
            }
        }
    }
    partial class SQLiteFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<MemberInfo, WriteFragmentDelegate> InitialMethodsForWriteScalar()
        {
            var result = base.InitialMethodsForWriteScalar();
            result.AddOrUpdate(SupportMembers.DateTime.Now, (w, e) => w.Write("datetime('now')"));
            result.AddOrUpdate(SupportMembers.Guid.NewGuid, (w, e) => w.Write("hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || hex(randomblob(2)) || '-' || hex(randomblob(2)) || '-' || hex(randomblob(6))"));
            result.AddOrUpdate(SupportMembers.DbFunctions.GetIdentity, (w, e) => w.Write("LAST_INSERT_ROWID()"));
            result.AddOrUpdate(SupportMembers.String.Concat, WriteScalarForConcat);
            return result;
        }
        private void WriteScalarForConcat(SqlWriter writer, ISqlFragment fragment)
        {
            var scalar = (ScalarFragment)fragment;
            scalar.Arguments[0].WriteSql(writer);
            writer.Write(" || ");
            scalar.Arguments[1].WriteSql(writer);
        }
    }
}