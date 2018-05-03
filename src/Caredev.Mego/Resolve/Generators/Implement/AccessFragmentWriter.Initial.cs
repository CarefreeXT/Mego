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
    partial class AccessFragmentWriter
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
            writer.Write("SELECT COUNT(1) FROM MSysObjects Where ");
            switch (exist.Kind)
            {
                case EDatabaseObject.Table: writer.Write("Type=1"); break;
                case EDatabaseObject.View: writer.Write("Type=5"); break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedWriteDatabaseObject, exist.Kind));
            }
            writer.Write(" AND Name='");
            writer.Write(exist.Name.Name);
            writer.Write('\'');
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForDelete(SqlWriter writer, ISqlFragment fragment)
        {
            var current = (DeleteFragment)fragment;
            writer.Write("DELETE ");
            if (current.Sources.Count() > 1)
            {
                writer.Write(current.Target.AliasName);
                writer.Write(".* ");
            }
            WriteFragmentForFrom(writer, current.Sources);
            WriteFragmentForWhere(writer, current.Where);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForFrom(SqlWriter writer, IEnumerable<ISourceFragment> sources)
        {
            if (sources.Any())
            {
                var list = sources.ToArray();
                writer.WriteLine();
                writer.Write("FROM ");
                WriteFragmentForSourceSpecial(writer, list, list.Length - 1);
            }
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForSourceJoin(SqlWriter writer, ISourceFragment source)
        {
            if (source.Join.Value != EJoinType.CrossJoin)
            {
                base.WriteFragmentForSourceJoin(writer, source);
            }
        }
    }
    partial class AccessFragmentWriter
    {
        /// <inheritdoc/>
        protected override IDictionary<MemberInfo, WriteFragmentDelegate> InitialMethodsForWriteScalar()
        {
            var result = base.InitialMethodsForWriteScalar();

            result.AddOrUpdate(SupportMembers.DateTime.Now, (w, e) => w.Write("NOW()"));

            result.AddOrUpdate(SupportMembers.DbFunctions.GetIdentity, (w, e) => w.Write("@@IDENTITY"));

            return result;
        }
    }
}