// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Translators;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    /// <summary>
    /// 集合成员表达式。
    /// </summary>
    public class DbCollectionMemberExpression : DbUnitTypeExpression, IDbDefaultUnitType, IDbExpandUnitExpression, IDbMemberExpression
    {
        /// <summary>
        /// 创建集合成员表达式。
        /// </summary>
        /// <param name="context">翻译上下文。</param>
        /// <param name="member">成员CLR对象。</param>
        /// <param name="source">源表达式对象。</param>
        public DbCollectionMemberExpression(TranslationContext context, PropertyInfo member, DbExpression source)
            : base(member.PropertyType, new DbDataItemExpression(member.PropertyType.ElementType()))
        {
            Member = member;
            Expression = source;
            switch (source)
            {
                case DbDataItemExpression dataitem:
                    if (dataitem.Unit.ExpressionType == EExpressionType.DataSet)
                    {
                        var table = context.Context.Configuration.Metadata.Table((DbDataSetExpression)dataitem.Unit);
                        InitialNavigate(table.Navigates[member], source);
                    }
                    else if (dataitem.Unit.ExpressionType == EExpressionType.CollectionMember)
                    {
                        var table = context.Context.Configuration.Metadata.Table(dataitem.ClrType);
                        InitialNavigate(table.Navigates[member], source);
                    }
                    break;
                case DbObjectMemberExpression objectMember:
                    InitialNavigate(objectMember.Metadata.Target.Navigates[member], source);
                    break;
            }
        }
        /// <summary>
        /// 成员CLR对象。
        /// </summary>
        public MemberInfo Member { get; private set; }
        /// <summary>
        /// 源表达式对象。
        /// </summary>
        public DbExpression Expression { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.CollectionMember;
        /// <summary>
        /// 连接操作中的默认值表达式。
        /// </summary>
        public DbExpression Default { get; set; }
        /// <summary>
        /// 在复合关系下的中间数据集表达式。
        /// </summary>
        public DbDataSetExpression RelationSet { get; private set; }
        /// <summary>
        /// 成员数据集表达式。
        /// </summary>
        public DbDataSetExpression TargetSet { get; private set; }
        /// <summary>
        /// 数据集关系表达式集合。
        /// </summary>
        public DbJoinKeyPairExpression[] Pairs { get; private set; }
        /// <summary>
        /// 复合关系表达式集合。
        /// </summary>
        public DbJoinKeyPairExpression[] CompositePairs { get; private set; }
        /// <summary>
        /// 当前对象成员的导航元数据。
        /// </summary>
        public NavigateMetadata Metadata { get; private set; }
        /// <summary>
        /// 当前数据集展开的成员表达式集合。
        /// </summary>
        public IList<DbExpression> ExpandItems { get; } = new List<DbExpression>();
        /// <summary>
        /// 初始化导航元数据及关系信息。
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="parent"></param>
        private void InitialNavigate(NavigateMetadata metadata, DbExpression parent)
        {
            Metadata = metadata;
            TargetSet = new DbDataSetExpression(typeof(IEnumerable<>).MakeGenericType(metadata.Target.ClrType));
            var comNavi = metadata as CompositeNavigateMetadata;
            if (comNavi != null)
            {
                RelationSet = new DbDataSetExpression(typeof(IEnumerable<>).MakeGenericType(comNavi.RelationTable.ClrType));
                Pairs = comNavi.Pairs.Select(a =>
                {
                    var left = new DbMemberExpression(a.ForeignKey.Member, RelationSet);
                    var right = new DbMemberExpression(a.PrincipalKey.Member, parent);
                    return new DbJoinKeyPairExpression(left, right);
                }).ToArray();
                CompositePairs = comNavi.CompositePairs.Select(a =>
                {
                    var left = new DbMemberExpression(a.PrincipalKey.Member, TargetSet);
                    var right = new DbMemberExpression(a.ForeignKey.Member, RelationSet);
                    return new DbJoinKeyPairExpression(left, right);
                }).ToArray();
            }
            else
            {
                Pairs = metadata.Pairs.Select(a =>
                {
                    var left = new DbMemberExpression(a.ForeignKey.Member, TargetSet);
                    var right = new DbMemberExpression(a.PrincipalKey.Member, parent);
                    return new DbJoinKeyPairExpression(left, right);
                }).ToArray();
            }
        }
    }
}