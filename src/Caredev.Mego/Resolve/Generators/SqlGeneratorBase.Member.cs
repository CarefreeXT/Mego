// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Outputs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 根据分组检索表达式初始化查询输出语句及输出对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级输出对象。</param>
        public void InitialRetrieval(GenerateContext context, ISourceFragment current, DbRetrievalFunctionExpression expression, ComplexOutputInfo parent)
        {
            if (!_InitialRetrievalMethods.TryGetValue(expression.Function, out InitialRetrievalDelegate method))
            {
                throw new NotSupportedException(string.Format(Res.NotSupportedInitialRetrieval, expression.Function));
            }
            method(context, current, expression, parent);
        }
        /// <summary>
        /// 根据分组检索表达式 First 初始化查询输出语句及输出对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级输出对象。</param>
        protected virtual void InitialRetrievalForFirst(GenerateContext context, ISourceFragment current, DbRetrievalFunctionExpression expression, ComplexOutputInfo parent)
        {
            var select = current as SelectFragment;
            select.Take = 1;
            InitialRetrievalForFilter(context, expression, select);
            if (parent is ObjectOutputInfo output) output.Option = EObjectOutputOption.One;
        }
        /// <summary>
        /// 根据分组检索表达式 FirstDefault 初始化查询输出语句及输出对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级输出对象。</param>
        protected virtual void InitialRetrievalForFirstDefault(GenerateContext context, ISourceFragment current, DbRetrievalFunctionExpression expression, ComplexOutputInfo parent)
        {
            var select = current as SelectFragment;
            select.Take = 1;
            InitialRetrievalForFilter(context, expression, select);
            if (parent is ObjectOutputInfo output) output.Option = EObjectOutputOption.ZeroOrOne;
        }
        /// <summary>
        /// 根据分组检索表达式 Single 初始化查询输出语句及输出对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级输出对象。</param>
        protected virtual void InitialRetrievalForSingle(GenerateContext context, ISourceFragment current, DbRetrievalFunctionExpression expression, ComplexOutputInfo parent)
        {
            var select = current as SelectFragment;
            select.Take = 2;
            InitialRetrievalForFilter(context, expression, select);
            if (parent is ObjectOutputInfo output) output.Option = EObjectOutputOption.OnlyOne;
        }
        /// <summary>
        /// 根据分组检索表达式 SingleDefault 初始化查询输出语句及输出对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级输出对象。</param>
        protected virtual void InitialRetrievalForSingleDefault(GenerateContext context, ISourceFragment current, DbRetrievalFunctionExpression expression, ComplexOutputInfo parent)
        {
            var select = current as SelectFragment;
            select.Take = 2;
            InitialRetrievalForFilter(context, expression, select);
            if (parent is ObjectOutputInfo output) output.Option = EObjectOutputOption.ZeroOrOnlyOne;
        }
        /// <summary>
        /// 根据分组检索表达式 ElementAt 初始化查询输出语句及输出对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级输出对象。</param>
        protected virtual void InitialRetrievalForElementAt(GenerateContext context, ISourceFragment current, DbRetrievalFunctionExpression expression, ComplexOutputInfo parent)
        {
            InitialRetrievalForElementAtImp(context, current, expression, parent, EObjectOutputOption.One);
        }
        /// <summary>
        /// 根据分组检索表达式 ElementAtDefault 初始化查询输出语句及输出对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级输出对象。</param>
        protected virtual void InitialRetrievalForElementAtDefault(GenerateContext context, ISourceFragment current, DbRetrievalFunctionExpression expression, ComplexOutputInfo parent)
        {
            InitialRetrievalForElementAtImp(context, current, expression, parent, EObjectOutputOption.ZeroOrOne);
        }
        //分组检索表达式过滤条件处理。
        private void InitialRetrievalForFilter(GenerateContext context, DbRetrievalFunctionExpression expression, SelectFragment select)
        {
            if (expression.Arguments.Length > 0)
            {
                var filter = CreateExpression(context, expression.Arguments[0], select) as ILogicFragment;
                if (select.Where == null)
                {
                    select.Where = filter;
                }
                else
                {
                    select.Where = select.Where.Merge(filter);
                }
            }
        }
        //分组检索表达式 ElementAt 实现。
        private void InitialRetrievalForElementAtImp(GenerateContext context, ISourceFragment current,
            DbRetrievalFunctionExpression expression, ComplexOutputInfo parent, EObjectOutputOption option)
        {
            var value = expression.Arguments[0] as DbConstantExpression;
            var select = current as SelectFragment;
            select.Take = 1;
            select.Skip = (int)value.Value;
            if (select.Skip > 0 && select.Sorts.Count == 0)
            {
                var member = GetFirstMemberForOrder(select);
                select.Sorts.Add(new SortFragment(context, member));
            }
            if (parent is ObjectOutputInfo output) output.Option = option;
        }
    }
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 根据表达式初始化查询成员集合。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">初始化数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级数据源。</param>
        public void InitialMembers(GenerateContext context, ISourceFragment current, DbExpression expression, ComplexOutputInfo parent)
        {
            if (!_InitialMembersMethods.TryGetValue(expression.ExpressionType, out InitialMembersDelegate method))
            {
                throw new NotSupportedException(string.Format(Res.NotSupportedInitialMember, expression.ExpressionType));
            }
            method(context, current, expression, parent);
        }
        /// <summary>
        /// 根据类型<see cref="DbAggregateFunctionExpression"/>表达式初始化查询成员集合。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">初始化数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级数据源。</param>
        protected virtual void InitialMembersForAggregateFunction(GenerateContext context, ISourceFragment current, DbExpression expression, ComplexOutputInfo parent)
        {
            var aggregate = (DbAggregateFunctionExpression)expression;
            context.RegisterSource((DbExpression)aggregate.Source, current);
            RetrievalMember(context, current, aggregate, null, false);
        }
        /// <summary>
        /// 根据类型<see cref="DbRetrievalFunctionExpression"/>表达式初始化查询成员集合。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">初始化数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级数据源。</param>
        protected virtual void InitialMembersForRetrievalFunction(GenerateContext context, ISourceFragment current, DbExpression expression, ComplexOutputInfo parent)
        {
            var aggregate = (DbRetrievalFunctionExpression)expression;
            var unittype = aggregate.Source as DbUnitTypeExpression;
            if (parent != null)
            {
                InitialMembers(context, current, unittype.Item, parent);
            }
            else
            {
                RetrievalMember(context, current, unittype.Item, null, false);
            }
            InitialRetrieval(context, current, aggregate, parent);
        }
        /// <summary>
        /// 根据类型<see cref="DbUnitValueContentExpression"/>表达式初始化查询成员集合。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">初始化数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级数据源。</param>
        protected virtual void InitialMembersForUnitValueContent(GenerateContext context, ISourceFragment current, DbExpression expression, ComplexOutputInfo parent)
        {
            var value = (DbUnitValueContentExpression)expression;
            current.RetrievalMembers(value.Content, false).ToArray();
        }
        /// <summary>
        /// 根据类型<see cref="DbUnitObjectContentExpression"/>表达式初始化查询成员集合。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">初始化数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级数据源。</param>
        protected virtual void InitialMembersForUnitObjectContent(GenerateContext context, ISourceFragment current, DbExpression expression, ComplexOutputInfo parent)
        {
            var value = (DbUnitObjectContentExpression)expression;
            InitialMembersForObjectMember(context, current, value.Content, parent);
        }
        /// <summary>
        /// 根据类型<see cref="DbObjectMemberExpression"/>表达式初始化查询成员集合。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">初始化数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级数据源。</param>
        protected virtual void InitialMembersForObjectMember(GenerateContext context, ISourceFragment current, DbExpression expression, ComplexOutputInfo parent)
        {
            var value = (DbObjectMemberExpression)expression;
            var primarys = parent.Metadata.PrimaryMembers;
            var fields = parent.ItemFields;
            foreach (var member in current.RetrievalMembers(value, false))
            {
                fields[primarys.IndexOf((PropertyInfo)member.Property)] = current.Members.IndexOf(member);
            }
            if (parent.ItemKeyFields == null)
                parent.ItemKeyFields = parent.ItemFields;
        }
        /// <summary>
        /// 根据类型表达式初始化查询成员集合（通用方法）。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">初始化数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级数据源。</param>
        protected virtual void InitialMembersForCommon(GenerateContext context, ISourceFragment current, DbExpression expression, ComplexOutputInfo parent)
        {
            var members = current.RetrievalMembers(expression, false).ToArray();
            if (parent != null)
            {
                var primarys = parent.Metadata.PrimaryMembers;
                var fields = parent.ItemFields;
                foreach (var member in members)
                    fields[primarys.IndexOf((PropertyInfo)member.Property)] = current.Members.IndexOf(member);
                var expandUnit = GetExpandUnit(expression);
                if (expandUnit != null)
                {
                    foreach (var item in expandUnit.ExpandItems.OfType<IDbMemberExpression>())
                    {
                        InitialExpandItemMember(context, (PropertyInfo)item.Member, current, parent, (DbExpression)item);
                    }
                }
            }
        }
        /// <summary>
        /// 根据类型<see cref="DbNewExpression"/>表达式初始化查询成员集合。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">初始化数据源。</param>
        /// <param name="expression">初始化表达式。</param>
        /// <param name="parent">父级数据源。</param>
        protected virtual void InitialMembersForNew(GenerateContext context, ISourceFragment current, DbExpression expression, ComplexOutputInfo parent)
        {
            if (parent != null)
            {
                var primarys = parent.Metadata.PrimaryMembers;
                var complexs = parent.Metadata.ComplexMembers;
                var fields = parent.ItemFields;
                var content = (DbNewExpression)expression;
                var keyFields = new List<int>();
                foreach (var item in content.Members)
                {
                    if (_CreateExpressionMethods.TryGetValue(item.Value.ExpressionType, out CreateExpressionDelegate method))
                    {
                        var member = RetrievalMemberForExpression(context, current, item.Value, item.Key, false);
                        fields[primarys.IndexOf((PropertyInfo)member.Property)] = current.Members.IndexOf(member);
                    }
                    else if (_RetrievalMemberMethods.TryGetValue(item.Value.ExpressionType, out RetrievalMemberDelegate method1))
                    {
                        var member = method1(context, current, item.Value, item.Key, false);
                        var index = current.Members.IndexOf(member);
                        fields[primarys.IndexOf((PropertyInfo)member.Property)] = index;
                        keyFields.Add(index);
                    }
                    else if (_RetrievalMembersMethods.TryGetValue(item.Value.ExpressionType, out RetrievalMembersDelegate method2))
                    {
                        var property = (PropertyInfo)item.Key;
                        var source = InitialExpandItemMember(context, property, current, parent, item.Value);
                        if (property.PropertyType.IsObject())
                        {
                            keyFields.AddRange(source.ItemKeyFields);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format(Res.NotSupportedInitialNewValue, item.Value.ExpressionType));
                    }
                }
                parent.ItemKeyFields = keyFields.Distinct().ToArray();
            }
            else
            {
                throw new GenerateException(Res.ExceptionInitialNewParentIsNull);
            }
        }
        //初始化展开成员项
        private ComplexOutputInfo InitialExpandItemMember(GenerateContext context, PropertyInfo property,
            ISourceFragment current, ComplexOutputInfo parent, DbExpression expression)
        {
            if (expression is IDbMemberExpression member)
            {
                if (member.Expression is IDbMemberExpression parentMmeber)
                {
                    parent = InitialExpandItemMember(context, (PropertyInfo)parentMmeber.Member, current, parent, member.Expression);
                }
            }
            var complexs = parent.Metadata.ComplexMembers;
            var source = context.CreateOutput(property.PropertyType);
            source.Index = complexs.IndexOf(property);
            parent.AddChildren(source);
            InitialMembers(context, current, expression, source);
            return source;
        }
        //获取展开成员单元
        private IDbExpandUnitExpression GetExpandUnit(DbExpression expression)
        {
            switch (expression)
            {
                case DbDataItemExpression dataitem:
                    return dataitem.Unit as IDbExpandUnitExpression;
                case DbUnitItemContentExpression unititemcontent:
                    return GetExpandUnit(unititemcontent.Content);
            }
            return expression as IDbExpandUnitExpression;
        }
    }
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 根据表达式检索单个成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="property">检索成员的成员CLR属性。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员。</returns>
        public IMemberFragment RetrievalMember(GenerateContext context, ISourceFragment current,
            DbExpression expression, MemberInfo property, bool onlyRetrieal = true)
        {
            if (_RetrievalMemberMethods.TryGetValue(expression.ExpressionType, out RetrievalMemberDelegate method))
                return method(context, current, expression, property, onlyRetrieal);
            throw new NotSupportedException(string.Format(Res.NotSupportedRetrievalMember, expression.ExpressionType));
        }
        /// <summary>
        /// 根据类型<see cref="DbMemberExpression"/>表达式检索单个成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="property">检索成员的成员CLR属性。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员。</returns>
        protected virtual IMemberFragment RetrievalMemberForMemberAccess(GenerateContext context, ISourceFragment current,
            DbExpression expression, MemberInfo property, bool onlyRetrieal)
        {
            var content = (DbMemberExpression)expression;
            var source = GetSource(context, content.Expression);
            if (source is VirtualSourceFragment virtualSource &&
                virtualSource.Expression.ExpressionType == EExpressionType.GroupBy)
            {
                source = virtualSource.GetBody();
                return ValidateMember(context, source.Members.First(), current, property, onlyRetrieal);
            }
            var result = source.GetMember(content.Member);
            return ValidateMember(context, result, current, property, onlyRetrieal);
        }
        /// <summary>
        /// 根据类型<see cref="DbAggregateFunctionExpression"/>表达式检索单个成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="property">检索成员的成员CLR属性。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员。</returns>
        protected virtual IMemberFragment RetrievalMemberForAggregateFunction(GenerateContext context, ISourceFragment current,
            DbExpression expression, MemberInfo property, bool onlyRetrieal)
        {
            var content = (DbAggregateFunctionExpression)expression;
            var tempsource = GetSource(context, (DbExpression)content.Source);
            IMemberFragment result = null;
            if (tempsource is VirtualSourceFragment virtualSourcwe)
            {
                var source = virtualSourcwe.GetBody();
                result = source.Members.OfType<AggregateFragment>().Where(a => a.Expression == content).FirstOrDefault();
                if (result == null)
                    result = source.CreateMember(property, content);
            }
            else
            {
                result = current.CreateMember(property, content);
            }
            return ValidateMember(context, result, current, property, onlyRetrieal);
        }
        /// <summary>
        /// 根据类型<see cref="DbUnitValueContentExpression"/>表达式检索单个成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="property">检索成员的成员CLR属性。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员。</returns>
        protected virtual IMemberFragment RetrievalMemberForUnitValueContent(GenerateContext context, ISourceFragment current,
            DbExpression expression, MemberInfo property, bool onlyRetrieal)
        {
            var content = (DbUnitValueContentExpression)expression;
            return RetrievalMember(context, current, content.Content, property, onlyRetrieal);
        }
        /// <summary>
        /// 根据表达式检索单个表达式成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="property">检索成员的成员CLR属性。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员。</returns>
        protected virtual IMemberFragment RetrievalMemberForExpression(GenerateContext context, ISourceFragment current,
            DbExpression expression, MemberInfo property, bool onlyRetrieal)
        {
            var result = current.Members.Where(a => a.Property == property).FirstOrDefault();
            if (result == null)
            {
                if (_CreateExpressionMethods.TryGetValue(expression.ExpressionType, out CreateExpressionDelegate method))
                {
                    if (onlyRetrieal)
                    {
                        throw new NotSupportedException(Res.NotSupportedOnlyRetrievalForExpression);
                    }
                    else
                    {
                        var expressionMember = method(context, expression, current);
                        return current.CreateMember(property, expressionMember);
                    }
                }
            }
            return result;
        }
    }
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 根据表达式检索多个成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员枚举。</returns>
        public IEnumerable<IMemberFragment> RetrievalMembers(GenerateContext context, ISourceFragment current,
            DbExpression expression, bool onlyRetrieal = true)
        {
            if (_RetrievalMemberMethods.TryGetValue(expression.ExpressionType, out RetrievalMemberDelegate method))
                return new IMemberFragment[] { method(context, current, expression, null, onlyRetrieal) };
            if (_RetrievalMembersMethods.TryGetValue(expression.ExpressionType, out RetrievalMembersDelegate method1))
                return method1(context, current, expression, onlyRetrieal).ToArray();
            throw new NotSupportedException(string.Format(Res.NotSupportedRetrievalMember, expression.ExpressionType));
        }
        /// <summary>
        /// 根据类型<see cref="DbUnitItemContentExpression"/>表达式检索多个成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员枚举。</returns>
        protected virtual IEnumerable<IMemberFragment> RetrievalMembersForUnitItemContent(GenerateContext context,
            ISourceFragment current, DbExpression expression, bool onlyRetrieal)
        {
            var content = (DbUnitItemContentExpression)expression;
            return current.RetrievalMembers(content.Content, onlyRetrieal);
        }
        /// <summary>
        /// 根据类型<see cref="DbDataItemExpression"/>表达式检索多个成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员枚举。</returns>
        protected virtual IEnumerable<IMemberFragment> RetrievalMembersForDataItem(GenerateContext context,
            ISourceFragment current, DbExpression expression, bool onlyRetrieal)
        {
            var content = (DbDataItemExpression)expression;
            var source = GetSource(context, expression);
            foreach (var member in source.Members)
            {
                yield return ValidateMember(context, member, current, null, onlyRetrieal);
            }
        }
        /// <summary>
        /// 根据类型<see cref="DbObjectMemberExpression"/>表达式检索多个成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员枚举。</returns>
        protected virtual IEnumerable<IMemberFragment> RetrievalMembersForObjectMember(GenerateContext context,
            ISourceFragment current, DbExpression expression, bool onlyRetrieal)
        {
            var source = GetSource(context, expression);
            foreach (var member in source.Members)
            {
                yield return ValidateMember(context, member, current, null, onlyRetrieal);
            }
        }
        /// <summary>
        /// 根据类型<see cref="DbNewExpression"/>表达式检索多个成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员枚举。</returns>
        protected virtual IEnumerable<IMemberFragment> RetrievalMembersForNew(GenerateContext context,
            ISourceFragment current, DbExpression expression, bool onlyRetrieal)
        {
            var content = (DbNewExpression)expression;
            foreach (var item in content.Members)
            {
                if (_CreateExpressionMethods.TryGetValue(item.Value.ExpressionType, out CreateExpressionDelegate method))
                {
                    yield return RetrievalMemberForExpression(context, current, item.Value, item.Key, onlyRetrieal);
                }
                else if (_RetrievalMemberMethods.TryGetValue(item.Value.ExpressionType, out RetrievalMemberDelegate method1))
                {
                    yield return method1(context, current, item.Value, item.Key, onlyRetrieal);
                }
                else if (_RetrievalMembersMethods.TryGetValue(item.Value.ExpressionType, out RetrievalMembersDelegate method2))
                {
                    foreach (var member in method2(context, current, item.Value, onlyRetrieal))
                        yield return member;
                }
                else
                {
                    throw new NotSupportedException(string.Format(Res.NotSupportedRetrievalNewValue, item.Value.ExpressionType));
                }
            }
        }
        /// <summary>
        /// 根据类型<see cref="VirtualSourceFragment"/>表达式检索多个成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="current">所在数据源。</param>
        /// <param name="expression">检索表达式。</param>
        /// <param name="onlyRetrieal">仅检索，如果为 false 则表示自动创建不存在的成员。</param>
        /// <returns>检索成员枚举。</returns>
        protected virtual IEnumerable<IMemberFragment> RetrievalMembersForVirtualList(GenerateContext context,
            ISourceFragment current, DbExpression expression, bool onlyRetrieal)
        {
            if (onlyRetrieal)
            {
                if (expression.ExpressionType == EExpressionType.GroupItem)
                {
                    var source = GetSource(context, expression);
                    if (source is VirtualSourceFragment virtualSource)
                    {
                        var body = virtualSource.GetBody();
                        foreach (var member in body.Members)
                            yield return ValidateMember(context, member, current, null, onlyRetrieal);
                    }
                }
                else
                {
                    throw new NotSupportedException(string.Format(Res.NotSupportedRetrievalVirtualList, expression.ExpressionType));
                }
            }
            else
            {
                var source = GetSource(context, expression);
                if (source is VirtualSourceFragment virtualSource)
                {
                    var list = virtualSource.GetList();
                    if (virtualSource.Expression.ExpressionType == EExpressionType.CollectionMember)
                    {
                        var collectionEzpression = (DbCollectionMemberExpression)virtualSource.Expression;
                        var currentSource = GetSource(context, collectionEzpression.TargetSet.Item);
                        context.RegisterSource(collectionEzpression.Item, currentSource);
                        foreach (var member in currentSource.Members)
                            yield return ValidateMember(context, member, current, null, onlyRetrieal);
                    }
                    else
                    {
                        foreach (var member in list.Members)
                            yield return ValidateMember(context, member, current, null, onlyRetrieal);
                    }
                }
                else
                {
                    foreach (var member in source.Members)
                    {
                        yield return ValidateMember(context, member, current, null, onlyRetrieal);
                    }
                }
            }
        }
    }
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 创建成员语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">所在的数据源。</param>
        /// <param name="property">成员CLR属性。</param>
        /// <param name="parameter">创建参数</param>
        /// <returns>成员语句。</returns>
        public IMemberFragment CreateMember(GenerateContext context, ISourceFragment source, MemberInfo property, object parameter)
        {
            var type = parameter.GetType();
            if (_CreateMemberMethods.TryGetValue(type, out CreateMemberDelegate method))
                return method(context, source, property, parameter);
            foreach (var kv in _CreateMemberMethods)
            {
                if (kv.Key.IsAssignableFrom(type))
                {
                    return kv.Value(context, source, property, parameter);
                }
            }
            if (typeof(IExpressionFragment).IsAssignableFrom(type))
            {
                return CreateMemberForExpression(context, source, property, parameter);
            }
            throw new NotSupportedException(string.Format(Res.NotSupportedCreateMember, type));
        }
        /// <summary>
        /// 创建类型<see cref="ColumnFragment"/>成员语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">所在的数据源。</param>
        /// <param name="property">成员CLR属性。</param>
        /// <param name="parameter">创建参数</param>
        /// <returns>成员语句。</returns>
        protected virtual IMemberFragment CreateMemberForColumn(GenerateContext context, ISourceFragment source, MemberInfo property, object parameter)
        {
            return new ColumnFragment(context, source, (ColumnMetadata)parameter);
        }
        /// <summary>
        /// 创建类型<see cref="ReferenceMemberFragment"/>成员语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">所在的数据源。</param>
        /// <param name="property">成员CLR属性。</param>
        /// <param name="parameter">创建参数</param>
        /// <returns>成员语句。</returns>
        protected virtual IMemberFragment CreateMemberForReference(GenerateContext context, ISourceFragment source, MemberInfo property, object parameter)
        {
            var result = new ReferenceMemberFragment(context, source, property, (MemberFragment)parameter);
            result.MakeUniqueName();
            return result;
        }
        /// <summary>
        /// 创建类型<see cref="ExpressionMemberFragment"/>成员语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">所在的数据源。</param>
        /// <param name="property">成员CLR属性。</param>
        /// <param name="parameter">创建参数</param>
        /// <returns>成员语句。</returns>
        protected virtual IMemberFragment CreateMemberForExpression(GenerateContext context, ISourceFragment source, MemberInfo property, object parameter)
        {
            var result = new ExpressionMemberFragment(context, source, property, (IExpressionFragment)parameter);
            result.MakeUniqueName();
            return result;
        }
        /// <summary>
        /// 创建类型<see cref="AggregateFragment"/>成员语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">所在的数据源。</param>
        /// <param name="property">成员CLR属性。</param>
        /// <param name="parameter">创建参数</param>
        /// <returns>成员语句。</returns>
        protected virtual IMemberFragment CreateMemberForAggregate(GenerateContext context, ISourceFragment source, MemberInfo property, object parameter)
        {
            var content = (DbAggregateFunctionExpression)parameter;
            var member = new AggregateFragment(context, source, property, content)
            {
                AliasName = content.Function.Name
            };
            if (content.Arguments.Length > 0)
            {
                var item = content.Source.Item;
                var tempsource = context.GetSourceFragment((DbExpression)content.Source);
                if (tempsource is VirtualSourceFragment virtualSource)
                {
                    context.RegisterTempSource(item, virtualSource.GetBody().Sources.First(), delegate ()
                    {
                        foreach (var argu in content.Arguments)
                            member.Arguments.Add(source.CreateExpression(argu));
                    });
                }
                else
                {
                    foreach (var argu in content.Arguments)
                        member.Arguments.Add(source.CreateExpression(argu));
                }
                var firstArgu = member.Arguments.OfType<MemberFragment>().FirstOrDefault();
                if (firstArgu != null)
                {
                    member.AliasName += firstArgu.OutputName;
                }
            }
            member.MakeUniqueName();
            return member;
        }
    }
}