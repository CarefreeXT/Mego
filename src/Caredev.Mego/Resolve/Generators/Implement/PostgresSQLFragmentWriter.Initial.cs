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
    partial class PostgresSQLFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<Type, WriteFragmentDelegate> InitialMethodsForWriteFragment()
        {
            var dictionary = base.InitialMethodsForWriteFragment();

            dictionary.AddOrUpdate(typeof(CreateColumnFragment), WriteFragmentForCreateColumn);
            dictionary.AddOrUpdate(typeof(CreateTableFragment), WriteFragmentForCreateTable);

            dictionary.AddOrUpdate(typeof(ObjectExsitFragment), WriteFragmentForObjectExsit);

            return dictionary;
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
                case Operates.EDatabaseObject.Table: writer.Write("TABLES"); break;
                case Operates.EDatabaseObject.View: writer.Write("VIEWS"); break;
            }
            writer.Write(" t WHERE t.TABLE_SCHEMA='");
            writer.Write(schema);
            writer.Write("' AND t.TABLE_NAME='");
            writer.Write(exist.Name.Name);
            writer.Write('\'');
            writer.Write(") THEN CAST(1 AS BOOLEAN) ELSE CAST(0 AS BOOLEAN) END");
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
    }
}
