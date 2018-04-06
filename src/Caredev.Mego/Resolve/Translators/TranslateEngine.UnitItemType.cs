// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using Caredev.Mego.Resolve.Expressions;
using Caredev.Mego.Exceptions;
using System;
using System.Linq.Expressions;
using System.Reflection;
namespace Caredev.Mego.Resolve.Translators
{
    using Res = Properties.Resources;
    using UnitItemTypeTranslateMethodType = System.Func<TranslationContext, MethodCallExpression, MethodInfo, DbExpression>;
    public partial class TranslateEngine
    {
        private DbExpression UnitItemTypeTranslate(TranslationContext context, Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)exp;
                    var method = callExpression.Method;
                    if (method.IsGenericMethod)
                        method = method.GetGenericMethodDefinition();
                    if (UnitItemTypeMethods.TryGetValue(method, out UnitItemTypeTranslateMethodType action))
                        return action(context, callExpression, method);
                    break;
                case ExpressionType.New:
                    return NewTranslate(context, (NewExpression)exp);
                case ExpressionType.MemberInit:
                    return MemberInitTranslate(context, (MemberInitExpression)exp);
                case ExpressionType.MemberAccess:
                    var access = ExpressionTranslate(context, exp);
                    var itemType = access as DbUnitItemTypeExpression;
                    if (itemType != null)
                        return new DbUnitItemContentExpression(itemType);
                    switch (access.ExpressionType)
                    {
                        case EExpressionType.MemberAccess:
                            return new DbUnitValueContentExpression((DbMemberExpression)access);
                        case EExpressionType.ObjectMember:
                            return new DbUnitObjectContentExpression((DbObjectMemberExpression)access);
                    }
                    break;
                case ExpressionType.Parameter:
                    return context.Parameters[(ParameterExpression)exp];
            }
            throw new TranslateException(exp, Res.NotSupportedParseUnitItem);
        }

        private DbUnitItemTypeExpression NewTranslate(TranslationContext context, NewExpression exp)
        {
            var memberCount = exp.Members == null ? 0 : exp.Members.Count;
            if (exp.Arguments.Count != memberCount)
            {
                throw new TranslateException(exp, Res.ExceptionIsMustParameterlessConstructor);
            }
            var newExp = new DbNewExpression(exp.Type);
            for (int i = 0; i < memberCount; i++)
            {
                newExp.Members.Add(exp.Members[i], ExpressionTranslate(context, exp.Arguments[i]));
            }
            return newExp;
        }

        private DbUnitItemTypeExpression MemberInitTranslate(TranslationContext context, MemberInitExpression exp)
        {
            if (exp.NewExpression.Arguments.Count > 0)
            {
                throw new TranslateException(exp, Res.ExceptionIsMustParameterlessConstructor);
            }
            var newExp = new DbNewExpression(exp.Type);
            foreach (var bind in exp.Bindings)
            {
                switch (bind.BindingType)
                {
                    case MemberBindingType.Assignment:
                        newExp.Members.Add(bind.Member, ExpressionTranslate(context, ((MemberAssignment)bind).Expression));
                        break;
                    default:
                        throw new NotSupportedException(string.Format(Res.NotSupportedNewMemberBindingType, bind.BindingType));
                }
            }
            return newExp;
        }
        private DbExpression RetrievalFunctionTranslate(TranslationContext context, MethodCallExpression exp, MethodInfo method)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            if (exp.Arguments.Count == 2)
            {
                DbExpression content = context.Lambda<DbExpression>(delegate (Expression body)
                {
                    return ExpressionTranslate(context, body);
                }, exp.Arguments[1], source.Item);
                return new DbRetrievalFunctionExpression(source, method, content);
            }
            return new DbRetrievalFunctionExpression(source, method);
        }
        private DbExpression RetrievalFunctionElementAtTranslate(TranslationContext context, MethodCallExpression exp, MethodInfo method)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var contant = (ConstantExpression)exp.Arguments[1];
            var value = new DbConstantExpression(contant.Type, contant.Value);
            return new DbRetrievalFunctionExpression(source, method, value);
        }
        private DbExpression AggregateFunctionTranslate(TranslationContext context, MethodCallExpression exp, MethodInfo method)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            if (exp.Arguments.Count == 2)
            {
                DbExpression content = context.Lambda<DbExpression>(delegate (Expression body)
                {
                    return ExpressionTranslate(context, body);
                }, exp.Arguments[1], source.Item);
                return new DbAggregateFunctionExpression(source, method, content);
            }
            return new DbAggregateFunctionExpression(source, method);
        }
        private DbExpression JudgeFunctionTranslate(TranslationContext context, MethodCallExpression exp, MethodInfo method)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            if (exp.Arguments.Count == 2)
            {
                DbExpression content = context.Lambda<DbExpression>(delegate (Expression body)
                {
                    return ExpressionTranslate(context, body);
                }, exp.Arguments[1], source.Item);
                return new DbJudgeFunctionExpression(source, method, content);
            }
            return new DbJudgeFunctionExpression(source, method);
        }
    }
}