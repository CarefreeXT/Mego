// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Fragments
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    /// <summary>
    /// 成员语句片段基类。
    /// </summary>
    public abstract class MemberFragment : SqlFragment, IMemberFragment
    {
        /// <summary>
        /// 创建成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="owner">成员所有者。</param>
        /// <param name="property">成员对应的CLR属性。</param>
        public MemberFragment(GenerateContext context, ISourceFragment owner, MemberInfo property)
            : base(context)
        {
            Owner = owner;
            Property = property;
        }
        /// <summary>
        /// 输出名称。
        /// </summary>
        public abstract string OutputName { get; }
        /// <summary>
        /// 别名。
        /// </summary>
        public string AliasName { get; set; }
        /// <summary>
        /// 成员所有者。
        /// </summary>
        public ISourceFragment Owner { get; }
        /// <summary>
        /// 成员对应的CLR类型。
        /// </summary>
        public MemberInfo Property { get; }
        /// <summary>
        /// 生成所者成员集合中唯一的成员名称。
        /// </summary>
        public void MakeUniqueName()
        {
            MakeUniqueName(Owner.Members.Where(a => a != this).Select(a => a.OutputName), OutputName);
        }
        /// <summary>
        /// 生成所者成员集合中唯一的成员名称。
        /// </summary>
        /// <param name="names">检查名称集合。</param>
        /// <param name="name">期望名称</param>
        public void MakeUniqueName(IEnumerable<string> names, string name)
        {
            if (names.Contains(name))
            {
                AliasName = names.Unique(name);
            }
        }
    }
    /// <summary>
    /// 数据列语句片段。
    /// </summary>
    public class ColumnFragment : MemberFragment
    {
        /// <summary>
        /// 创建数据列。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">成员所有者。</param>
        /// <param name="metadata">列元数据。</param>
        public ColumnFragment(GenerateContext context, ISourceFragment source, ColumnMetadata metadata)
            : base(context, source, metadata.Member)
        {
            Metadata = metadata;
        }
        /// <summary>
        /// 列元数据。
        /// </summary>
        public ColumnMetadata Metadata { get; }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName => Metadata.Name;
        /// <inheritdoc/>
        public override string OutputName => AliasName ?? Metadata.Name;
    }
    /// <summary>
    /// 表达式成员语句片段。
    /// </summary>
    public class ExpressionMemberFragment : MemberFragment
    {
        /// <summary>
        /// 创建表达式成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">成员所有者。</param>
        /// <param name="property">成员属性。</param>
        /// <param name="expression">表达式语句。</param>
        public ExpressionMemberFragment(GenerateContext context, ISourceFragment source, MemberInfo property, IExpressionFragment expression)
            : base(context, source, property)
        {
            Expression = expression;
        }
        /// <summary>
        /// 表达式语句。
        /// </summary>
        public IExpressionFragment Expression { get; }
        /// <inheritdoc/>
        public override string OutputName
        {
            get
            {
                if (!string.IsNullOrEmpty(AliasName))
                    return AliasName;
                return Property.Name;
            }
        }
    }
    /// <summary>
    /// 聚合成员语句片段。
    /// </summary>
    public class AggregateFragment : MemberFragment
    {
        /// <summary>
        /// 创建聚合成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="owner">成员所有者。</param>
        /// <param name="property">成员属性。</param>
        /// <param name="expression">聚合表达式语句。</param>
        public AggregateFragment(GenerateContext context, ISourceFragment owner, MemberInfo property, DbAggregateFunctionExpression expression)
            : base(context, owner, property)
        {
            Expression = expression;
        }
        /// <summary>
        /// 聚合表达式语句。
        /// </summary>
        public DbAggregateFunctionExpression Expression { get; }
        /// <summary>
        /// 相应的聚合函数。
        /// </summary>
        public MemberInfo Function => Expression.Function;
        /// <summary>
        /// 函数调用参数。
        /// </summary>
        public List<ISqlFragment> Arguments { get; } = new List<ISqlFragment>();
        /// <inheritdoc/>
        public override string OutputName
        {
            get
            {
                if (!string.IsNullOrEmpty(AliasName))
                    return AliasName;
                return Property.Name;
            }
        }
    }
    /// <summary>
    /// 引用成员语句片段。
    /// </summary>
    public class ReferenceMemberFragment : MemberFragment
    {
        /// <summary>
        /// 初始化引用成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="source">成员所有者。</param>
        /// <param name="property">成员属性。</param>
        /// <param name="parameter">创建参数。</param>
        public ReferenceMemberFragment(GenerateContext context, ISourceFragment source, MemberInfo property, MemberFragment parameter)
            : base(context, source, property)
        {
            Reference = parameter;
        }
        /// <summary>
        /// 引用成员。
        /// </summary>
        public MemberFragment Reference { get; }
        /// <inheritdoc/>
        public override string OutputName
        {
            get
            {
                if (!string.IsNullOrEmpty(AliasName))
                    return AliasName;
                return Reference.OutputName;
            }
        }
    }
    /// <summary>
    /// 提交成员语句片段。
    /// </summary>
    public class CommitMemberFragment : MemberFragment
    {
        /// <summary>
        /// 初始化提交成员。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="loader">属性值加载器。</param>
        /// <param name="owner">成员所有者。</param>
        /// <param name="property">成员属性。</param>
        public CommitMemberFragment(GenerateContext context, IPropertyValueLoader loader, ISourceFragment owner, MemberInfo property)
            : base(context, owner, property)
        {
            Loader = loader;
            Index = loader.IndexOf(property);
        }
        /// <inheritdoc/>
        public override string OutputName => string.Empty;
        /// <summary>
        /// 属性值加载器。
        /// </summary>
        public IPropertyValueLoader Loader { get; }
        /// <summary>
        /// 当前成员在<see cref="Loader"/>中的索引值。
        /// </summary>
        public int Index { get; }
        /// <summary>
        /// 当前成员关联的列元数据。
        /// </summary>
        public ColumnMetadata Metadata { get; set; }
    }
}