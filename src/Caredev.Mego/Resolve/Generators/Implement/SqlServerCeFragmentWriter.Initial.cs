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
    }
}
