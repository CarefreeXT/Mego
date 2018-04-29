// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System.Collections.Generic;
    using System.Linq;
    //公共方法写入方法
    public partial class FragmentWriterBase
    {
        /// <summary>
        /// 写入数据源内容。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="source">数据源语句。</param>
        protected virtual void WriteFragmentForSourceContent(SqlWriter writer, ISourceFragment source)
        {
            if (source is ValuesFragment)
            {
                source.WriteSql(writer);
            }
            else
            {
                if (source is SelectFragment || source is SetFragment)
                {
                    writer.Write('(');
                    source.WriteSql(writer);
                    writer.Write(')');
                }
                else
                {
                    source.WriteSql(writer);
                }
                if (!(source is InheritFragment))
                {
                    WriteAliasName(writer, source.AliasName);
                }
            }
        }
        /// <summary>
        /// 写入数据源连接条件。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="source">数据源语句。</param>
        protected virtual void WriteFragmentForSourceCondition(SqlWriter writer, ISourceFragment source)
        {
            if (source.Condition != null)
            {
                writer.Write(" ON ");
                source.Condition.WriteSql(writer);
            }
        }
        /// <summary>
        /// 写入数据源连接子句
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="source">目标写入的数据源。</param>
        protected virtual void WriteFragmentForSourceJoin(SqlWriter writer, ISourceFragment source)
        {
            switch (source.Join.Value)
            {
                case EJoinType.CrossJoin: writer.Write("CROSS JOIN "); break;
                case EJoinType.InnerJoin: writer.Write("INNER JOIN "); break;
                case EJoinType.LeftJoin: writer.Write("LEFT JOIN "); break;
                case EJoinType.RightJoin: writer.Write("RIGHT JOIN "); break;
            }
        }
        /// <summary>
        /// 写入单个数据源对象。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">写入语句。</param>
        protected virtual void WriteFragmentForSource(SqlWriter writer, ISqlFragment fragment)
        {
            var source = (SourceFragment)fragment;
            if (source.Join.HasValue)
            {
                WriteFragmentForSourceJoin(writer, source);
                WriteFragmentForSourceContent(writer, source);
                WriteFragmentForSourceCondition(writer, source);
            }
            else
            {
                WriteFragmentForSourceContent(writer, source);
            }
        }
        /// <summary>
        /// 写入查询数据源子句。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="sources">数据源集合。</param>
        protected virtual void WriteFragmentForFrom(SqlWriter writer, IEnumerable<ISourceFragment> sources)
        {
            if (sources.Any())
            {
                writer.WriteLine();
                writer.Write("FROM ");
                sources.ForEach(() => writer.WriteLine(),
                    source => WriteFragmentForSource(writer, source));
            }
        }
        /// <summary>
        /// 写入排序语句。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="sorts">排序语句集合。</param>
        protected virtual void WriteFragmentForOrderBy(SqlWriter writer, IEnumerable<IExpressionFragment> sorts)
        {
            if (sorts.Any())
            {
                writer.WriteLine();
                writer.Write("ORDER BY ");
                sorts.ForEach(() => writer.Write(", "), sort => sort.WriteSql(writer));
            }
        }
        /// <summary>
        /// 写入过滤条件。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="where">过滤条件。</param>
        protected virtual void WriteFragmentForWhere(SqlWriter writer, ILogicFragment where)
        {
            if (where != null)
            {
                writer.WriteLine();
                writer.Write("WHERE ");
                where.WriteSql(writer);
            }
        }
        /// <summary>
        /// 写入分组语句。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="groups">分组语句集合。</param>
        protected virtual void WriteFragmentForGroupBy(SqlWriter writer, IEnumerable<ISqlFragment> groups)
        {
            if (groups.Any())
            {
                writer.WriteLine();
                writer.Write("GROUP BY ");
                groups.ForEach(() => writer.Write(", "), item => item.WriteSql(writer));
            }
        }
        /// <summary>
        /// 写入SELECT输出成员。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="members">写入的成员列表。</param>
        protected virtual void WriteFragmentForSelectMembers(SqlWriter writer, IEnumerable<IMemberFragment> members)
        {
            members.ForEach(() => writer.Write(", "), column =>
            {
                column.WriteSql(writer);
                if (!string.IsNullOrEmpty(column.AliasName))
                {
                    writer.Write(" AS ");
                    WriteDbName(writer, column.AliasName);
                }
            });
        }
    }
}
