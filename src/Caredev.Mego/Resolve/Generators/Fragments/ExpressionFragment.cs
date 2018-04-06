// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Fragments
{
    using Caredev.Mego.Resolve.Expressions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Res = Properties.Resources;
    /// <summary>
    /// 排序语句片段。
    /// </summary>
    public class SortFragment : SqlFragment, IExpressionFragment
    {
        /// <summary>
        /// 初始化排序。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">成员表达式。</param>
        /// <param name="kind">排序种类。</param>
        public SortFragment(GenerateContext context, IExpressionFragment content, EOrderKind kind = EOrderKind.Ascending)
            : base(context)
        {
            Content = content;
            Kind = kind;
        }
        /// <summary>
        /// 成员表达式。
        /// </summary>
        public IExpressionFragment Content { get; }
        /// <summary>
        /// 排序种类。
        /// </summary>
        public EOrderKind Kind { get; }
    }
    /// <summary>
    /// 二元运算语句片段。
    /// </summary>
    public class BinaryFragment : SqlFragment, ILogicFragment
    {
        /// <summary>
        /// 初始化二元运算。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="kind">二元运算种类。</param>
        public BinaryFragment(GenerateContext context, EBinaryKind kind)
            : base(context)
        {
            Kind = kind;
        }
        /// <summary>
        /// 二元运算种类。
        /// </summary>
        public EBinaryKind Kind { get; }
        /// <summary>
        /// 左表达式。
        /// </summary>
        public IExpressionFragment Left { get; set; }
        /// <summary>
        /// 右表达式。
        /// </summary>
        public IExpressionFragment Right { get; set; }
    }
    /// <summary>
    /// 一元运算语句片段。
    /// </summary>
    public class UnaryFragment : SqlFragment, ILogicFragment
    {
        /// <summary>
        /// 初始化一元运算。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="kind">运算种类。</param>
        /// <param name="type">输出CLR类型。</param>
        public UnaryFragment(GenerateContext context, EUnaryKind kind, Type type)
            : base(context)
        {
            Kind = kind;
            Type = type;
        }
        /// <summary>
        /// 运算种类。
        /// </summary>
        public EUnaryKind Kind { get; }
        /// <summary>
        /// 输出CLR类型。
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// 表达式。
        /// </summary>
        public IExpressionFragment Expresssion { get; set; }
    }
    /// <summary>
    /// 二元逻辑运算语句片段。
    /// </summary>
    public class BinaryLogicalFragment : SqlFragment, ILogicFragment
    {
        /// <summary>
        /// 初始化二元逻辑运算。
        /// </summary>
        /// <param name="context">生成上下文。生成上下文。</param>
        /// <param name="kind">逻辑运算种类。</param>
        public BinaryLogicalFragment(GenerateContext context, EBinaryKind kind)
            : base(context)
        {
            if (kind == EBinaryKind.AndAlso || kind == EBinaryKind.OrElse)
            {
                Kind = kind;
            }
            else
            {
                throw new ArgumentException(Res.ExceptionBinaryLogicalKind, nameof(kind));
            }
        }
        /// <summary>
        /// 逻辑运算种类，只允许<see cref="EBinaryKind.AndAlso"/>和<see cref="EBinaryKind.OrElse"/>。
        /// </summary>
        public EBinaryKind Kind { get; }
        /// <summary>
        /// 子表达式集合。
        /// </summary>
        public List<IExpressionFragment> Expressions { get; } = new List<IExpressionFragment>();
    }
    /// <summary>
    /// 标量函数调用语句片段。
    /// </summary>
    public class ScalarFragment : SqlFragment, ILogicFragment
    {
        /// <summary>
        /// 初始化标量函数。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="arguments">参数表达式集合。</param>
        public ScalarFragment(GenerateContext context, params IExpressionFragment[] arguments)
            : base(context)
        {
            Arguments = new List<IExpressionFragment>(arguments);
        }
        /// <summary>
        /// 调用函数的CLR描述对象。
        /// </summary>
        public MemberInfo Function { get; set; }
        /// <summary>
        /// 参数表达式集合。
        /// </summary>
        public List<IExpressionFragment> Arguments { get; }
    }
    /// <summary>
    /// 常量语句片段。
    /// </summary>
    public class ConstantFragment : SqlFragment, IExpressionFragment
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="value">常量值。</param>
        public ConstantFragment(GenerateContext context, object value)
            : base(context)
        {
            Value = value;
        }
        /// <summary>
        /// 常量值。
        /// </summary>
        public object Value { get; }
    }
    /// <summary>
    /// 常量列表语句片段。
    /// </summary>
    public class ConstantListFragment : SqlFragment, IExpressionFragment
    {
        /// <summary>
        /// 初始化常量列表。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="value">列表值。</param>
        public ConstantListFragment(GenerateContext context, IEnumerable value)
            : base(context)
        {
            Value = value;
        }
        /// <summary>
        /// 列表值。
        /// </summary>
        public IEnumerable Value { get; }
    }
    /// <summary>
    /// 值列表语句片段。
    /// </summary>
    public class ValueListFragment : SqlFragment, IExpressionFragment
    {
        /// <summary>
        /// 初始化值列表。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="member">提交成员语句。</param>
        /// <param name="value">值枚举。</param>
        public ValueListFragment(GenerateContext context, CommitMemberFragment member, IEnumerable value)
             : base(context)
        {
            Member = member;
            Value = value;
        }
        /// <summary>
        /// 提交成员语句。
        /// </summary>
        public CommitMemberFragment Member { get; }
        /// <summary>
        /// 值枚举。
        /// </summary>
        public IEnumerable Value { get; }
    }
    /// <summary>
    /// 默认值语句片段。
    /// </summary>
    public class DefaultFragment : SqlFragment, IExpressionFragment
    {
        /// <summary>
        /// 初始化默认值。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="type">值类型。</param>
        public DefaultFragment(GenerateContext context, Type type)
            : base(context)
        {

        }
        /// <summary>
        /// 值类型。
        /// </summary>
        public Type ClrType { get; }
    }
    /// <summary>
    /// 行索引语句片段。
    /// </summary>
    public class RowIndexFragment : SqlFragment, IExpressionFragment
    {
        /// <summary>
        /// 初始化行索引。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        public RowIndexFragment(GenerateContext context) : base(context)
        {
        }
        /// <summary>
        /// 索引值。
        /// </summary>
        public int Index { get; set; } = 1;
    }
}
