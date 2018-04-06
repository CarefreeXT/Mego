// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    /// <summary>
    /// 语句片段扩展方法
    /// </summary>
    public static class SqlFragmentExtensions
    {
        /// <summary>
        /// 创建成员片段。
        /// </summary>
        /// <param name="current">当前数据源片段。</param>
        /// <param name="property">成员CLR属性。</param>
        /// <param name="parameter">创建参数。</param>
        /// <returns>新建成员片段。</returns>
        public static IMemberFragment CreateMember(this ISourceFragment current, MemberInfo property, object parameter)
        {
            var context = current.Context;
            var generator = context.Generator;
            var newMember = generator.CreateMember(context, current, property, parameter);
            current.Members.Add(newMember);
            return newMember;
        }
        /// <summary>
        /// 创建表达式片段。
        /// </summary>
        /// <param name="current">当前数据源片段。</param>
        /// <param name="expression">表达式。</param>
        /// <returns>新建表达式片段。</returns>
        public static IExpressionFragment CreateExpression(this ISourceFragment current, DbExpression expression)
        {
            var context = current.Context;
            return context.Generator.CreateExpression(context, expression, current);
        }
        /// <summary>
        /// 根据成员表达式检索成员片段。
        /// </summary>
        /// <param name="current">当前数据源片段。</param>
        /// <param name="expression">成员表达式对象。</param>
        /// <param name="property">成员CLR属性。</param>
        /// <param name="onlyRetrieal">仅检索。</param>
        /// <returns>获取到的片段对象。</returns>
        public static IMemberFragment RetrievalMember(this ISourceFragment current, DbExpression expression, MemberInfo property = null, bool onlyRetrieal = true)
        {
            var context = current.Context;
            return context.Generator.RetrievalMember(context, current, expression, property, onlyRetrieal);
        }
        /// <summary>
        /// 根据成员表达式检索多个成员片段。
        /// </summary>
        /// <param name="current">当前数据源片段。</param>
        /// <param name="expression">成员表达式对象</param>
        /// <param name="onlyRetrieal">仅检索。</param>
        /// <returns>获取到的片段对象枚举。</returns>
        public static IEnumerable<IMemberFragment> RetrievalMembers(this ISourceFragment current, DbExpression expression, bool onlyRetrieal = true)
        {
            var context = current.Context;
            return context.Generator.RetrievalMembers(context, current, expression, onlyRetrieal);
        }
#if NET35
        /// <summary>
        /// 合并多个<see cref="BinaryFragment"/>片段为一个逻辑判断语句片段。
        /// </summary>
        /// <param name="fragments">合并的语句片段集合。</param>
        /// <param name="kind">逻辑操作符种类。</param>
        /// <returns>合并结果。</returns>
        public static ILogicFragment Merge(this IEnumerable<BinaryFragment> fragments, EBinaryKind kind = EBinaryKind.AndAlso) => Merge(fragments, kind);
#endif
        /// <summary>
        /// 合并多个<see cref="IExpressionFragment"/>片段为一个逻辑判断语句片段。
        /// </summary>
        /// <param name="fragments">合并的语句片段集合。</param>
        /// <param name="kind">逻辑操作符种类。</param>
        /// <returns>合并结果。</returns>
        public static ILogicFragment Merge(this IEnumerable<IExpressionFragment> fragments, EBinaryKind kind = EBinaryKind.AndAlso)
        {
            var array = fragments.ToArray();
            if (array.Length == 1) return array[0] as ILogicFragment;
            var other = new IExpressionFragment[array.Length - 1];
            Array.Copy(array, 1, other, 0, other.Length);
            return Merge(array[0], kind, other);
        }
        /// <summary>
        /// 向指定的表达式语句片段合并多个<see cref="IExpressionFragment"/>片段。
        /// </summary>
        /// <param name="target">目标语句片段</param>
        /// <param name="kind">逻辑操作符种类。</param>
        /// <param name="fragments">合并的语句片段集合。</param>
        /// <returns>合并结果。</returns>
        public static ILogicFragment Merge(this IExpressionFragment target, EBinaryKind kind, IEnumerable<IExpressionFragment> fragments)
        {
            var current = target as BinaryLogicalFragment;
            if (!fragments.Any())
            {
                throw new ArgumentException(Res.ExceptionMergeFragmentIsNotEmpty, nameof(fragments));
            }
            foreach (var fragment in fragments)
            {
                if (current == null)
                {
                    if (fragment is BinaryLogicalFragment binary && binary.Kind == kind)
                    {
                        current = binary;
                        current.Expressions.Insert(0, target);
                    }
                    else
                    {
                        current = new BinaryLogicalFragment(target.Context, kind);
                        current.Expressions.Add(target);
                        current.Expressions.Add(fragment);
                    }
                }
                else
                {
                    if (fragment is BinaryLogicalFragment logical && logical.Kind == current.Kind)
                    {
                        current.Expressions.AddRange(logical.Expressions);
                    }
                    else
                    {
                        current.Expressions.Add(fragment);
                    }
                }
            }
            return current;
        }
        /// <summary>
        /// 向指定的表达式语句片段合并多个<see cref="IExpressionFragment"/>片段。
        /// </summary>
        /// <param name="target">目标语句片段</param>
        /// <param name="kind">逻辑操作符种类。</param>
        /// <param name="fragments">合并的语句片段集合。</param>
        /// <returns>合并结果。</returns>
        public static ILogicFragment Merge(this IExpressionFragment target, EBinaryKind kind, params IExpressionFragment[] fragments)
            => Merge(target, kind, (IEnumerable<IExpressionFragment>)fragments);
        /// <summary>
        /// 向指定的表达式语句片段使用逻辑且合并多个<see cref="IExpressionFragment"/>片段。
        /// </summary>
        /// <param name="target">目标语句片段</param>
        /// <param name="fragments">合并的语句片段集合。</param>
        /// <returns>合并结果。</returns>
        public static ILogicFragment Merge(this IExpressionFragment target, params IExpressionFragment[] fragments)
            => Merge(target, EBinaryKind.AndAlso, (IEnumerable<IExpressionFragment>)fragments);
        /// <summary>
        /// 向指定的表达式语句片段使用逻辑且合并多个<see cref="IExpressionFragment"/>片段。
        /// </summary>
        /// <param name="target">目标语句片段。</param>
        /// <param name="fragments">合并的语句片段集合。</param>
        /// <returns>合并结果。</returns>
        public static ILogicFragment Merge(this IExpressionFragment target, IEnumerable<IExpressionFragment> fragments)
            => Merge(target, EBinaryKind.AndAlso, fragments);
        /// <summary>
        /// 按主键连接两个数据源片段。
        /// </summary>
        /// <param name="source">源语句片段。</param>
        /// <param name="target">目标语句片段。</param>
        /// <param name="keys">连接主键。</param>
        /// <param name="type">连接类型。</param>
        public static void Join(this ISourceFragment source, ISourceFragment target, IEnumerable<ColumnMetadata> keys, EJoinType type = EJoinType.InnerJoin)
        {
            target.Join = EJoinType.InnerJoin;
            target.Condition = JoinCondition(source, target, keys);
        }
        /// <summary>
        /// 按主键生成连接两个数据源片段的条件。
        /// </summary>
        /// <param name="source">源语句片段。</param>
        /// <param name="target">目标语句片段。</param>
        /// <param name="keys">连接主键。</param>
        /// <returns>条件逻辑语句片段。</returns>
        public static ILogicFragment JoinCondition(this ISourceFragment source, ISourceFragment target, IEnumerable<ColumnMetadata> keys)
        {
            return keys.Select(key => new BinaryFragment(source.Context, EBinaryKind.Equal)
            {
                Left = source.GetMember(key),
                Right = target.GetMember(key)
            }).Merge();
        }
    }
}