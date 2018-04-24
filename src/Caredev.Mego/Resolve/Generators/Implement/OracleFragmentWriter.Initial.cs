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
    }
}
