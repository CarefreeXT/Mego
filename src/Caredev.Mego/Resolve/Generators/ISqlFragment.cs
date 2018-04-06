// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using System.Collections.Generic;
    using System.Reflection;
    /// <summary>
    /// SQL语句片段对象。
    /// </summary>
    public interface ISqlFragment
    {
        /// <summary>
        /// 语句生成上下文。
        /// </summary>
        GenerateContext Context { get; }
        /// <summary>
        /// 有结束符。
        /// </summary>
        bool HasTerminator { get; }
        /// <summary>
        /// 写入SQL语句。
        /// </summary>
        /// <param name="writer"></param>
        void WriteSql(SqlWriter writer);
    }
    /// <summary>
    /// 表达式片段对象。
    /// </summary>
    public interface IExpressionFragment : ISqlFragment
    {
    }
    /// <summary>
    /// 名称片段对象。
    /// </summary>
    public interface INameFragment : ISqlFragment
    {
        /// <summary>
        /// 名称。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 名称种类。
        /// </summary>
        EDbNameKind NameKind { get; }
    }
    /// <summary>
    /// 名称加架构名片段对象。
    /// </summary>
    public interface INameSchemaFragment : INameFragment
    {
        /// <summary>
        /// 架构名。
        /// </summary>
        string Schema { get; }
    }
    /// <summary>
    /// 逻辑操作表达式片段。
    /// </summary>
    public interface ILogicFragment : IExpressionFragment
    {
    }
    /// <summary>
    /// 创建语句对象。
    /// </summary>
    public interface ICreateFragment : ISqlFragment
    {
    }
    /// <summary>
    /// 数据成员片段对象。
    /// </summary>
    public interface IMemberFragment : IExpressionFragment
    {
        /// <summary>
        /// 成员所有者。
        /// </summary>
        ISourceFragment Owner { get; }
        /// <summary>
        /// 成员对应的CLR属性。
        /// </summary>
        MemberInfo Property { get; }
        /// <summary>
        /// 输出名称。
        /// </summary>
        string OutputName { get; }
        /// <summary>
        /// 别名。
        /// </summary>
        string AliasName { get; }
    }
    /// <summary>
    /// 数据源对象。
    /// </summary>
    public interface ISourceFragment : ISqlFragment
    {
        /// <summary>
        /// 父级对象。
        /// </summary>
        ISourceFragment Parent { get; set; }
        /// <summary>
        /// 数据源成员集合。
        /// </summary>
        List<IMemberFragment> Members { get; }
        /// <summary>
        /// 当前数据源别名。
        /// </summary>
        string AliasName { get; }
        /// <summary>
        /// 当前数据源的连接操作。
        /// </summary>
        EJoinType? Join { get; set; }
        /// <summary>
        /// 当前数据源的连接条件。
        /// </summary>
        IExpressionFragment Condition { get; set; }
        /// <summary>
        /// 根据指定的<see cref="MemberInfo"/>获取成员。
        /// </summary>
        /// <param name="member">指定的成员信息。</param>
        /// <returns>查找结果。</returns>
        IMemberFragment GetMember(MemberInfo member);
        /// <summary>
        /// 根据指定的<see cref="ColumnMetadata"/>获取成员。
        /// </summary>
        /// <param name="metadata">指定的成员信息。</param>
        /// <returns>查找结果。</returns>
        IMemberFragment GetMember(ColumnMetadata metadata);
    }
}
