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
    partial class ExcelFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<Type, WriteFragmentDelegate> InitialMethodsForWriteFragment()
        {
            var dictionary = base.InitialMethodsForWriteFragment();

            dictionary.AddOrUpdate(typeof(CreateColumnFragment), WriteFragmentForCreateColumn);

            dictionary.TryRemove(typeof(CreateViewFragment));
            dictionary.TryRemove(typeof(RenameObjectFragment));
            dictionary.TryRemove(typeof(DropRelationFragment));
            dictionary.TryRemove(typeof(CreateRelationFragment));
            return dictionary;
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
        protected override void WriteFragmentForCreateTable(SqlWriter writer, ISqlFragment fragment)
        {
            var create = (CreateTableFragment)fragment;
            var metadata = create.Metadata;
            writer.Write("CREATE TABLE ");
            create.Name.WriteSql(writer);
            writer.Write('(');
            create.Members.ForEach(() => writer.WriteLine(", "),
                m => m.WriteSql(writer));
            writer.WriteLine(")");
        }
        private void WriteFragmentForCreateColumn(SqlWriter writer, ISqlFragment fragment)
        {
            var create = (CreateColumnFragment)fragment;
            WriteDbName(writer, create.Name);
            writer.Write(' ');
            var column = create.Metadata;
            WriteDbDataType(writer, column);
        }
    }
}
