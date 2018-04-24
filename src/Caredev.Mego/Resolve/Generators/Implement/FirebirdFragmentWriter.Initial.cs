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
    partial class FirebirdFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<Type, WriteFragmentDelegate> InitialMethodsForWriteFragment()
        {
            var dictionary = base.InitialMethodsForWriteFragment();

            dictionary.AddOrUpdate(typeof(CreateColumnFragment), WriteFragmentForCreateColumn);
            dictionary.AddOrUpdate(typeof(ObjectExsitFragment), WriteFragmentForObjectExsit);

            dictionary.TryRemove(typeof(RenameObjectFragment));

            return dictionary;
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
                    if (!nullable.Value) writer.Write(" NOT NULL");
                }
                else
                {
                    var type = column.Member.PropertyType;
                    if (type.IsValueType && !type.IsNullable())
                    {
                        writer.Write(" NOT NULL");
                    }
                }
                if (def != null)
                {
                    writer.Write(" DEFAULT (" + def.Content + ")");
                }
            }
        }
        private void WriteFragmentForObjectExsit(SqlWriter writer, ISqlFragment fragment)
        {
            var exist = (ObjectExsitFragment)fragment;
            writer.Write("SELECT COUNT(1) FROM RDB$RELATIONS ");
            switch (exist.Kind)
            {
                case EDatabaseObject.Table: writer.Write("WHERE RDB$VIEW_BLR IS NULL AND (RDB$SYSTEM_FLAG IS NULL OR RDB$SYSTEM_FLAG = 0)"); break;
                case EDatabaseObject.View: writer.Write("WHERE RDB$VIEW_BLR IS NOT NULL AND (RDB$SYSTEM_FLAG IS NULL OR RDB$SYSTEM_FLAG = 0)"); break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, exist.Kind));
            }
            writer.Write(" AND RDB$RELATION_NAME='");
            writer.Write(exist.Name.Name);
            writer.Write('\'');
        }
    }
}
