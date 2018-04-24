// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Contents;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    internal static class GenerateContextExtensions
    {

        public static BinaryFragment LessThan(this GenerateContext context, IExpressionFragment left, IExpressionFragment right)
        {
            return new BinaryFragment(context, EBinaryKind.LessThan)
            {
                Left = left,
                Right = right
            };
        }

        public static BinaryFragment Add(this GenerateContext context, IExpressionFragment left, IExpressionFragment right)
        {
            return new BinaryFragment(context, EBinaryKind.Add)
            {
                Left = left,
                Right = right
            };
        }

        public static BinaryFragment Assign(this GenerateContext context, IExpressionFragment left, IExpressionFragment right)
        {
            return new BinaryFragment(context, EBinaryKind.Assign)
            {
                Left = left,
                Right = right
            };
        }

        public static BinaryFragment Equal(this GenerateContext context, IExpressionFragment left, IExpressionFragment right)
        {
            return new BinaryFragment(context, EBinaryKind.Equal)
            {
                Left = left,
                Right = right
            };
        }

        public static SimpleFragment StatementString(this GenerateContext context, string content)
        {
            return new SimpleFragment(context, content);
        }

        public static InsertFragment Insert(this GenerateContext context, ISourceFragment source, CommitKeyUnit target)
        {
            return Insert(context, source, target.Table,
                target.Keys.Concat(target.Members).Select(a => a.Metadata));
        }

        public static InsertFragment Insert(this GenerateContext context, ISourceFragment source, TableMetadata target
            , IEnumerable<ColumnMetadata> members)
        {
            DbName name = null;
            if (context.Data is CommitContentBase data)
            {
                name = data.TargetName;
            }
            var table = new TableFragment(context, target, name);
            var select = new SelectFragment(context, source);
            var current = new InsertFragment(context, table, select);
            foreach (var member in members)
            {
                select.CreateMember(null, source.GetMember(member));
                current.CreateMember(null, member);
            }
            return current;
        }

        public static INameFragment ConvertName(this GenerateContext context, DbName name)
        {
            if (name != null)
            {
                switch (name.Kind)
                {
                    case EDbNameKind.Contact:
                        return new SimpleFragment(context, name.Name);
                    case EDbNameKind.Name:
                        return new DbNameFragment(context, name.Name);
                    case EDbNameKind.NameSchema:
                        return new ObjectNameFragment(context, name.Name, name.Schema);
                }
            }
            return null;
        }
    }
}
