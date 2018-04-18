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
    partial class PostgresSQLFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<Type, WriteFragmentDelegate> InitialMethodsForWriteFragment()
        {
            var dictionary = base.InitialMethodsForWriteFragment();

            dictionary.AddOrUpdate(typeof(CreateColumnFragment), WriteFragmentForCreateColumn);
            dictionary.AddOrUpdate(typeof(CreateTableFragment), WriteFragmentForCreateTable);

            return dictionary;
        }
        private void WriteFragmentForCreateTable(SqlWriter writer, ISqlFragment fragment)
        {
            var create = (CreateTableFragment)fragment;
            var metadata = create.Metadata;

            writer.Write("CREATE TABLE ");
            create.Table.WriteSql(writer);
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
