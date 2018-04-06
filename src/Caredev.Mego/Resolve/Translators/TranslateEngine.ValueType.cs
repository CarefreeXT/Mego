// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using Caredev.Mego.Resolve.Expressions;
using Caredev.Mego.Exceptions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Caredev.Mego.DataAnnotations;

namespace Caredev.Mego.Resolve.Translators
{
    using Res = Properties.Resources;
    using ValueTypeTranslateMethodType = System.Func<TranslationContext, MethodCallExpression, MethodInfo, DbExpression>;

    public partial class TranslateEngine
    {
        private DbExpression ValueTypeTranslate(TranslationContext context, Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Call:
                    var call = (MethodCallExpression)exp;
                    var method = call.Method;
                    if (method.IsGenericMethod)
                        method = method.GetGenericMethodDefinition();
                    if (ValueTypeMethods.TryGetValue(method, out ValueTypeTranslateMethodType action))
                        return action(context, call, method);
                    if (method.IsDefined(typeof(DbFunctionAttribute)))
                        return ScalarCustomFunctionTranslate(context, call, method);
                    if (RecursiveConstantTranslate(context, exp, out DbExpression result))
                        return result;
                    break;
            }
            throw new TranslateException(exp, Res.NotSupportedExpressionParseValueType);
        }

        private DbExpression RetrievalFunctionInlineTranslate(TranslationContext context, MethodCallExpression exp, MethodInfo method)
        {
            var source = ExpressionConvertUnitType(ExpressionTranslate(context, exp.Arguments[0]));
            if (source == null)
            {
                throw new TranslateException(exp.Arguments[0], Res.ExceptionIsNotUnitType);
            }
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
        private DbExpression RetrievalFunctionElementAtInlineTranslate(TranslationContext context, MethodCallExpression exp, MethodInfo method)
        {
            var source = ExpressionConvertUnitType(ExpressionTranslate(context, exp.Arguments[0]));
            if (source == null)
            {
                throw new TranslateException(exp.Arguments[0], Res.ExceptionIsNotUnitType);
            }
            var contant = (ConstantExpression)exp.Arguments[1];
            var value = new DbConstantExpression(contant.Type, contant.Value);
            return new DbRetrievalFunctionExpression(source, method, value);
        }
        private DbExpression AggregateFunctionInlineTranslate(TranslationContext context, MethodCallExpression call, MethodInfo method)
        {
            var source = ExpressionConvertUnitType(ExpressionTranslate(context, call.Arguments[0]));
            if (source == null)
            {
                throw new TranslateException(call.Arguments[0], Res.ExceptionIsNotUnitType);
            }
            if (call.Arguments.Count == 2)
            {
                DbExpression content = context.Lambda<DbExpression>(delegate (Expression body)
                {
                    return ExpressionTranslate(context, body);
                }, call.Arguments[1], source.Item);
                return new DbAggregateFunctionExpression(source, method, content);
            }
            return new DbAggregateFunctionExpression(source, method);
        }
        private DbExpression JudgeFunctionInlineTranslate(TranslationContext context, MethodCallExpression call, MethodInfo method)
        {
            var source = ExpressionConvertUnitType(ExpressionTranslate(context, call.Arguments[0]));
            if (source == null)
            {
                throw new TranslateException(call.Arguments[0], Res.ExceptionIsNotUnitType);
            }
            if (call.Arguments.Count == 2)
            {
                DbExpression content = context.Lambda<DbExpression>(delegate (Expression body)
                {
                    return ExpressionTranslate(context, body);
                }, call.Arguments[1], source.Item);
                return new DbJudgeFunctionExpression(source, method, content);
            }
            return new DbJudgeFunctionExpression(source, method);
        }
        private DbExpression ScalarFunctionInlineTranslate(TranslationContext context, MethodCallExpression call, MethodInfo method)
        {
            if (call.Object != null)
            {
                if (call.Arguments.Count > 0)
                {
                    var argus = new DbExpression[call.Arguments.Count + 1];
                    argus[0] = ExpressionTranslate(context, call.Object);
                    for (int i = 1; i < argus.Length; i++)
                    {
                        argus[i] = ExpressionTranslate(context, call.Arguments[i - 1]);
                    }
                    return new DbScalarFunctionExpression(method, EMapFunctionKind.Function, argus);
                }
                return new DbScalarFunctionExpression(method, EMapFunctionKind.Function, ExpressionTranslate(context, call.Object));
            }
            else
            {
                if (call.Arguments.Count > 0)
                {
                    var argus = call.Arguments.Select(a => ExpressionTranslate(context, a)).ToArray();
                    return new DbScalarFunctionExpression(method, EMapFunctionKind.Function, argus);
                }
                return new DbScalarFunctionExpression(method, EMapFunctionKind.Function);
            }
        }
        private DbExpression ScalarPropertyTranslate(TranslationContext context, MemberExpression member)
        {
            if (member.Expression == null)
                return new DbScalarFunctionExpression(member.Member, EMapFunctionKind.Function);
            else
            {
                var source = ExpressionTranslate(context, member.Expression);
                return new DbScalarFunctionExpression(member.Member, EMapFunctionKind.Function, source);
            }
        }
        private DbExpression ScalarCustomFunctionTranslate(TranslationContext context, MethodCallExpression call, MethodInfo method)
        {
            if (call.Arguments.Count > 0)
            {
                var argus = call.Arguments.Select(a => ExpressionTranslate(context, a)).ToArray();
                return new DbScalarFunctionExpression(method, EMapFunctionKind.Function, argus);
            }
            return new DbScalarFunctionExpression(method, EMapFunctionKind.Function);
        }
    }
}