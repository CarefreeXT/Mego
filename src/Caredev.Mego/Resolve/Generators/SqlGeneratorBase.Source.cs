// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Linq;
    using Res = Properties.Resources;
    partial class SqlGeneratorBase
    {
        /// <summary>
        /// 根据指定表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        public ISourceFragment CreateSource(GenerateContext context, DbExpression expression)
        {
            if (_CreateSourceMethods.TryGetValue(expression.ExpressionType, out CreateSourceDelegate method))
                return method(context, expression);
            throw new NotSupportedException(string.Format(Res.NotSupportedCreateSource, expression.ExpressionType));
        }
        /// <summary>
        /// 根据指定类型<see cref="DbSetConnectExpression"/>表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual ISourceFragment CreateSourceForSetConnect(GenerateContext context, DbExpression expression)
        {
            var content = (DbSetConnectExpression)expression;
            var source = CreateSource(context, content.Source);
            var target = CreateSource(context, content.Target);
            if (source is SetFragment select)
            {
                select.AddSource(target, content.Kind);
            }
            else
            {
                select = new SetFragment(context, source);
                select.AddSource(target, content.Kind);
            }
            context.RegisterSource(content, select);
            if (!select.Members.Any())
            {
                var unittype = content.Source as DbUnitTypeExpression;
                var members = RetrievalMembers(context, source, unittype.Item, false);
                select.Members.AddRange(members.Select(a => CreateMember(context, select, a.Property, a)));
            }
            if (!target.Members.Any())
            {
                var unittype = content.Target as DbUnitTypeExpression;
                RetrievalMembers(context, target, unittype.Item, false).ToArray();
            }
            context.RegisterSource(content.Item, select);
            if (IsSelectFragment(content))
                return InitialSelectFragment(new SelectFragment(context, select), content);
            return select;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbCrossJoinExpression"/>表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual ISourceFragment CreateSourceForCrossJoin(GenerateContext context, DbExpression expression)
        {
            var content = (DbCrossJoinExpression)expression;
            var source = CreateSource(context, content.Source);
            if (source is SelectFragment select)
            {
                if (select.IsRecommandLock)
                {
                    select = new SelectFragment(context, source);
                }
            }
            else
            {
                select = new SelectFragment(context, source);
            }

            var target = CreateSource(context, content.Target);
            if (target is VirtualSourceFragment virtualSource)
            {
                switch (virtualSource.Expression.ExpressionType)
                {
                    case EExpressionType.CollectionMember:
                        CreateVirtualJoinForCollectionMember(context, content, virtualSource);
                        break;
                    case EExpressionType.GroupJoin:
                        CreateVirtualJoinForGroupJoin(context, content, virtualSource);
                        break;
                    case EExpressionType.GroupBy:
                        CreateVirtualJoinForGroupBy(context, content, virtualSource);
                        break;
                    default:
                        throw new NotSupportedException(string.Format(Res.NotSupportedJoinVirtualSource, expression.ExpressionType));
                }
            }
            else
            {
                select.AddSource(target);
                target.Join = EJoinType.CrossJoin;
            }
            if (IsSelectFragment(content))
            {
                InitialSelectFragment(select, content);
            }
            return select;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbInnerJoinExpression"/>表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual ISourceFragment CreateSourceForInnerJoin(GenerateContext context, DbExpression expression)
        {
            var content = (DbInnerJoinExpression)expression;
            var source = CreateSource(context, content.Source);
            var target = CreateSource(context, content.Target);

            if (source is SelectFragment select)
            {
                if (select.IsRecommandLock)
                {
                    select = new SelectFragment(context, source);
                }
            }
            else
            {
                select = new SelectFragment(context, source);
            }

            select.AddSource(target);
            if (IsSelectFragment(content))
            {
                InitialSelectFragment(select, content);
            }

            target.Join = EJoinType.InnerJoin;
            target.Condition = content.KeyPairs.Select(a => select.CreateExpression(a)).Merge();
            return select;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbDataSetExpression"/>表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual ISourceFragment CreateSourceForDataSet(GenerateContext context, DbExpression expression)
        {
            var content = (DbDataSetExpression)expression;
            var metadata = context.Metadata.Table(content.Item.ClrType);
            SourceFragment table = null;
            if (metadata.InheritSets.Length > 0)
            {
                if (content.Name != null)
                {
                    throw new NotSupportedException(Res.NotSupportedInheritSetUsingDbName);
                }
                table = new InheritFragment(context, metadata);
            }
            else
            {
                table = new TableFragment(context, content);
            }
            context.RegisterSource(content.Item, table);
            context.RegisterSource(content, table);
            if (IsSelectFragment(content))
                return InitialSelectFragment(new SelectFragment(context, table), content);
            if (table is InheritFragment inherit)
            {
                inherit.Initialize();
            }
            return table;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbSelectExpression"/>表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual ISourceFragment CreateSourceForSelect(GenerateContext context, DbExpression expression)
        {
            var content = (DbSelectExpression)expression;
            var source = CreateSource(context, content.Source);
            if (source is SelectFragment select)
            {
                if (!(select.IsRecommandLock && IsForceSelectFragment(content)))
                {
                    InitialSelectFragment(select, content);
                    return select;
                }
            }
            return InitialSelectFragment(new SelectFragment(context, source), content);
        }
        /// <summary>
        /// 根据指定类型<see cref="DbObjectMemberExpression"/>表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual ISourceFragment CreateSourceForObjectMember(GenerateContext context, DbExpression expression)
        {
            var content = (DbObjectMemberExpression)expression;
            var source = GetSource(context, content.Expression);
            if (source is VirtualSourceFragment virtualSource)
            {
                return virtualSource.GetBody();
            }
            else
            {
                var select = (SelectFragment)source.Parent;
                if (select.IsRecommandLock)
                {
                    select = select.Parent as SelectFragment;
                }
                var pairs = content.Pairs;
                if (content.CompositePairs != null)
                {
                    var relation = CreateSource(context, content.RelationSet);
                    select.AddSource(relation);
                    context.RegisterSource(content.RelationSet, relation);
                    relation.Join = EJoinType.LeftJoin;
                    relation.Condition = pairs.Select(a => select.CreateExpression(a)).Merge();
                    pairs = content.CompositePairs;
                }
                var target = CreateSource(context, content.TargetSet);
                select.AddSource(target);
                context.RegisterSource(content.TargetSet, target);
                if (content.CompositePairs == null)
                    target.Join = content.Nullable ? EJoinType.LeftJoin : EJoinType.InnerJoin;
                else
                    target.Join = EJoinType.InnerJoin;
                target.Condition = pairs.Select(a => select.CreateExpression(a)).Merge();
                return target;
            }
        }
        /// <summary>
        /// 根据指定类型<see cref="DbGroupSetExpression"/>表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual ISourceFragment CreateSourceForGroupSet(GenerateContext context, DbExpression expression)
        {
            var content = (DbGroupSetExpression)expression;
            var source = (VirtualSourceFragment)GetSource(context, content.Parent.Item);
            var target = GetSource(context, content.Parent.Target.Item);
            return source;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbGroupByExpression"/>表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual ISourceFragment CreateSourceForGroupBy(GenerateContext context, DbExpression expression)
        {
            var content = (DbGroupByExpression)expression;
            var groupitem = (DbGroupItemExpression)content.Item;
            var source = CreateSource(context, content.Source);
            var body = new SelectFragment(context, source);
            body.RetrievalMembers(content.Key, false);
            foreach (var member in body.Members.OfType<ReferenceMemberFragment>())
            {
                body.GroupBys.Add(member.Reference);
            }
            var container = new SelectFragment(context, body);
            var virtualSource = new VirtualSourceFragment(context, expression, container, source, body);
            context.RegisterSource(content, virtualSource);
            context.RegisterSource(content.Item, virtualSource);
            return container;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbGroupJoinExpression"/>表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual ISourceFragment CreateSourceForGroupJoin(GenerateContext context, DbExpression expression)
        {
            var content = (DbGroupJoinExpression)expression;
            var source = CreateSource(context, content.Source);
            var taget = CreateSource(context, content.Target);
            var container = new SelectFragment(context, source);
            var virtualSource = new VirtualSourceFragment(context, expression, container, source);
            context.RegisterSource(content.Item, virtualSource);
            if (IsSelectFragment(content))
                return InitialSelectFragment(container, content);
            return container;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbCollectionMemberExpression"/>表达式创建数据源语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual ISourceFragment CreateSourceForCollectionMember(GenerateContext context, DbExpression expression)
        {
            var content = (DbCollectionMemberExpression)expression;
            var source = GetSource(context, content.Expression);
            var target = CreateSource(context, content.TargetSet);
            var container = (SelectFragment)source.Parent;
            var virtualSource = new VirtualSourceFragment(context, expression, container, source);
            context.RegisterSource(content, virtualSource);
            return virtualSource;
        }
        //生成复合集合成员虚拟数据源连接。
        private ISourceFragment GenerateVirtualCompositeJoinForCollectionMember(GenerateContext context, DbCollectionMemberExpression content, SelectFragment body, bool iscomposite)
        {
            var relation = CreateSource(context, content.RelationSet);
            body.AddSource(relation);
            relation.Join = EJoinType.InnerJoin;
            var pairs = iscomposite ? content.CompositePairs : content.Pairs;
            relation.Condition = pairs.Select(a => body.CreateExpression(a)).Merge();
            return relation;
        }
        //生成集合成员虚拟数据源连接。
        private void GenerateVirtualJoinForCollectionMember(GenerateContext context, VirtualSourceFragment source, ISourceFragment body, bool iscomposite)
        {
            var content = (DbCollectionMemberExpression)source.Expression;
            var container = source.Container;
            if (container.IsRecommandLock)
                container = container.Parent as SelectFragment;
            body.Join = EJoinType.LeftJoin;
            var pairs = content.Pairs;
            if (content.Metadata.IsComposite && iscomposite)
                pairs = content.CompositePairs;
            body.Condition = pairs.Select(a => container.CreateExpression(a)).Merge();
        }
        /// <summary>
        /// 根据指定类型<see cref="DbCollectionMemberExpression"/>表达式创建虚拟数据源主体语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">虚拟数据源</param>
        /// <returns>查询语句片段。</returns>
        public virtual SelectFragment CreateVirtualBodyForCollectionMember(GenerateContext context, VirtualSourceFragment source)
        {
            var content = (DbCollectionMemberExpression)source.Expression;
            var target = GetSource(context, content.TargetSet.Item);
            var container = source.Container;
            var body = new SelectFragment(context, target);
            if (content.Metadata.IsComposite)
            {
                GenerateVirtualCompositeJoinForCollectionMember(context, content, body, true);
            }
            container.AddSource(body);

            GenerateVirtualJoinForCollectionMember(context, source, body, false);
            foreach (var a in content.Pairs)
                body.GroupBys.Add(body.RetrievalMember(a.Left));
            return body;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbCollectionMemberExpression"/>表达式创建虚拟数据源列表语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">虚拟数据源</param>
        /// <returns>查询语句片段。</returns>
        public virtual SelectFragment CreateVirtualListForCollectionMember(GenerateContext context, VirtualSourceFragment source)
        {
            var content = (DbCollectionMemberExpression)source.Expression;
            var target = GetSource(context, content.TargetSet.Item);
            var container = source.Container;
            if (container.IsRecommandLock)
                container = container.Parent as SelectFragment;
            var list = new SelectFragment(context, target);
            if (content.Metadata.IsComposite)
            {
                GenerateVirtualCompositeJoinForCollectionMember(context, content, list, true);
            }
            container.AddSource(list);

            GenerateVirtualJoinForCollectionMember(context, source, list, false);
            list.RetrievalMembers(content.TargetSet.Item, false);
            if (IsSelectFragment(content))
            {
                context.RegisterTempSource(content.Item, target, delegate ()
                {
                    list = InitialSelectFragment(list, content);
                });
            }
            return list;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbCollectionMemberExpression"/>表达式创建虚拟数据源连接语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">表达式。</param>
        /// <param name="source">虚拟数据源</param>
        /// <returns>查询语句片段。</returns>
        public virtual ISourceFragment CreateVirtualJoinForCollectionMember(GenerateContext context, DbCrossJoinExpression expression, VirtualSourceFragment source)
        {
            var content = (DbCollectionMemberExpression)source.Expression;
            var target = GetSource(context, content.TargetSet.Item);
            var container = source.Container;

            if (content.Metadata.IsComposite)
            {
                var relationSet = GenerateVirtualCompositeJoinForCollectionMember(context, content, container, false);
                relationSet.Join = content.Default == null ? EJoinType.InnerJoin : EJoinType.LeftJoin;
            }
            container.AddSource(target);
            GenerateVirtualJoinForCollectionMember(context, source, target, true);
            if (content.Metadata.IsComposite)
                target.Join = EJoinType.InnerJoin;
            else
                target.Join = content.Default == null ? EJoinType.InnerJoin : EJoinType.LeftJoin;
            context.RegisterSource(content.Item, target);
            return target;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbGroupJoinExpression"/>表达式创建虚拟数据源主体语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">虚拟数据源</param>
        /// <returns>查询语句片段。</returns>
        public virtual SelectFragment CreateVirtualBodyForGroupJoin(GenerateContext context, VirtualSourceFragment source)
        {
            var content = (DbGroupJoinExpression)source.Expression;

            var target = GetSource(context, content.Target.Item);

            var body = new SelectFragment(context, target);
            var container = source.Container;
            container.AddSource(body);

            body.Join = EJoinType.LeftJoin;
            body.Condition = content.KeyPairs.Select(a =>
            {
                body.GroupBys.Add(body.RetrievalMember(a.Right));
                return container.CreateExpression(a);
            }).Merge();
            return body;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbGroupJoinExpression"/>表达式创建虚拟数据源列表语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">虚拟数据源</param>
        /// <returns>查询语句片段。</returns>
        public virtual SelectFragment CreateVirtualListForGroupJoin(GenerateContext context, VirtualSourceFragment source)
        {
            var content = (DbGroupJoinExpression)source.Expression;
            var target = GetSource(context, content.Target.Item);

            var list = new SelectFragment(context, target);
            var container = source.Container;
            if (container.IsRecommandLock)
                container = container.Parent as SelectFragment;
            container.AddSource(list);

            list.Join = EJoinType.LeftJoin;
            list.Condition = content.KeyPairs.Select(a => container.CreateExpression(a)).Merge();
            list.RetrievalMembers(content.Target.Item, false);
            return list;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbGroupJoinExpression"/>表达式创建虚拟数据源连接语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">表达式。</param>
        /// <param name="source">虚拟数据源</param>
        /// <returns>查询语句片段。</returns>
        public virtual ISourceFragment CreateVirtualJoinForGroupJoin(GenerateContext context, DbCrossJoinExpression expression, VirtualSourceFragment source)
        {
            var content = (DbGroupJoinExpression)source.Expression;
            var groupset = (DbGroupSetExpression)expression.Target;

            var target = GetSource(context, content.Target.Item);
            context.RegisterSource(expression.Target.Item, target);

            var joinBody = target;
            var container = source.Container;
            container.AddSource(target);
            joinBody.Join = groupset.Default == null ? EJoinType.InnerJoin : EJoinType.LeftJoin;
            joinBody.Condition = content.KeyPairs.Select(a => container.CreateExpression(a)).Merge();
            return joinBody;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbGroupByExpression"/>表达式创建虚拟数据源列表语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">虚拟数据源</param>
        /// <returns>查询语句片段。</returns>
        public virtual SelectFragment CreateVirtualListForGroupBy(GenerateContext context, VirtualSourceFragment source)
        {
            var content = (DbGroupByExpression)source.Expression;
            var target = source.Source;

            var body = source.GetBody();
            var list = new SelectFragment(context, target);
            var container = source.Container;
            if (container.IsRecommandLock)
                container = container.Parent as SelectFragment;
            container.AddSource(list);
            list.RetrievalMembers(content.Source.Item, false);

            list.Join = EJoinType.InnerJoin;
            list.Condition = body.Members.OfType<ReferenceMemberFragment>().Select(member =>
            {
                var left = list.Members.OfType<ReferenceMemberFragment>().Where(a => a.Reference == member.Reference).Single();
                return new BinaryFragment(context, EBinaryKind.Equal)
                {
                    Left = left,
                    Right = ValidateMember(context, member, container, null, true)
                };
            }).Merge();
            return list;
        }
        /// <summary>
        /// 根据指定类型<see cref="DbGroupByExpression"/>表达式创建虚拟数据源连接语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">表达式。</param>
        /// <param name="source">虚拟数据源</param>
        /// <returns>查询语句片段。</returns>
        public virtual SourceFragment CreateVirtualJoinForGroupBy(GenerateContext context, DbCrossJoinExpression expression, VirtualSourceFragment source)
        {
            throw new NotImplementedException();
        }
    }

    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 判断指定表达式是否需要生成<see cref="SelectFragment"/>语句片段。
        /// </summary>
        /// <param name="unit">判断的表达式。</param>
        /// <returns>如果是返回 true，否则返回 false 。</returns>
        private bool IsSelectFragment(DbUnitTypeExpression unit)
        {
            return unit.ExpressionType != EExpressionType.DataSet
                || unit.Distinct || unit.Skip > 0 || unit.Take > 0
                || unit.Orders.Count > 0 || unit.Filters.Count > 0;
        }
        /// <summary>
        /// 判断指定表达式是否强制需要生成<see cref="SelectFragment"/>语句片段。
        /// </summary>
        /// <param name="unit">判断的表达式。</param>
        /// <returns>如果是返回 true，否则返回 false 。</returns>
        private bool IsForceSelectFragment(DbUnitTypeExpression unit)
        {
            return unit.ExpressionType == EExpressionType.GroupBy
                || unit.Distinct || unit.Skip > 0 || unit.Take > 0
                || unit.Orders.Count > 0;
        }
        /// <summary>
        /// 判断指定语句片段是否需要生成<see cref="SelectFragment"/>语句片段。
        /// </summary>
        /// <param name="source">判断的语句片段。</param>
        /// <returns>如果是返回 true，否则返回 false 。</returns>
        private bool IsForceSelectFragment(SelectFragment source) => source.IsRecommandLock;
        /// <summary>
        /// 根据指定表达式初始化<see cref="SelectFragment"/>语句片段。
        /// </summary>
        /// <param name="select">初始化语句。</param>
        /// <param name="content">表达式对象。</param>
        /// <returns>返回当前语句。</returns>
        private SelectFragment InitialSelectFragment(SelectFragment select, DbUnitTypeExpression content)
        {
            var context = select.Context;
            if (content.Filters.Count > 0)
            {
                if (select.Where == null)
                {
                    select.Where = content.Filters.Select(a => select.CreateExpression(a)).Merge();
                }
                else
                {
                    select.Where.Merge(content.Filters.Select(a => select.CreateExpression(a)));
                }
            }
            select.Take = content.Take;
            select.Skip = content.Skip;
            select.Distinct = content.Distinct;
            if (content.Orders.Count > 0)
            {
                foreach (var sort in content.Orders)
                {
                    select.Sorts.Add(select.CreateExpression(sort));
                }
            }
            if (select.Skip > 0 && content.Orders.Count == 0)
            {
                var temp = GetFirstMemberForOrder(select);
                var member = ValidateMember(context, temp, select, null, true);
                //var member = select.RetrievalMembers(content.Item).First();
                select.Sorts.Add(new SortFragment(context, member));
            }
            return select;
        }
        /// <summary>
        /// 用于为单前查询获取用于排序的成员。
        /// </summary>
        /// <param name="select">指定的查询语句。</param>
        /// <returns>查找结果。</returns>
        private IMemberFragment GetFirstMemberForOrder(SelectFragment select)
        {
            if (select.Members.Any())
                return select.Members.First();
            foreach (var source in select.Sources)
            {
                if (source.Members.Any())
                {
                    var type = source.GetType();
                    return source.Members.First();
                }
                if (source is SelectFragment innerSelect)
                {
                    var temp = GetFirstMemberForOrder(innerSelect);
                    if (temp != null) return temp;
                }
            }
            return null;
        }
        /// <summary>
        /// 根据指定表达式获取相应数据源。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">指定表达式。</param>
        /// <returns>查找结果。</returns>
        protected virtual ISourceFragment GetSource(GenerateContext context, DbExpression expression)
        {
            var result = context.GetSourceFragment(expression);
            if (result != null) return result;
            result = CreateSource(context, expression);
            context.RegisterSource(expression, result);
            return result;
        }
    }
}