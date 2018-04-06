// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.Outputs;
    using Caredev.Mego.Resolve.ValueGenerates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Res = Properties.Resources;
    /// <summary>
    /// 用于提交数据的生成数据对象，生成关联<see cref="CommitUnitBase"/>对象。
    /// </summary>
    public abstract class GenerateDataForUnits : GenerateDataForCommit
    {
        /// <summary>
        /// 创建数据对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">当前操作对象。</param>
        internal GenerateDataForUnits(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
        }
        /// <summary>
        /// 主要提交单元。
        /// </summary>
        public CommitUnitBase MainUnit { get; internal protected set; }
        /// <summary>
        /// 其他继承表相关的提交单元。
        /// </summary>
        public CommitKeyUnit[] SubUnits { get; internal protected set; }
        /// <summary>
        /// 当前需要返回数据的成员。
        /// </summary>
        public IEnumerable<CommitMember> ReturnMembers => _ReturnMembers;
        private List<CommitMember> _ReturnMembers = new List<CommitMember>();
        /// <summary>
        /// 枚举当前所有需要提交的单元。
        /// </summary>
        /// <returns>单元枚举器。</returns>
        public IEnumerable<CommitUnitBase> GetUnits()
        {
            yield return MainUnit;
            if (SubUnits != null)
            {
                foreach (var unit in SubUnits)
                    yield return unit;
            }
        }
        /// <summary>
        /// 获取指定列的值生成对象。
        /// </summary>
        /// <param name="column">指定数据列元数据。</param>
        /// <returns>值生成对象。</returns>
        protected abstract ValueGenerateBase GetValueGenerator(ColumnMetadata column);
        /// <summary>
        /// 注册需要返回的成员。
        /// </summary>
        /// <param name="member">指定提交成员。</param>
        /// <returns>返回传入成员。</returns>
        protected CommitMember ReisterReturnMember(CommitMember member)
        {
            if (Operate.HasResult)
            {
                _ReturnMembers.Add(member);
            }
            return member;
        }
        /// <summary>
        /// 创建指定数据列提交单元成员。
        /// </summary>
        /// <param name="column">指定数据列。</param>
        /// <param name="generator">值生成对象。</param>
        /// <param name="customExpression">自定义表达式。</param>
        /// <returns>创建结果。</returns>
        protected CommitMember CreateCommitUnitMember(ColumnMetadata column, ValueGenerateBase generator, DbExpression customExpression = null)
        {
            var context = this.GenerateContext;
            if (generator == null)
            {
                if (customExpression != null)
                    return ReisterReturnMember(new CommitExpressionMember(column, generator, customExpression));
                else
                    return new CommitMember(column);
            }
            else
            {
                switch (generator.GeneratedOption)
                {
                    case EGeneratedOption.Expression:
                        var expression = generator as ValueGenerateExpression;
                        return ReisterReturnMember(new CommitExpressionMember(column, generator, context.Translate(expression.Expression)));
                    case EGeneratedOption.Database:
                    case EGeneratedOption.Identity:
                        return ReisterReturnMember(new CommitMember(column, generator, ECommitValueType.Database));
                    case EGeneratedOption.Ignore:
                    case EGeneratedOption.Memory:
                        if (customExpression != null)
                            return ReisterReturnMember(new CommitExpressionMember(column, generator, customExpression));
                        else
                            return new CommitMember(column);
                    default:
                        throw new NotSupportedException(string.Format(Res.NotSupportedGeneratedOption, generator.GeneratedOption));
                }
            }
        }
        /// <summary>
        /// 根据属性映射表达式创建提交单元。
        /// </summary>
        /// <param name="commitUnit">当前提交单元。</param>
        /// <param name="columns">指定的列元数据集合。</param>
        /// <param name="content">属性映射表达式。</param>
        /// <returns>当前提交单元。</returns>
        protected CommitUnitBase CreateCommitUnit(CommitUnitBase commitUnit, IEnumerable<ColumnMetadata> columns, DbExpression content)
        {
            if (content is DbSelectExpression select && select.Item is DbNewExpression newExpression)
            {
                foreach (var column in columns)
                {
                    var generator = GetValueGenerator(column);
                    if (newExpression.Members.TryGetValue(column.Member, out DbExpression expression))
                    {
                        if (expression is DbMemberExpression memberAccess && memberAccess.Member == column.Member
                               && memberAccess.Expression == select.Source.Item)
                        {
                            commitUnit.Add(CreateCommitUnitMember(column, generator));
                        }
                        else
                        {
                            commitUnit.Add(CreateCommitUnitMember(column, generator, expression));
                        }
                    }
                    else if (generator != null && generator.GeneratedOption != EGeneratedOption.Ignore)
                    {
                        commitUnit.Add(CreateCommitUnitMember(column, generator));
                    }
                }
            }
            else
            {
                foreach (var column in columns)
                {
                    var generator = GetValueGenerator(column);
                    commitUnit.Add(CreateCommitUnitMember(column, generator));
                }
            }
            return commitUnit;
        }
        /// <summary>
        /// 生成当前操作的输出对象。
        /// </summary>
        public void GenerateOutput()
        {
            if (ReturnMembers.Any())
            {
                var returns = ReturnMembers.Select(a => a.Metadata.Member).ToArray();
                var metadata = GenerateContext.Metadata.Type(Items.ClrType);
                var fields = Utility.Array<int>(metadata.PrimaryMembers.Count, -1);
                var output = new CollectionOutputInfo(metadata, fields);
                for (int i = 0; i < metadata.PrimaryMembers.Count; i++)
                {
                    var member = metadata.PrimaryMembers[i];
                    fields[i] = Array.IndexOf(returns, member.Member);
                }
                Operate.Output = output;
            }
        }
    }
}