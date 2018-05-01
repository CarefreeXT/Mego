// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.Providers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    public abstract partial class MSLocalDbFragmentWriter
    {
        /// <inheritdoc/>
        protected override void WriteFragmentForInsertValue(SqlWriter writer, ISqlFragment fragment)
        {
            var insert = (InsertValueFragment)fragment;
            writer.Write("INSERT INTO ");
            insert.Target.WriteSql(writer);
            writer.WriteLine();
            writer.Write('(');
            insert.Members.ForEach(
                () => writer.Write(","),
                column => this.WriteDbName(writer, column.OutputName));
            writer.WriteLine(")");
            writer.Write("VALUES");
            writer.Write('(');
            insert.Values.ForEach(() => writer.Write(","), val => val.WriteSql(writer));
            writer.Write(')');
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
            writer.Enter(delegate ()
            {
                WriteFragmentForWhere(writer, update.Where);
            }, update);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForDelete(SqlWriter writer, ISqlFragment fragment)
        {
            var current = (DeleteFragment)fragment;
            writer.Write("DELETE ");
            current.Target.WriteSql(writer);
            writer.WriteLine();
            writer.Enter(delegate ()
            {
                WriteFragmentForWhere(writer, current.Where);
            }, current);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForColumnMember(SqlWriter writer, ISqlFragment fragment)
        {
            if (writer.Current is UpdateFragment || writer.Current is DeleteFragment)
            {
                var content = (ColumnFragment)fragment;
                WriteDbName(writer, content.ColumnName);
            }
            else
            {
                base.WriteFragmentForColumnMember(writer, fragment);
            }
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForSelect(SqlWriter writer, ISqlFragment fragment)
        {
            var select = (SelectFragment)fragment;
            writer.Enter(delegate ()
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
                WriteFragmentForFrom(writer, select.Sources);
                WriteFragmentForWhere(writer, select.Where);
                WriteFragmentForGroupBy(writer, select.GroupBys);
                WriteFragmentForOrderBy(writer, select.Sorts);
            }, select);
        }
        /// <inheritdoc/>
        protected override void WriteFragmentForCommitMember(SqlWriter writer, ISqlFragment fragment)
        {
            var content = (CommitMemberFragment)fragment;
            var command = fragment.Context.Data.OperateCommand.GetCustomCommand<IMicrosoftCustomCommand>();
            var name = command.AddParameter(content.Metadata, content.Loader, content.Index);
            WriteParameterName(writer, name);
        }
    }
}