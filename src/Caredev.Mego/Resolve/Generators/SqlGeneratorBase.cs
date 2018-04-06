// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.Outputs;
    using Caredev.Mego.Resolve.ValueGenerates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Caredev.Mego.Exceptions;
    using Res = Properties.Resources;
    /// <summary>
    /// 语句生成器基类。
    /// </summary>
    public abstract partial class SqlGeneratorBase : IDbSqlGenerator
    {
        /// <summary>
        /// 当前生成器对应的数据库版本号。
        /// </summary>
        public abstract short Version { get; }
        /// <summary>
        /// 当前生成器对应的提供程序名称。
        /// </summary>
        public abstract string ProviderName { get; }
        /// <summary>
        /// 当前数据库的全局特性。
        /// </summary>
        public abstract DbFeature Feature { get; }
        /// <summary>
        /// 语句片段写入器。
        /// </summary>
        public abstract FragmentWriterBase FragmentWriter { get; }
        /// <summary>
        /// 根据指定操作及表达式生成语句。
        /// </summary>
        /// <param name="operate">操作对象。</param>
        /// <param name="content">表达式。</param>
        /// <returns>生成的语句。</returns>
        public string Generate(DbOperateBase operate, DbExpression content)
        {
            var context = new GenerateContext(operate, this);
            context.Data.Inititalze(content);
            SqlFragment fragment = null;
            switch (operate.Type)
            {
                case EOperateType.InsertObjects: fragment = GenerateForInsert(context); break;
                case EOperateType.UpdateObjects: fragment = GenerateForUpdate(context); break;
                case EOperateType.DeleteObjects: fragment = GenerateForDelete(context); break;
                case EOperateType.QueryObject:
                case EOperateType.QueryCollection: fragment = GenerateForQuery(context, content); break;
                case EOperateType.InsertPropertys: fragment = GenerateForInsert(context, content); break;
                case EOperateType.UpdatePropertys: fragment = GenerateForUpdate(context, content); break;
                case EOperateType.InsertStatement: fragment = GenerateForInsertStatement(context, content); break;
                case EOperateType.UpdateStatement: fragment = GenerateForUpdateStatement(context, content); break;
                case EOperateType.DeleteStatement: fragment = GenerateForDeleteStatement(context, content); break;
                case EOperateType.AddRelation:
                case EOperateType.RemoveRelation: fragment = GenerateForRelation(context); break;
                default:
                    if (operate is DbMaintenanceOperateBase maintenance)
                    {
                        fragment = GenerateForMaintenance(context);
                    }
                    break;
            }
            return fragment.ToString();
        }
    }
    public partial class SqlGeneratorBase : IDbSqlGenerator
    {

        private SourceFragment GenerateForQuery(GenerateContext context, DbExpression content)
        {
            SourceFragment select = null;
            var output = context.GetOutputRoot();
            var root = output as ComplexOutputInfo;

            switch (content)
            {
                case DbSourceFunctionExpression function:
                    {
                        var unittype = function.Source as DbUnitTypeExpression;
                        select = GenerateForQueryRoot(context, unittype, content, root);
                    }
                    break;
                default:
                    {
                        var unittype = content as DbUnitTypeExpression;
                        select = GenerateForQueryRoot(context, unittype, unittype.Item, root);
                    }
                    break;
            }
            context.VerfityOutput(root);
            return select;
        }

        private IMemberFragment ValidateMember(GenerateContext context, IMemberFragment member,
            ISourceFragment current, MemberInfo property, bool onlyRetrieal)
        {
            var memberSource = member.Owner;
            if (onlyRetrieal)
            {
                if (current == memberSource)
                {
                    throw new GenerateException(Res.ExceptionMemberSourceMatch);
                }
                if (current == memberSource.Parent) return member;
            }
            else
            {
                if (current == memberSource) return member;
            }

            var parent = memberSource.Parent;
            if (parent == null)
            {
                throw new GenerateException(Res.ExceptionMemberSourceParentIsNulll);
            }

            var result = parent.Members.OfType<ReferenceMemberFragment>().Where(a => a.Reference == member).FirstOrDefault();
            if (result != null) return ValidateMember(context, result, current, property, onlyRetrieal);

            if (!onlyRetrieal && parent == current)
            {
                return parent.CreateMember(property ?? member.Property, member);
            }
            else
            {
                var newMember = parent.CreateMember(member.Property, member);
                return ValidateMember(context, newMember, current, property, onlyRetrieal);
            }
        }

        private SourceFragment GenerateForQueryRoot(GenerateContext context, DbUnitTypeExpression unittype,
            DbExpression initialExpression, ComplexOutputInfo root = null)
        {
            var source = CreateSource(context, unittype);
            var select = source as SelectFragment;
            if (select == null || select.IsRecommandLock)
            {
                var oldSelect = select;
                select = new SelectFragment(context, source);
                InitialMembers(context, select, initialExpression, root);
                if (oldSelect != null && !select.IsRecommandLock && select.Sources.Count() == 1)
                {
                    select = oldSelect;
                }
            }
            else
            {
                InitialMembers(context, select, initialExpression, root);
            }
            return select;
        }

        private IEnumerable<ISqlFragment> GenerateCreateTemplateTable(GenerateContext context,
            TemporaryTableFragment temptable, CommitObjectFragment source, DbObjectsOperateBase operate)
        {
            var createtable = new CreateTempTableFragment(context, temptable);
            var insert = new InsertValueFragment(context, temptable, source, operate);
            foreach (var column in temptable.Members.OfType<ColumnFragment>().Select(a => a.Metadata))
            {
                createtable.Members.Add(new CreateColumnFragment(context, column, createtable));
                insert.SetValue(column);
            }
            yield return createtable;
            yield return insert;
        }

        private IEnumerable<ISqlFragment> GenerateCreateTemplateTable(GenerateContext context,
            CommitObjectFragment source, DbObjectsOperateBase operate, IEnumerable<ColumnMetadata> members)
        {
            var createtable = new CreateTempTableFragment(context, members);
            var temptable = createtable.Table;
            var insert = new InsertValueFragment(context, temptable, source, operate);
            foreach (var column in members)
            {
                insert.SetValue(column);
            }
            yield return createtable;
            yield return insert;
        }
    }
}