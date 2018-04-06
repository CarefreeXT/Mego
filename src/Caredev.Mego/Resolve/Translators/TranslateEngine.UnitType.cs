// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using Caredev.Mego.Exceptions;
using Caredev.Mego.Resolve.Expressions;
using System;
using System.Linq.Expressions;
using System.Reflection;
namespace Caredev.Mego.Resolve.Translators
{
    using Res = Properties.Resources;
    using UnitTypeTranslateMethodType = System.Func<TranslationContext, MethodCallExpression, DbUnitTypeExpression>;

    public partial class TranslateEngine
    {
        private DbUnitTypeExpression UnitTypeTranslate(TranslationContext context, Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)exp;
                    var method = callExpression.Method;
                    if (method.IsGenericMethod)
                        method = method.GetGenericMethodDefinition();
                    if (UnitTypeMethods.TryGetValue(method, out UnitTypeTranslateMethodType action))
                        return action(context, callExpression);
                    break;
                case ExpressionType.Constant:
                    var constant = (ConstantExpression)exp;
                    if (constant.Value is IDbSet dbset)
                    {
                        if (context.Context != dbset.Context)
                        {
                            throw new NotSupportedException(Res.NotSupportedContextIsNotMatch);
                        }
                        return new DbDataSetExpression(constant.Type, dbset.Name);
                    }
                    throw new TranslateException(exp, Res.ExceptionIsMustDbSetInstance);
                case ExpressionType.MemberAccess:
                    var member = (MemberExpression)exp;
                    if (member.Expression.NodeType == ExpressionType.MemberAccess)
                    {
                        var subexp = (MemberExpression)member.Expression;
                        if (subexp.Expression.NodeType == ExpressionType.Constant)
                        {
                            var obj = Expression.Lambda(member).Compile().DynamicInvoke();
                            if (obj is IDbSet dbset2)
                            {
                                if (context.Context != dbset2.Context)
                                {
                                    throw new NotSupportedException(Res.NotSupportedContextIsNotMatch);
                                }
                                return new DbDataSetExpression(exp.Type, dbset2.Name);
                            }
                            throw new TranslateException(exp, Res.ExceptionIsMustDbSetInstance);
                        }
                        if (ExpressionTranslate(context, member) is DbUnitTypeExpression unitExpression)
                        {
                            return unitExpression;
                        }
                    }
                    else
                    {
                        if (ExpressionTranslate(context, member) is DbUnitTypeExpression unitExpression)
                            return unitExpression;
                    }
                    break;
            }
            throw new TranslateException(exp, Res.NotSupportedParseUnitType);
        }

        private DbUnitTypeExpression Include1Translate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var expandunit = source as IDbExpandUnitExpression;
            if (expandunit == null)
            {
                throw new TranslateException(exp.Arguments[0], Res.NotSupportedUnitIsNotExpandUnit);
            }
            var constant = exp.Arguments[1] as ConstantExpression;
            if (constant.Value != null)
            {
                var content = GetMemberExpressionByPath(context, source.Item, constant.Value.ToString());
                expandunit.ExpandItems.Add(content);
            }
            return source;
        }
        private DbUnitTypeExpression Include2Translate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var expandunit = source as IDbExpandUnitExpression;
            if (expandunit == null)
            {
                throw new TranslateException(exp.Arguments[0], Res.NotSupportedUnitIsNotExpandUnit);
            }
            var content = context.Lambda<DbExpression>(body =>
            {
                switch (body.NodeType)
                {
                    case ExpressionType.Call:
                        return UnitTypeTranslate(context, body);
                    case ExpressionType.MemberAccess:
                        return ExpressionTranslate(context, body);
                }
                throw new TranslateException(body, Res.NotSupportedIncludeUnitItem);
            }, exp.Arguments[1], source.Item);
            expandunit.ExpandItems.Add(content);
            return source;
        }
        private DbUnitTypeExpression DefaultIfEmptyTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var defalutset = source as IDbDefaultUnitType;
            if (defalutset == null)
                throw new Exception("与期望数据类型不符合");
            if (exp.Arguments.Count == 2)
                defalutset.Default = ExpressionTranslate(context, exp.Arguments[1]);
            else
                defalutset.Default = new DbDefaultExpression(source.Item.ClrType);
            return source;
        }
        private DbUnitTypeExpression SelectTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            return context.Lambda<DbUnitTypeExpression>(delegate (Expression body)
            {
                var itemType = UnitItemTypeTranslate(context, body) as DbUnitItemTypeExpression;
                switch (itemType.ExpressionType)
                {
                    case EExpressionType.DataItem:
                    case EExpressionType.GroupItem:
                        itemType = new DbUnitItemContentExpression(itemType);
                        break;
                    case EExpressionType.Select:
                        //如果出现重复SELECT调用则自动合并
                        return ((DbSelectExpression)source).ReplaceItem(itemType);
                }
                return new DbSelectExpression(source, itemType);
            }, exp.Arguments[1], source.Item);
        }
        private DbUnitTypeExpression SelectManyTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var target = context.Lambda<DbUnitTypeExpression>(delegate (Expression body)
            {
                return UnitTypeTranslate(context, body);
            }, exp.Arguments[1], source.Item);
            var item = context.Lambda<DbUnitItemTypeExpression>(delegate (Expression body)
            {
                return UnitItemTypeTranslate(context, body) as DbUnitItemTypeExpression;
            }, exp.Arguments[2], source.Item, target.Item);
            if (item.ExpressionType == EExpressionType.DataItem || item.ExpressionType == EExpressionType.GroupItem)
                item = new DbUnitItemContentExpression(item);
            return new DbCrossJoinExpression(source, target, item);
        }
        private DbUnitTypeExpression WhereTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var filter = context.Lambda<DbExpression>(delegate (Expression body)
            {
                return ExpressionTranslate(context, body);
            }, exp.Arguments[1], source.Item);
            source.Filters.Add(filter);
            return source;
        }
        private DbUnitTypeExpression OrderByTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            if (source.Orders.Count > 0)
                source.Orders.Clear();
            return context.Lambda<DbUnitTypeExpression>(delegate (Expression e1)
            {
                source.Orders.Add(new DbOrderExpression(ExpressionTranslate(context, e1), EOrderKind.Ascending));
                return source;
            }, exp.Arguments[1], source.Item);
        }
        private DbUnitTypeExpression OrderByDescendingTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            if (source.Orders.Count > 0)
                source.Orders.Clear();
            return context.Lambda<DbUnitTypeExpression>(delegate (Expression e1)
            {
                source.Orders.Add(new DbOrderExpression(ExpressionTranslate(context, e1), EOrderKind.Descending));
                return source;
            }, exp.Arguments[1], source.Item);
        }
        private DbUnitTypeExpression ThenByTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            return context.Lambda<DbUnitTypeExpression>(delegate (Expression e1)
            {
                source.Orders.Add(new DbOrderExpression(ExpressionTranslate(context, e1), EOrderKind.Ascending));
                return source;
            }, exp.Arguments[1], source.Item);
        }
        private DbUnitTypeExpression ThenByDescendingTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            return context.Lambda<DbUnitTypeExpression>(delegate (Expression e1)
            {
                source.Orders.Add(new DbOrderExpression(ExpressionTranslate(context, e1), EOrderKind.Descending));
                return source;
            }, exp.Arguments[1], source.Item);
        }
        private DbUnitTypeExpression GroupJoinTranslate(TranslationContext context, MethodCallExpression exp)
        {
            return GropuAndJoinTranslate(context, exp, true);
        }
        private DbUnitTypeExpression GroupByTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var result = context.Lambda<DbExpression>(delegate (Expression body)
            {
                return ExpressionTranslate(context, body);
            }, exp.Arguments[1], source.Item);
            var keyProperty = exp.Method.ReturnType.GetGenericArguments()[0].GetProperty("Key");
            if (exp.Arguments.Count == 3)
            {
                var selecter = context.Lambda<DbExpression>(delegate (Expression body)
                {
                    return ExpressionTranslate(context, body);
                }, exp.Arguments[2], source.Item) as DbUnitItemTypeExpression;
                if (selecter == null)
                    throw new Exception("Error");
                return new DbGroupByExpression(source, keyProperty, result, selecter);
            }
            return new DbGroupByExpression(source, keyProperty, result);
        }
        private DbUnitTypeExpression SkipTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var value = (int)((ConstantExpression)exp.Arguments[1]).Value;
            if (value > 0) source.Skip += value;
            return source;
        }
        private DbUnitTypeExpression TakeTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var value = (int)((ConstantExpression)exp.Arguments[1]).Value;
            if (value > 0) source.Take += value;
            return source;
        }
        private DbUnitTypeExpression DistinctTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            source.Distinct = true;
            return source;
        }
        private DbUnitTypeExpression UnionTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var target = UnitTypeTranslate(context, exp.Arguments[1]);
            return new DbSetConnectExpression(source, target, EConnectKind.Union);
        }
        private DbUnitTypeExpression IntersectTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var target = UnitTypeTranslate(context, exp.Arguments[1]);
            return new DbSetConnectExpression(source, target, EConnectKind.Intersect);
        }
        private DbUnitTypeExpression ExceptTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var target = UnitTypeTranslate(context, exp.Arguments[1]);
            return new DbSetConnectExpression(source, target, EConnectKind.Except);
        }
        private DbUnitTypeExpression ConcatTranslate(TranslationContext context, MethodCallExpression exp)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var target = UnitTypeTranslate(context, exp.Arguments[1]);
            return new DbSetConnectExpression(source, target, EConnectKind.Concat);
        }
        private DbUnitTypeExpression JoinTranslate(TranslationContext context, MethodCallExpression exp)
        {
            return GropuAndJoinTranslate(context, exp, false);
        }
    }
}