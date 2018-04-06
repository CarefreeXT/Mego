// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using Caredev.Mego.Common;
using Caredev.Mego.Resolve.Expressions;
using Caredev.Mego.Resolve.Metadatas;
using Caredev.Mego.Resolve.Operates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace Caredev.Mego.Resolve.Translators
{
    using Caredev.Mego.Exceptions;
    using Res = Properties.Resources;
    using ValueTypeTranslatePropertyType = System.Func<TranslationContext, MemberExpression, DbExpression>;
    /// <summary>
    /// 翻译引擎，该引擎用于将LINQ的<see cref="Expression"/>对象翻译为<see cref="DbExpression"/>。
    /// </summary>
    public sealed partial class TranslateEngine
    {
        internal TranslateEngine()
        {
            InitialMethods();
        }
        /// <summary>
        /// 翻译指定的操作。
        /// </summary>
        /// <param name="operate">目标操作。</param>
        /// <returns>数据表达式。</returns>
        public DbExpression Translate(DbOperateBase operate)
        {
            Utility.NotNull(operate, nameof(operate));
            var context = operate.Executor.Context;
            switch (operate)
            {
                case DbQueryOperateBase query:
                    return Translate(query.Expression, context);
                case DbPropertysOperateBase propertys:
                    return Translate(propertys.Expression, context);
                case DbObjectsOperateBase objects:
                    return Translate(objects.Expression, context);
                case DbRelationOperateBase relations:
                    return new DbDataSetExpression(typeof(IEnumerable<>).MakeGenericType(relations.ClrType));
                case DbStatementOperateBase statement:
                    return Translate(statement.Expression, context);
            }
            throw new ArgumentException(string.Format(Res.NotSupportedParseOperate, operate.Type));
        }
        /// <summary>
        /// 翻译指定的表达式。
        /// </summary>
        /// <param name="expression">源表达式。</param>
        /// <param name="context">数据上下文</param>
        /// <returns>数据表达式。</returns>
        public DbExpression Translate(Expression expression, DbContext context)
        {
            return RootTranslate(new TranslationContext(context), expression);
        }

        private DbExpression RootTranslate(TranslationContext context, Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Call:
                    var call = (MethodCallExpression)exp;
                    var method = call.Method;
                    if (method.IsGenericMethod)
                        method = method.GetGenericMethodDefinition();
                    if (UnitTypeMethods.ContainsKey(method))
                        return UnitTypeTranslate(context, exp);
                    else if (UnitItemTypeMethods.ContainsKey(method))
                        return UnitItemTypeMethods[method](context, call, method);
                    else if (ValueTypeMethods.ContainsKey(method))
                        return ValueTypeMethods[method](context, call, method);
                    break;
                case ExpressionType.Constant:
                    return UnitTypeTranslate(context, exp);
                default:
                    return ExpressionTranslate(context, exp);
            }
            throw new TranslateException(exp, Res.NotSupportedParseExpression);
        }

        private DbExpression ExpressionTranslate(TranslationContext context, Expression exp)
        {
            switch (exp)
            {
                case ParameterExpression para:
                    return context.Parameters[para];
                case BinaryExpression binary:
                    var left = ExpressionTranslate(context, binary.Left);
                    var right = ExpressionTranslate(context, binary.Right);
                    if (BinaryKindMap.ContainsKey(binary.NodeType))
                    {
                        return new DbBinaryExpression(BinaryKindMap[binary.NodeType], left, right, exp.Type);
                    }
                    else if (RecursiveConstantTranslate(context, binary, out DbExpression result))
                    {
                        return result;
                    }
                    break;
                case UnaryExpression unary:
                    if (!UnaryKindMap.ContainsKey(unary.NodeType))
                    {
                        throw new TranslateException(exp, Res.NotSupportedParseExpressionToUnary);
                    }
                    return new DbUnaryExpression(UnaryKindMap[unary.NodeType]
                        , ExpressionTranslate(context, unary.Operand), unary.Type);
                case ConstantExpression constant:
                    return new DbConstantExpression(constant.Type, constant.Value);
#if !NET35
                case DefaultExpression def:
                    return new DbDefaultExpression(def.Type);
#endif
                case NewExpression newExp:
                    return NewTranslate(context, newExp);
                case MemberInitExpression initExp:
                    return MemberInitTranslate(context, initExp);
                case MethodCallExpression call:
                    return ValueTypeTranslate(context, call);
                case MemberExpression access:
                    if (ValueTypePropertys.TryGetValue(access.Member, out ValueTypeTranslatePropertyType action))
                        return action(context, access);
                    else if (RecursiveConstantTranslate(context, exp, out DbExpression result))
                    {
                        return result;
                    }
                    else
                    {
                        var property = (PropertyInfo)access.Member;
                        var subaccess = ExpressionTranslate(context, access.Expression);
                        if (subaccess.ExpressionType == EExpressionType.New)
                        {
                            var subnew = (DbNewExpression)subaccess;
                            return subnew.Members[property];
                        }
                        else
                        {
                            switch (subaccess.ExpressionType)
                            {
                                case EExpressionType.DataItem:
                                case EExpressionType.GroupItem:
                                case EExpressionType.UnitItemContent:
                                case EExpressionType.ObjectMember:
                                case EExpressionType.RetrievalFunction:
                                case EExpressionType.OriginalObject:
                                    return CreateMemberExpression(context, property, subaccess);
                                default:
                                    throw new TranslateException(exp, Res.NotSupportedParseExpressionToMemberAccess);
                            }
                        }
                    }
            }
            throw new TranslateException(exp, Res.NotSupportedParseExpression);
        }

        private DbExpression CreateMemberExpression(TranslationContext context, PropertyInfo property, DbExpression subaccess)
        {
            Type type = property.PropertyType;
            return subaccess.GetOrSetChildren(property, () =>
            {
                DbExpression item = null;
                if (type.IsPrimary())
                    item = new DbMemberExpression(property, subaccess);
                else if (type.IsComplexCollection())
                    item = new DbCollectionMemberExpression(context, property, subaccess);
                else
                    item = new DbObjectMemberExpression(context, property, subaccess);
                return item;
            });
        }

        private IDbUnitTypeExpression ExpressionConvertUnitType(DbExpression exp)
        {
            if (exp.ExpressionType == EExpressionType.UnitItemContent)
            {
                var content = (DbUnitItemContentExpression)exp;
                return content.Content as IDbUnitTypeExpression;
            }
            return exp as IDbUnitTypeExpression;
        }

        private DbUnitTypeExpression GropuAndJoinTranslate(TranslationContext context, MethodCallExpression exp, bool isgroup)
        {
            var source = UnitTypeTranslate(context, exp.Arguments[0]);
            var target = UnitTypeTranslate(context, exp.Arguments[1]);

            var left = context.Lambda<DbExpression>(delegate (Expression body)
            {
                return ExpressionTranslate(context, body);
            }, exp.Arguments[2], source.Item);
            var right = context.Lambda<DbExpression>(delegate (Expression body)
            {
                return ExpressionTranslate(context, body);
            }, exp.Arguments[3], target.Item);

            var rightitem = isgroup ? (DbExpression)new DbGroupSetExpression(target) : target.Item;
            var result = context.Lambda<DbUnitItemTypeExpression>(delegate (Expression body)
            {
                return UnitItemTypeTranslate(context, body) as DbUnitItemTypeExpression;
            }, exp.Arguments[4], source.Item, rightitem);

            if (result.ExpressionType == EExpressionType.DataItem || result.ExpressionType == EExpressionType.GroupItem)
                result = new DbUnitItemContentExpression(result);

            if (isgroup)
            {
                var value = new DbGroupJoinExpression(source, target, left, right, result);
                ((DbGroupSetExpression)rightitem).Parent = value;
                return value;
            }
            else
                return new DbInnerJoinExpression(source, target, left, right, result);
        }

        private DbExpression GetMemberExpressionByPath(TranslationContext context, DbUnitItemTypeExpression item, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(Res.ExceptionMemberPathCannotEmpty, nameof(path));
            }
            var paths = path.Split('.');
            var metadata = context.Context.Configuration.Metadata;
            DbExpression current = null;
            NavigateMetadata navigate = null;
            for (var i = 0; i < paths.Length; i++)
            {
                var table = current == null ? metadata.Table(item.ClrType) : navigate.Target;
                var navikeyvalue = table.Navigates.Where(a => a.Key.Name == paths[i]).FirstOrDefault();
                var propertyinfo = navikeyvalue.Key as PropertyInfo;
                if (propertyinfo == null)
                {
                    throw new KeyNotFoundException(Res.ExceptionNotFoundNavigateKey);
                }

                if (propertyinfo.PropertyType.IsComplexCollection() && i != paths.Length - 1)
                {
                    throw new ArgumentException(Res.ExceptionCollectionMemberLevelError);
                }
                current = CreateMemberExpression(context, propertyinfo, current == null ? item : current);
                navigate = navikeyvalue.Value;
            }
            return current;
        }
        //用于递归解析常量值。
        private bool RecursiveConstantTranslate(TranslationContext context, Expression exp, out DbExpression value)
        {
            if (IsConstant(exp))
            {
                var obj = Expression.Lambda(exp).Compile().DynamicInvoke();
                value = new DbConstantExpression(exp.Type, obj);
                return true;
            }
            value = null;
            return false;
        }
        private bool IsConstant(Expression exp)
        {
            var type = exp.Type;
            if (!(typeof(DbContext).IsAssignableFrom(type) || typeof(IContextContent).IsAssignableFrom(type)))
            {
                switch (exp)
                {
                    case ConstantExpression constant:
                        return true;
                    case BinaryExpression binary:
                        return binary.NodeType == ExpressionType.ArrayIndex && IsConstant(binary.Left);
                    case MethodCallExpression call:
                        return call.Method.IsStatic ? IsConstant(call.Arguments[0]) : IsConstant(call.Object);
                    case MemberExpression access:
                        return IsConstant(access.Expression);

                }
            }
            return false;
        }
    }
}