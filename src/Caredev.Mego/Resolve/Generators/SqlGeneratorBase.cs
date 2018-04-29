// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Contents;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.Outputs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
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
            if (!_GenerateFragmentMethods.TryGetValue(context.Data.GetType(), out GenerateFragmentDelegate method))
            {
                throw new NotImplementedException();
            }
            var fragment = method(context, content);
            return fragment.ToString();
        }
    }
    public partial class SqlGeneratorBase : IDbSqlGenerator
    {
        /// <summary>
        /// 针对选择查询表达式注册数据源。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="content">表达式内容。</param>
        /// <param name="source">目前数据源语句对象。</param>
        protected void RegisterExpressionForCommit(GenerateContext context, DbExpression content, ISourceFragment source)
        {
            if (content != null && content is DbSelectExpression select)
            {
                context.RegisterSource(select.Source, source, true);
                context.RegisterSource(select.Source.Item, source, true);
            }
        }
        /// <summary>
        /// 创建操作内容数据对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">操作对象。</param>
        /// <returns></returns>
        public virtual OperateContentBase CreateData(GenerateContext context, DbOperateBase operate)
        {
            switch (operate.Type)
            {
                case EOperateType.AddRelation:
                case EOperateType.RemoveRelation:
                    return new RelationContent(context, operate as DbRelationOperateBase);
                case EOperateType.QueryCollection:
                case EOperateType.QueryObject:
                    return new QueryContent(context, operate as DbQueryOperateBase);
                case EOperateType.InsertStatement:
                case EOperateType.UpdateStatement:
                case EOperateType.DeleteStatement:
                    return new StatementContent(context, operate as DbStatementOperateBase);
                default:
                    if (operate is DbObjectsOperateBase objects)
                    {
                        var metadata = context.Metadata.Table(objects.ClrType);
                        var isherit = metadata.InheritSets.Length > 0;
                        switch (operate.Type)
                        {
                            case EOperateType.InsertObjects:
                            case EOperateType.InsertPropertys:
                                if (isherit)
                                {
                                    return new InheritInsertContent(context, objects);
                                }
                                else
                                {
                                    return new InsertContent(context, objects);
                                }
                            case EOperateType.UpdateObjects:
                            case EOperateType.UpdatePropertys:
                                if (isherit)
                                {
                                    return new InheritUpdateContent(context, objects);
                                }
                                else
                                {
                                    return new UpdateContent(context, objects);
                                }
                            case EOperateType.DeleteObjects:
                                if (isherit)
                                {
                                    return new InheritDeleteContent(context, objects);
                                }
                                else
                                {
                                    return new DeleteContent(context, objects);
                                }
                            default:
                                throw new System.InvalidOperationException();
                        }
                    }
                    else if (operate is DbMaintenanceOperateBase maintenance)
                    {
                        return new MaintenanceContent(context, maintenance);
                    }
                    else
                    {
                        return new OperateContentBase(context, operate);
                    }
            }
        }

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