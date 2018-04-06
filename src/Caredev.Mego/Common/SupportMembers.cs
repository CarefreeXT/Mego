// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq.Expressions;
    using System.Reflection;
    using Res = Properties.Resources;
    /// <summary>
    /// 本系统默认支持解析的方法及属性。
    /// </summary>
    public static class SupportMembers
    {
        /// <summary>
        /// 获取泛型的<see cref="System.Func{TResult}"/>实际类型。
        /// </summary>
        /// <param name="types">类型参数集合。</param>
        /// <returns>返回泛型化后的函数对象。</returns>
        private static System.Type GetFunctionType(System.Type[] types)
        {
            switch (types.Length)
            {
                case 01: return typeof(System.Func<>).MakeGenericType(types);
                case 02: return typeof(System.Func<,>).MakeGenericType(types);
                case 03: return typeof(System.Func<,,>).MakeGenericType(types);
                case 04: return typeof(System.Func<,,,>).MakeGenericType(types);
                case 05: return typeof(System.Func<,,,,>).MakeGenericType(types);
                default: throw new NotSupportedException(Res.ExceptionFuncTypeArgumentNumberError);
            }
        }
        /// <summary>
        /// 比较参数类型是否与指定的类型集合组成的目标<see cref="System.Linq.Expressions.Expression{TDelegate}"/>类型相同。
        /// </summary>
        /// <param name="parameter">指定的参数。</param>
        /// <param name="types">指定的类型集合。</param>
        /// <returns>相同返回 true，否则返回 false。</returns>
        private static bool CompareExpression(ParameterInfo parameter, params System.Type[] types)
        {
            System.Type funcType = GetFunctionType(types);
            return parameter.ParameterType == typeof(Expression<>).MakeGenericType(funcType);
        }
        /// <summary>
        /// 比较参数类型是否与指定的类型集合组成的目标<see cref="System.Func{TResult}"/>类型相同。
        /// </summary>
        /// <param name="parameter">指定的参数。</param>
        /// <param name="types">指定的类型集合。</param>
        /// <returns>相同返回 true，否则返回 false。</returns>
        private static bool CompareFunction(ParameterInfo parameter, params System.Type[] types)
        {
            return parameter.ParameterType == GetFunctionType(types);
        }
        /// <summary>
        /// 将参数类型与指定的类型集合依次做比较。
        /// </summary>
        /// <param name="parameter">参数信息。</param>
        /// <param name="arg0">比较的前置参数类型。</param>
        /// <param name="types">比较的类型集合。</param>
        /// <returns>存在相同返回 true，否则返回 false。</returns>
        private static bool CompareExpressions(ParameterInfo parameter, System.Type arg0, params System.Type[] types)
        {
            foreach (var type in types)
            {
                if (CompareExpression(parameter, arg0, type))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 将参数类型与指定的类型集合依次做比较。
        /// </summary>
        /// <param name="parameter">参数信息。</param>
        /// <param name="arg0">比较的前置参数类型。</param>
        /// <param name="types">比较的类型集合。</param>
        /// <returns>存在相同返回 true，否则返回 false。</returns>
        private static bool CompareFunctions(ParameterInfo parameter, System.Type arg0, params System.Type[] types)
        {
            foreach (var type in types)
            {
                if (CompareFunction(parameter, arg0, type))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 依次设置方法数组的值。
        /// </summary>
        /// <param name="methods">目标数组。</param>
        /// <param name="method">当前方法。</param>
        private static void SetArrayValue(MethodInfo[] methods, MethodInfo method)
        {
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i] == null)
                {
                    methods[i] = method;
                    return;
                }
            }
        }
        /// <summary>
        /// <see cref="System.Linq.Queryable"/>支持成员
        /// </summary>
        public static class Queryable
        {
            static Queryable()
            {
                System.Type queryableType = typeof(System.Linq.Queryable);
                foreach (var method in queryableType.GetMethods())
                {
                    var parameters = method.GetParameters();
                    if (method.IsGenericMethod && parameters.Length > 0)
                    {
                        var arguments = method.GetGenericArguments();
                        if (parameters[0].ParameterType == typeof(System.Linq.IQueryable<>).MakeGenericType(arguments[0])
                            || parameters[0].ParameterType == typeof(System.Linq.IOrderedQueryable<>).MakeGenericType(arguments[0]))
                        {
                            switch (method.Name)
                            {
                                case nameof(System.Linq.Queryable.Where):
                                    if (CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        Where = method;
                                    break;
                                case nameof(System.Linq.Queryable.Select):
                                    if (arguments.Length == 2 && CompareExpression(parameters[1], arguments[0], arguments[1]))
                                        Select = method;
                                    break;
                                case nameof(System.Linq.Queryable.SelectMany):
                                    if (arguments.Length == 3 && parameters.Length == 3
                                        && CompareExpression(parameters[1], arguments[0], typeof(IEnumerable<>).MakeGenericType(arguments[1]))
                                        && CompareExpression(parameters[2], arguments[0], arguments[1], arguments[2]))
                                        SelectMany = method;
                                    break;
                                case nameof(System.Linq.Queryable.Join):
                                    if (parameters.Length == 5
                                        && parameters[1].ParameterType == typeof(IEnumerable<>).MakeGenericType(arguments[1])
                                        && CompareExpression(parameters[2], arguments[0], arguments[2])
                                        && CompareExpression(parameters[3], arguments[1], arguments[2])
                                        && CompareExpression(parameters[4], arguments[0], arguments[1], arguments[3]))
                                        Join = method;
                                    break;
                                case nameof(System.Linq.Queryable.GroupJoin):
                                    if (parameters.Length == 5
                                       && parameters[1].ParameterType == typeof(IEnumerable<>).MakeGenericType(arguments[1])
                                       && CompareExpression(parameters[2], arguments[0], arguments[2])
                                       && CompareExpression(parameters[3], arguments[1], arguments[2])
                                       && CompareExpression(parameters[4], arguments[0], typeof(IEnumerable<>).MakeGenericType(arguments[1]), arguments[3]))
                                        GroupJoin = method;
                                    break;
                                case nameof(System.Linq.Queryable.GroupBy)://存在多种情况
                                    if (arguments.Length == 2 && parameters.Length == 2 && CompareExpression(parameters[1], arguments[0], arguments[1]))
                                        GroupBy2 = method;
                                    else if (arguments.Length == 3 && parameters.Length == 3 && CompareExpression(parameters[1], arguments[0], arguments[1])
                                        && CompareExpression(parameters[2], arguments[0], arguments[2]))
                                        GroupBy3 = method;
                                    break;
                                case nameof(System.Linq.Queryable.OrderBy):
                                    if (parameters.Length == 2 && CompareExpression(parameters[1], arguments[0], arguments[1]))
                                        OrderBy = method;
                                    break;
                                case nameof(System.Linq.Queryable.OrderByDescending):
                                    if (parameters.Length == 2 && CompareExpression(parameters[1], arguments[0], arguments[1]))
                                        OrderByDescending = method;
                                    break;
                                case nameof(System.Linq.Queryable.ThenBy):
                                    if (parameters.Length == 2 && CompareExpression(parameters[1], arguments[0], arguments[1]))
                                        ThenBy = method;
                                    break;
                                case nameof(System.Linq.Queryable.ThenByDescending):
                                    if (parameters.Length == 2 && CompareExpression(parameters[1], arguments[0], arguments[1]))
                                        ThenByDescending = method;
                                    break;
                                case nameof(System.Linq.Queryable.Skip):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(int))
                                        Skip = method;
                                    break;
                                case nameof(System.Linq.Queryable.Take):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(int))
                                        Take = method;
                                    break;
                                case nameof(System.Linq.Queryable.Distinct):
                                    if (parameters.Length == 1)
                                        Distinct = method;
                                    break;
                                case nameof(System.Linq.Queryable.Union):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(IEnumerable<>).MakeGenericType(arguments[0]))
                                        Union = method;
                                    break;
                                case nameof(System.Linq.Queryable.Intersect):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(IEnumerable<>).MakeGenericType(arguments[0]))
                                        Intersect = method;
                                    break;
                                case nameof(System.Linq.Queryable.Except):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(IEnumerable<>).MakeGenericType(arguments[0]))
                                        Except = method;
                                    break;
                                case nameof(System.Linq.Queryable.Concat):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(IEnumerable<>).MakeGenericType(arguments[0]))
                                        Concat = method;
                                    break;
                                case nameof(System.Linq.Queryable.DefaultIfEmpty):
                                    if (parameters.Length == 1)
                                        DefaultIfEmpty1 = method;
                                    else if (parameters.Length == 2 && parameters[1].ParameterType == arguments[0])
                                        DefaultIfEmpty2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.Contains):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == arguments[0])
                                        Contains = method;
                                    break;
                                case nameof(System.Linq.Queryable.First):
                                    if (parameters.Length == 1)
                                        First1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        First2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.FirstOrDefault):
                                    if (parameters.Length == 1)
                                        FirstOrDefault1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        FirstOrDefault2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.Last):
                                    if (parameters.Length == 1)
                                        Last1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        Last2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.LastOrDefault):
                                    if (parameters.Length == 1)
                                        LastOrDefault1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        LastOrDefault2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.Single):
                                    if (parameters.Length == 1)
                                        Single1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        Single2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.SingleOrDefault):
                                    if (parameters.Length == 1)
                                        SingleOrDefault1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        SingleOrDefault2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.ElementAt):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(int))
                                        ElementAt = method;
                                    break;
                                case nameof(System.Linq.Queryable.ElementAtOrDefault):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(int))
                                        ElementAtOrDefault = method;
                                    break;
                                case nameof(System.Linq.Queryable.Count):
                                    if (parameters.Length == 1)
                                        Count1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        Count2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.LongCount):
                                    if (parameters.Length == 1)
                                        LongCount1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        LongCount2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.Max):
                                    if (parameters.Length == 1)
                                        Max1 = method;
                                    else if (parameters.Length == 2 && arguments.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], arguments[1]))
                                        Max2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.Min):
                                    if (parameters.Length == 1)
                                        Min1 = method;
                                    else if (parameters.Length == 2 && arguments.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], arguments[1]))
                                        Min2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.Any):
                                    if (parameters.Length == 1)
                                        Any1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        Any2 = method;
                                    break;
                                case nameof(System.Linq.Queryable.All):
                                    if (arguments.Length == 1 && parameters.Length == 2 &&
                                        CompareExpression(parameters[1], arguments[0], typeof(bool)))
                                        All = method;
                                    break;
                                case nameof(System.Linq.Queryable.Sum):
                                    if (parameters.Length == 2 &&
                                        CompareExpressions(parameters[1], arguments[0]
                                            , typeof(int), typeof(float), typeof(long), typeof(double), typeof(decimal)
                                            , typeof(int?), typeof(float?), typeof(long?), typeof(double?), typeof(decimal?)))
                                        SetArrayValue(Sum2, method);
                                    break;
                                case nameof(System.Linq.Queryable.Average):
                                    if (parameters.Length == 2 &&
                                        CompareExpressions(parameters[1], arguments[0]
                                            , typeof(int), typeof(float), typeof(long), typeof(double), typeof(decimal)
                                            , typeof(int?), typeof(float?), typeof(long?), typeof(double?), typeof(decimal?)))
                                        SetArrayValue(Average2, method);
                                    break;
                            }
                        }
                    }
                    else if (parameters.Length == 1 && parameters[0].ParameterType.IsGenericType
                        && parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(System.Linq.IQueryable<>))
                    {
                        switch (method.Name)
                        {
                            case nameof(System.Linq.Queryable.Sum):
                                SetArrayValue(Sum1, method);
                                break;
                            case nameof(System.Linq.Queryable.Average):
                                SetArrayValue(Average1, method);
                                break;
                        }
                    }
                }

                foreach (var method in typeof(DbSetExtensions).GetMethods())
                {
                    if (method.Name == "Include" && method.IsGenericMethod)
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length == 2)
                        {
                            var arguments = method.GetGenericArguments();
                            if (parameters[0].ParameterType == typeof(System.Linq.IQueryable<>).MakeGenericType(arguments[0]))
                            {
                                if (arguments.Length == 1 && parameters[1].ParameterType == typeof(string))
                                    Include1 = method;
                                else if (arguments.Length == 2 && CompareExpression(parameters[1], arguments[0], arguments[1]))
                                    Include2 = method;
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// <see cref="System.Linq.Queryable.Where{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo Where;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Select{TSource, TResult}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, TResult}})"/>
            /// </summary>
            public static readonly MethodInfo Select;
            /// <summary>
            /// <see cref="System.Linq.Queryable.SelectMany{TSource, TCollection, TResult}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, int, IEnumerable{TCollection}}}, Expression{Func{TSource, TCollection, TResult}})"/>
            /// </summary>
            public static readonly MethodInfo SelectMany;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Join{TOuter, TInner, TKey, TResult}(System.Linq.IQueryable{TOuter}, IEnumerable{TInner}, Expression{Func{TOuter, TKey}}, Expression{Func{TInner, TKey}}, Expression{Func{TOuter, TInner, TResult}})"/>
            /// </summary>
            public static readonly MethodInfo Join;
            /// <summary>
            /// <see cref="System.Linq.Queryable.GroupJoin{TOuter, TInner, TKey, TResult}(System.Linq.IQueryable{TOuter}, IEnumerable{TInner}, Expression{Func{TOuter, TKey}}, Expression{Func{TInner, TKey}}, Expression{Func{TOuter, IEnumerable{TInner}, TResult}})"/>
            /// </summary>
            public static readonly MethodInfo GroupJoin;
            /// <summary>
            /// <see cref="System.Linq.Queryable.GroupBy{TSource, TKey}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
            /// </summary>
            public static readonly MethodInfo GroupBy2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.GroupBy{TSource, TKey, TElement, TResult}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, TKey}}, Expression{Func{TSource, TElement}}, Expression{Func{TKey, IEnumerable{TElement}, TResult}})"/>
            /// </summary>
            public static readonly MethodInfo GroupBy3;
            /// <summary>
            /// <see cref="System.Linq.Queryable.OrderBy{TSource, TKey}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
            /// </summary>
            public static readonly MethodInfo OrderBy;
            /// <summary>
            /// <see cref="System.Linq.Queryable.OrderByDescending{TSource, TKey}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
            /// </summary>
            public static readonly MethodInfo OrderByDescending;
            /// <summary>
            /// <see cref="System.Linq.Queryable.ThenBy{TSource, TKey}(System.Linq.IOrderedQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
            /// </summary>
            public static readonly MethodInfo ThenBy;
            /// <summary>
            /// <see cref="System.Linq.Queryable.ThenByDescending{TSource, TKey}(System.Linq.IOrderedQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
            /// </summary>
            public static readonly MethodInfo ThenByDescending;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Skip{TSource}(System.Linq.IQueryable{TSource}, int)"/>
            /// </summary>
            public static readonly MethodInfo Skip;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Take{TSource}(System.Linq.IQueryable{TSource}, int)"/>
            /// </summary>
            public static readonly MethodInfo Take;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Distinct{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Distinct;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Union{TSource}(System.Linq.IQueryable{TSource}, IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Union;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Except{TSource}(System.Linq.IQueryable{TSource}, IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Except;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Intersect{TSource}(System.Linq.IQueryable{TSource}, IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Intersect;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Concat{TSource}(System.Linq.IQueryable{TSource}, IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Concat;
            /// <summary>
            /// <see cref="System.Linq.Queryable.DefaultIfEmpty{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo DefaultIfEmpty1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.DefaultIfEmpty{TSource}(System.Linq.IQueryable{TSource}, TSource)"/>
            /// </summary>
            public static readonly MethodInfo DefaultIfEmpty2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Contains{TSource}(System.Linq.IQueryable{TSource}, TSource)"/>
            /// </summary>
            public static readonly MethodInfo Contains;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Count{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Count1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Count{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo Count2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.LongCount{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo LongCount1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.LongCount{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo LongCount2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Max{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Max1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Max{TSource, TResult}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, TResult}})"/>
            /// </summary>
            public static readonly MethodInfo Max2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Min{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Min1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Min{TSource, TResult}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, TResult}})"/>
            /// </summary>
            public static readonly MethodInfo Min2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Average(System.Linq.IQueryable{int})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Average1 = new MethodInfo[10];
            /// <summary>
            /// <see cref="System.Linq.Queryable.Average{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, int}})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Average2 = new MethodInfo[10];
            /// <summary>
            /// <see cref="System.Linq.Queryable.Sum{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, int}})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Sum1 = new MethodInfo[10];
            /// <summary>
            /// <see cref="System.Linq.Queryable.Sum{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, float}})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Sum2 = new MethodInfo[10];
            /// <summary>
            /// <see cref="System.Linq.Queryable.Any{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Any1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Any{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo Any2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.All{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo All;
            /// <summary>
            /// <see cref="System.Linq.Queryable.ElementAt{TSource}(System.Linq.IQueryable{TSource}, int)"/>
            /// </summary>
            public static readonly MethodInfo ElementAt;
            /// <summary>
            /// <see cref="System.Linq.Queryable.ElementAtOrDefault{TSource}(System.Linq.IQueryable{TSource}, int)"/>
            /// </summary>
            public static readonly MethodInfo ElementAtOrDefault;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Single{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Single1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Single{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo Single2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.SingleOrDefault{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo SingleOrDefault1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.SingleOrDefault{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo SingleOrDefault2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Last{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Last1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.Last{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo Last2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.LastOrDefault{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo LastOrDefault1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.LastOrDefault{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo LastOrDefault2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.First{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo First1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.First{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo First2;
            /// <summary>
            /// <see cref="System.Linq.Queryable.FirstOrDefault{TSource}(System.Linq.IQueryable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo FirstOrDefault1;
            /// <summary>
            /// <see cref="System.Linq.Queryable.FirstOrDefault{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
            /// </summary>
            public static readonly MethodInfo FirstOrDefault2;
            /// <summary>
            /// <see cref="DbSetExtensions.Include{TSource, TTarget}(IEnumerable{TSource}, Expression{Func{TSource, TTarget}})"/>
            /// </summary>
            public static readonly MethodInfo Include1;
            /// <summary>
            /// <see cref="DbSetExtensions.Include{TSource, TTarget}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, TTarget}})"/>
            /// </summary>
            public static readonly MethodInfo Include2;
        }
        /// <summary>
        /// <see cref="System.Linq.Enumerable"/>支持成员
        /// </summary>
        public static class Enumerable
        {
            static Enumerable()
            {
                System.Type queryableType = typeof(System.Linq.Enumerable);
                foreach (var method in queryableType.GetMethods())
                {
                    var parameters = method.GetParameters();
                    if (method.IsGenericMethod && parameters.Length > 0)
                    {
                        var arguments = method.GetGenericArguments();
                        if (parameters[0].ParameterType == typeof(IEnumerable<>).MakeGenericType(arguments[0])
                            || parameters[0].ParameterType == typeof(System.Linq.IOrderedEnumerable<>).MakeGenericType(arguments[0]))
                        {
                            switch (method.Name)
                            {
                                case nameof(System.Linq.Enumerable.Where):
                                    if (CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        Where = method;
                                    break;
                                case nameof(System.Linq.Enumerable.DefaultIfEmpty):
                                    if (parameters.Length == 1)
                                        DefaultIfEmpty1 = method;
                                    else if (parameters.Length == 2 && parameters[1].ParameterType == arguments[0])
                                        DefaultIfEmpty2 = method;
                                    break;
                                case nameof(System.Linq.Enumerable.Contains):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == arguments[0])
                                        Contains = method;
                                    break;
                                case nameof(System.Linq.Enumerable.First):
                                    if (parameters.Length == 1)
                                        First1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        First2 = method;
                                    break;
                                case nameof(System.Linq.Enumerable.FirstOrDefault):
                                    if (parameters.Length == 1)
                                        FirstOrDefault1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        FirstOrDefault2 = method;
                                    break;
                                case nameof(System.Linq.Enumerable.Last):
                                    if (parameters.Length == 1)
                                        Last1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        Last2 = method;
                                    break;
                                case nameof(System.Linq.Enumerable.LastOrDefault):
                                    if (parameters.Length == 1)
                                        LastOrDefault1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        LastOrDefault2 = method;
                                    break;
                                case nameof(System.Linq.Enumerable.Single):
                                    if (parameters.Length == 1)
                                        Single1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        Single2 = method;
                                    break;
                                case nameof(System.Linq.Enumerable.SingleOrDefault):
                                    if (parameters.Length == 1)
                                        SingleOrDefault1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        SingleOrDefault2 = method;
                                    break;
                                case nameof(System.Linq.Enumerable.ElementAt):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(int))
                                        ElementAt = method;
                                    break;
                                case nameof(System.Linq.Enumerable.ElementAtOrDefault):
                                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(int))
                                        ElementAtOrDefault = method;
                                    break;
                                case nameof(System.Linq.Enumerable.Count):
                                    if (parameters.Length == 1)
                                        Count1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        Count2 = method;
                                    break;
                                case nameof(System.Linq.Enumerable.LongCount):
                                    if (parameters.Length == 1)
                                        LongCount1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        LongCount2 = method;
                                    break;
                                case nameof(System.Linq.Enumerable.Any):
                                    if (parameters.Length == 1)
                                        Any1 = method;
                                    else if (parameters.Length == 2 &&
                                        CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        Any2 = method;
                                    break;
                                case nameof(System.Linq.Enumerable.All):
                                    if (arguments.Length == 1 && parameters.Length == 2 &&
                                        CompareFunction(parameters[1], arguments[0], typeof(bool)))
                                        All = method;
                                    break;
                                case nameof(System.Linq.Enumerable.Sum):
                                    if (parameters.Length == 2 &&
                                        CompareFunctions(parameters[1], arguments[0]
                                            , typeof(int), typeof(float), typeof(long), typeof(double), typeof(decimal)
                                            , typeof(int?), typeof(float?), typeof(long?), typeof(double?), typeof(decimal?)))
                                        SetArrayValue(Sum2, method);
                                    break;
                                case nameof(System.Linq.Enumerable.Average):
                                    if (parameters.Length == 2 &&
                                        CompareFunctions(parameters[1], arguments[0]
                                            , typeof(int), typeof(float), typeof(long), typeof(double), typeof(decimal)
                                            , typeof(int?), typeof(float?), typeof(long?), typeof(double?), typeof(decimal?)))
                                        SetArrayValue(Average2, method);
                                    break;
                                case nameof(System.Linq.Enumerable.Max):
                                    if (parameters.Length == 2 &&
                                        CompareFunctions(parameters[1], arguments[0]
                                            , typeof(int), typeof(float), typeof(long), typeof(double), typeof(decimal)
                                            , typeof(int?), typeof(float?), typeof(long?), typeof(double?), typeof(decimal?)))
                                        SetArrayValue(Max2, method);
                                    else if (parameters.Length == 2 && arguments.Length == 2 && CompareFunction(parameters[1], arguments[0], arguments[1]))
                                        SetArrayValue(Max2, method);
                                    else if (parameters.Length == 1 && method.ReturnType == arguments[0])
                                        SetArrayValue(Max2, method);
                                    break;
                                case nameof(System.Linq.Enumerable.Min):
                                    if (parameters.Length == 2 &&
                                        CompareFunctions(parameters[1], arguments[0]
                                            , typeof(int), typeof(float), typeof(long), typeof(double), typeof(decimal)
                                            , typeof(int?), typeof(float?), typeof(long?), typeof(double?), typeof(decimal?)))
                                        SetArrayValue(Min2, method);
                                    else if (parameters.Length == 2 && arguments.Length == 2 && CompareFunction(parameters[1], arguments[0], arguments[1]))
                                        SetArrayValue(Min2, method);
                                    else if (parameters.Length == 1 && method.ReturnType == arguments[0])
                                        SetArrayValue(Min2, method);
                                    break;
                            }
                        }
                    }
                    else if (parameters.Length == 1 && parameters[0].ParameterType.IsGenericType
                        && parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        switch (method.Name)
                        {
                            case nameof(System.Linq.Enumerable.Sum):
                                SetArrayValue(Sum1, method);
                                break;
                            case nameof(System.Linq.Enumerable.Average):
                                SetArrayValue(Average1, method);
                                break;
                            case nameof(System.Linq.Enumerable.Max):
                                SetArrayValue(Max1, method);
                                break;
                            case nameof(System.Linq.Enumerable.Min):
                                SetArrayValue(Min1, method);
                                break;
                        }
                    }
                }

                foreach (var method in typeof(DbSetExtensions).GetMethods())
                {
                    if (method.IsGenericMethod)
                    {
                        var parameters = method.GetParameters();
                        switch (method.Name)
                        {
                            case nameof(DbSetExtensions.Include):
                                if (parameters.Length == 2)
                                {
                                    var arguments = method.GetGenericArguments();
                                    if (parameters[0].ParameterType == typeof(IEnumerable<>).MakeGenericType(arguments[0]))
                                    {
                                        if (arguments.Length == 1 && parameters[1].ParameterType == typeof(string))
                                            Include1 = method;
                                        else if (arguments.Length == 2 && CompareExpression(parameters[1], arguments[0], arguments[1]))
                                            Include2 = method;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo Where;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.DefaultIfEmpty{TSource}(IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo DefaultIfEmpty1;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.DefaultIfEmpty{TSource}(IEnumerable{TSource}, TSource)"/>
            /// </summary>
            public static readonly MethodInfo DefaultIfEmpty2;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Contains{TSource}(IEnumerable{TSource}, TSource)"/>
            /// </summary>
            public static readonly MethodInfo Contains;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Max(IEnumerable{int})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Max1 = new MethodInfo[10];
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, int})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Max2 = new MethodInfo[12];
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Min(IEnumerable{int})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Min1 = new MethodInfo[10];
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Min{TSource}(IEnumerable{TSource}, Func{TSource, int})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Min2 = new MethodInfo[12];
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Count{TSource}(IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Count1;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Count{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo Count2;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.LongCount{TSource}(IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo LongCount1;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.LongCount{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo LongCount2;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Average(IEnumerable{int})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Average1 = new MethodInfo[10];
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, int})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Average2 = new MethodInfo[10];
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Sum(IEnumerable{int})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Sum1 = new MethodInfo[10];
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, int})"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Sum2 = new MethodInfo[10];
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Any{TSource}(IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Any1;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Any{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo Any2;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.All{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo All;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.ElementAt{TSource}(IEnumerable{TSource}, int)"/>
            /// </summary>
            public static readonly MethodInfo ElementAt;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.ElementAtOrDefault{TSource}(IEnumerable{TSource}, int)"/>
            /// </summary>
            public static readonly MethodInfo ElementAtOrDefault;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Single{TSource}(IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Single1;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Single{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo Single2;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo SingleOrDefault1;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo SingleOrDefault2;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Last{TSource}(IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo Last1;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.Last{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo Last2;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.LastOrDefault{TSource}(IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo LastOrDefault1;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.LastOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo LastOrDefault2;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.First{TSource}(IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo First1;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.First{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo First2;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource})"/>
            /// </summary>
            public static readonly MethodInfo FirstOrDefault1;
            /// <summary>
            /// <see cref="System.Linq.Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
            /// </summary>
            public static readonly MethodInfo FirstOrDefault2;

            /// <summary>
            /// <see cref="DbSetExtensions.Include{TSource, TTarget}(IEnumerable{TSource}, Expression{Func{TSource, TTarget}})"/>
            /// </summary>
            public static readonly MethodInfo Include1;
            /// <summary>
            /// <see cref="DbSetExtensions.Include{TSource, TTarget}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, TTarget}})"/>
            /// </summary>
            public static readonly MethodInfo Include2;
        }
        /// <summary>
        /// <see cref="System.String"/>支持成员
        /// </summary>
        public static class String
        {
            static String()
            {
                foreach (var method in typeof(System.String).GetMethods())
                {
                    var parameters = method.GetParameters();
                    switch (method.Name)
                    {
                        case nameof(System.String.Trim):
                            if (parameters.Length == 0 && method.ReturnType == typeof(string)) Trim = method;
                            break;
                        case nameof(System.String.TrimStart):
                            if (parameters.Length == 1 && method.ReturnType == typeof(string)) TrimStart = method;
                            break;
                        case nameof(System.String.TrimEnd):
                            if (parameters.Length == 1 && method.ReturnType == typeof(string)) TrimEnd = method;
                            break;
                        case nameof(System.String.ToUpper):
                            if (parameters.Length == 0 && method.ReturnType == typeof(string)) ToUpper = method;
                            break;
                        case nameof(System.String.ToLower):
                            if (parameters.Length == 0 && method.ReturnType == typeof(string)) ToLower = method;
                            break;
                        case nameof(System.String.StartsWith):
                            if (parameters.Length == 1 && method.ReturnType == typeof(bool)) StartsWith = method;
                            break;
                        case nameof(System.String.EndsWith):
                            if (parameters.Length == 1 && method.ReturnType == typeof(bool)) EndsWith = method;
                            break;
                        case nameof(System.String.IsNullOrEmpty):
                            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string) && method.ReturnType == typeof(bool)) IsNullOrEmpty = method;
                            break;
#if !NET35
                        case nameof(System.String.IsNullOrWhiteSpace):
                            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string) && method.ReturnType == typeof(bool)) IsNullOrWhiteSpace = method;
                            break;
#endif
                        case nameof(System.String.Substring):
                            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(int) && method.ReturnType == typeof(string))
                                Substring1 = method;
                            else if (parameters.Length == 2 && parameters[0].ParameterType == typeof(int) && parameters[1].ParameterType == typeof(int) && method.ReturnType == typeof(string))
                                Substring2 = method;
                            break;
                        case nameof(System.String.Contains):
                            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string) && method.ReturnType == typeof(bool)) Contains = method;
                            break;
                        case nameof(System.String.Replace):
                            if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(string) && method.ReturnType == typeof(string))
                                Replace = method;
                            break;
                        case nameof(System.String.Concat):
                            if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(string) && method.ReturnType == typeof(string))
                                Concat = method;
                            break;
                        case nameof(System.String.Compare):
                            if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(string) && method.ReturnType == typeof(int))
                                Compare = method;
                            break;
                    }
                }

                foreach (var property in typeof(string).GetProperties())
                {
                    switch (property.Name)
                    {
                        case nameof(System.String.Length):
                            Length = property;
                            break;
                    }
                }
            }
            /// <summary>
            /// <see cref="System.String.IsNullOrEmpty"/>
            /// </summary>
            public static readonly MethodInfo IsNullOrEmpty;
#if !NET35
            /// <summary>
            /// <see cref="System.String.IsNullOrWhiteSpace"/>
            /// </summary>
            public static readonly MethodInfo IsNullOrWhiteSpace;
#endif
            /// <summary>
            /// <see cref="System.String.Substring(int)"/>
            /// </summary>
            public static readonly MethodInfo Substring1;
            /// <summary>
            /// <see cref="System.String.Substring(int, int)"/>
            /// </summary>
            public static readonly MethodInfo Substring2;
            /// <summary>
            /// <see cref="System.String.Contains(string)"/>
            /// </summary>
            public static readonly MethodInfo Contains;
            /// <summary>
            /// <see cref="System.String.Replace(string, string)"/>
            /// </summary>
            public static readonly MethodInfo Replace;
            /// <summary>
            /// <see cref="System.String.Concat(string, string)"/>
            /// </summary>
            public static readonly MethodInfo Concat;
            /// <summary>
            /// <see cref="System.String.Compare(string, string)"/>
            /// </summary>
            public static readonly MethodInfo Compare;
            /// <summary>
            /// <see cref="System.String.StartsWith(string)"/>
            /// </summary>
            public static readonly MethodInfo StartsWith;
            /// <summary>
            /// <see cref="System.String.EndsWith(string)"/>
            /// </summary>
            public static readonly MethodInfo EndsWith;
            /// <summary>
            /// <see cref="System.String.Trim()"/>
            /// </summary>
            public static readonly MethodInfo Trim;
            /// <summary>
            /// <see cref="System.String.TrimStart(char[])"/>
            /// </summary>
            public static readonly MethodInfo TrimStart;
            /// <summary>
            /// <see cref="System.String.TrimEnd(char[])"/>
            /// </summary>
            public static readonly MethodInfo TrimEnd;
            /// <summary>
            /// <see cref="System.String.ToUpper()"/>
            /// </summary>
            public static readonly MethodInfo ToUpper;
            /// <summary>
            /// <see cref="System.String.ToLower()"/>
            /// </summary>
            public static readonly MethodInfo ToLower;
            /// <summary>
            /// <see cref="System.String.Length"/>
            /// </summary>
            public static readonly PropertyInfo Length;
        }
        /// <summary>
        /// <see cref="System.Math"/>支持成员
        /// </summary>
        public static class Math
        {
            static Math()
            {
                foreach (var method in typeof(System.Math).GetMethods())
                {
                    var parameters = method.GetParameters();
                    switch (method.Name)
                    {
                        case nameof(System.Math.Abs): SetArrayValue(Abs, method); break;
                        case nameof(System.Math.Ceiling): SetArrayValue(Ceiling, method); break;
                        case nameof(System.Math.Floor): SetArrayValue(Floor, method); break;
                        case nameof(System.Math.Pow): Pow = method; break;
                        case nameof(System.Math.Sign): SetArrayValue(Sign, method); break;
                        case nameof(System.Math.Exp): Exp = method; break;
                        case nameof(System.Math.Log): SetArrayValue(Log, method); break;
                        case nameof(System.Math.Log10): Log10 = method; break;
                        case nameof(System.Math.Sin): Sin = method; break;
                        case nameof(System.Math.Cos): Cos = method; break;
                        case nameof(System.Math.Tan): Tan = method; break;
                        case nameof(System.Math.Sqrt): Sqrt = method; break;
                        case nameof(System.Math.Asin): Asin = method; break;
                        case nameof(System.Math.Acos): Acos = method; break;
                        case nameof(System.Math.Atan): Atan = method; break;
                    }
                }
            }
            /// <summary>
            /// <see cref="System.Math.Abs(int)"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Abs = new MethodInfo[7];
            /// <summary>
            /// <see cref="System.Math.Ceiling(double)"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Ceiling = new MethodInfo[2];
            /// <summary>
            /// <see cref="System.Math.Floor(double)"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Floor = new MethodInfo[2];
            /// <summary>
            /// <see cref="System.Math.Pow(double, double)"/>
            /// </summary>
            public static readonly MethodInfo Pow;
            /// <summary>
            /// <see cref="System.Math.Sign(int)"/>函数集合
            /// </summary>
            public static readonly MethodInfo[] Sign = new MethodInfo[7];
            /// <summary>
            /// <see cref="System.Math.Exp(double)"/>
            /// </summary>
            public static readonly MethodInfo Exp;
            /// <summary>
            /// <see cref="System.Math.Log(double)"/>
            /// </summary>
            public static readonly MethodInfo[] Log = new MethodInfo[2];
            /// <summary>
            /// <see cref="System.Math.Log10(double)"/>
            /// </summary>
            public static readonly MethodInfo Log10;
            /// <summary>
            /// <see cref="System.Math.Sin(double)"/>
            /// </summary>
            public static readonly MethodInfo Sin;
            /// <summary>
            /// <see cref="System.Math.Cos(double)"/>
            /// </summary>
            public static readonly MethodInfo Cos;
            /// <summary>
            /// <see cref="System.Math.Tan(double)"/>
            /// </summary>
            public static readonly MethodInfo Tan;
            /// <summary>
            /// <see cref="System.Math.Sqrt(double)"/>
            /// </summary>
            public static readonly MethodInfo Sqrt;
            /// <summary>
            /// <see cref="System.Math.Asin(double)"/>
            /// </summary>
            public static readonly MethodInfo Asin;
            /// <summary>
            /// <see cref="System.Math.Acos(double)"/>
            /// </summary>
            public static readonly MethodInfo Acos;
            /// <summary>
            /// <see cref="System.Math.Atan(double)"/>
            /// </summary>
            public static readonly MethodInfo Atan;
        }
        /// <summary>
        /// <see cref="System.DateTime"/>支持成员
        /// </summary>
        public static class DateTime
        {
            static DateTime()
            {
                foreach (var method in typeof(System.DateTime).GetMethods())
                {
                    switch (method.Name)
                    {
                        case nameof(System.DateTime.AddDays): AddDays = method; break;
                        case nameof(System.DateTime.AddHours): AddHours = method; break;
                        case nameof(System.DateTime.AddMilliseconds): AddMilliseconds = method; break;
                        case nameof(System.DateTime.AddMinutes): AddMinutes = method; break;
                        case nameof(System.DateTime.AddMonths): AddMonths = method; break;
                        case nameof(System.DateTime.AddSeconds): AddSeconds = method; break;
                        case nameof(System.DateTime.AddYears): AddYears = method; break;
                    }
                }

                foreach (var property in typeof(System.DateTime).GetProperties())
                {
                    switch (property.Name)
                    {
                        case nameof(System.DateTime.Now): Now = property; break;
                        case nameof(System.DateTime.UtcNow): UtcNow = property; break;
                        case nameof(System.DateTime.Date): Date = property; break;

                        case nameof(System.DateTime.Year): Year = property; break;
                        case nameof(System.DateTime.Month): Month = property; break;
                        case nameof(System.DateTime.Day): Day = property; break;
                        case nameof(System.DateTime.Hour): Hour = property; break;
                        case nameof(System.DateTime.Minute): Minute = property; break;
                        case nameof(System.DateTime.Second): Second = property; break;
                        case nameof(System.DateTime.Millisecond): Millisecond = property; break;

                        case nameof(System.DateTime.DayOfYear): DayOfYear = property; break;
                        case nameof(System.DateTime.DayOfWeek): DayOfWeek = property; break;
                        case nameof(System.DateTime.TimeOfDay): TimeOfDay = property; break;
                    }
                }
            }
            /// <summary>
            /// <see cref="System.DateTime.AddDays(double)"/>
            /// </summary>
            public static readonly MethodInfo AddDays;
            /// <summary>
            /// <see cref="System.DateTime.AddHours(double)"/>
            /// </summary>
            public static readonly MethodInfo AddHours;
            /// <summary>
            /// <see cref="System.DateTime.AddMinutes(double)"/>
            /// </summary>
            public static readonly MethodInfo AddMilliseconds;
            /// <summary>
            /// <see cref="System.DateTime.AddMinutes(double)"/>
            /// </summary>
            public static readonly MethodInfo AddMinutes;
            /// <summary>
            /// <see cref="System.DateTime.AddMonths(int)"/>
            /// </summary>
            public static readonly MethodInfo AddMonths;
            /// <summary>
            /// <see cref="System.DateTime.AddSeconds(double)"/>
            /// </summary>
            public static readonly MethodInfo AddSeconds;
            /// <summary>
            /// <see cref="System.DateTime.AddYears(int)"/>
            /// </summary>
            public static readonly MethodInfo AddYears;
            /// <summary>
            /// <see cref="System.DateTime.Now"/>
            /// </summary>
            public static readonly PropertyInfo Now;
            /// <summary>
            /// <see cref="System.DateTime.UtcNow"/>
            /// </summary>
            public static readonly PropertyInfo UtcNow;
            /// <summary>
            /// <see cref="System.DateTime.Date"/>
            /// </summary>
            public static readonly PropertyInfo Date;
            /// <summary>
            /// <see cref="System.DateTime.Year"/>
            /// </summary>
            public static readonly PropertyInfo Year;
            /// <summary>
            /// <see cref="System.DateTime.Month"/>
            /// </summary>
            public static readonly PropertyInfo Month;
            /// <summary>
            /// <see cref="System.DateTime.Day"/>
            /// </summary>
            public static readonly PropertyInfo Day;
            /// <summary>
            /// <see cref="System.DateTime.Hour"/>
            /// </summary>
            public static readonly PropertyInfo Hour;
            /// <summary>
            /// <see cref="System.DateTime.Minute"/>
            /// </summary>
            public static readonly PropertyInfo Minute;
            /// <summary>
            /// <see cref="System.DateTime.Second"/>
            /// </summary>
            public static readonly PropertyInfo Second;
            /// <summary>
            /// <see cref="System.DateTime.Millisecond"/>
            /// </summary>
            public static readonly PropertyInfo Millisecond;
            /// <summary>
            /// <see cref="System.DateTime.DayOfYear"/>
            /// </summary>
            public static readonly PropertyInfo DayOfYear;
            /// <summary>
            /// <see cref="System.DateTime.DayOfWeek"/>
            /// </summary>
            public static readonly PropertyInfo DayOfWeek;
            /// <summary>
            /// <see cref="System.DateTime.TimeOfDay"/>
            /// </summary>
            public static readonly PropertyInfo TimeOfDay;
        }
        /// <summary>
        /// <see cref="System.Guid"/>支持成员
        /// </summary>
        public static class Guid
        {
            static Guid()
            {
                NewGuid = typeof(System.Guid).GetMethod(nameof(System.Guid.NewGuid));
            }
            /// <summary>
            /// <see cref="System.Guid.NewGuid"/>
            /// </summary>
            public static readonly MethodInfo NewGuid;
        }
        /// <summary>
        /// <see cref="System.Data.Common.DbDataReader"/>支持成员
        /// </summary>
        public static class DataReader
        {
            static DataReader()
            {
                var type = typeof(DbDataReader);
                foreach (var method in type.GetMethods())
                {
                    switch (method.Name)
                    {
                        case nameof(GetBoolean): GetBoolean = method; break;
                        case nameof(GetByte): GetByte = method; break;
                        case nameof(GetBytes): GetBytes = method; break;
                        case nameof(GetChar): GetChar = method; break;
                        case nameof(GetChars): GetChars = method; break;
                        case nameof(GetDateTime): GetDateTime = method; break;
                        case nameof(GetDecimal): GetDecimal = method; break;
                        case nameof(GetDouble): GetDouble = method; break;
                        case nameof(GetFloat): GetFloat = method; break;
                        case nameof(GetGuid): GetGuid = method; break;
                        case nameof(GetInt16): GetInt16 = method; break;
                        case nameof(GetInt32): GetInt32 = method; break;
                        case nameof(GetInt64): GetInt64 = method; break;
                        case nameof(GetString): GetString = method; break;
                        case nameof(IsDBNull): IsDBNull = method; break;
                        case nameof(GetValue): GetValue = method; break;
                    }
                }
            }
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetBoolean(int)"/>
            /// </summary>
            public static readonly MethodInfo GetBoolean;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetByte(int)"/>
            /// </summary>
            public static readonly MethodInfo GetByte;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetBytes(int, long, byte[], int, int)"/>
            /// </summary>
            public static readonly MethodInfo GetBytes;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetChar(int)"/>
            /// </summary>
            public static readonly MethodInfo GetChar;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetChars(int, long, char[], int, int)"/>
            /// </summary>
            public static readonly MethodInfo GetChars;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetDateTime(int)"/>
            /// </summary>
            public static readonly MethodInfo GetDateTime;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetDecimal(int)"/>
            /// </summary>
            public static readonly MethodInfo GetDecimal;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetDouble(int)"/>
            /// </summary>
            public static readonly MethodInfo GetDouble;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetFloat(int)"/>
            /// </summary>
            public static readonly MethodInfo GetFloat;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetGuid(int)"/>
            /// </summary>
            public static readonly MethodInfo GetGuid;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetInt16(int)"/>
            /// </summary>
            public static readonly MethodInfo GetInt16;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetInt32(int)"/>
            /// </summary>
            public static readonly MethodInfo GetInt32;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetInt64(int)"/>
            /// </summary>
            public static readonly MethodInfo GetInt64;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetString(int)"/>
            /// </summary>
            public static readonly MethodInfo GetString;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.GetValue(int)"/>
            /// </summary>
            public static readonly MethodInfo GetValue;
            /// <summary>
            /// <see cref="System.Data.Common.DbDataReader.IsDBNull(int)"/>
            /// </summary>
            public static readonly MethodInfo IsDBNull;
        }
        /// <summary>
        /// 系统使用函数
        /// </summary>
        public static class DbFunctions
        {
            static DbFunctions()
            {
                var type = typeof(Caredev.Mego.DbFunctions);
                GetIdentity = type.GetMethod(nameof(GetIdentity));
            }
            /// <summary>
            /// 标识列插入后获取自增列值。
            /// </summary>
            public static readonly MethodInfo GetIdentity;
        }
        /// <summary>
        /// 系统使用成员
        /// </summary>
        internal static class DbMembers
        {
            static DbMembers()
            {
                CustomRowIndex = typeof(DbCustomMembers).GetProperty(nameof(CustomRowIndex));
            }
            /// <summary>
            /// 自定义行索引
            /// </summary>
            public static readonly PropertyInfo CustomRowIndex;
        }
    }
}
