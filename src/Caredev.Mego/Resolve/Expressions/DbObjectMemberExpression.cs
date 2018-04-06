// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Translators;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    /// <summary>
    /// 对象成员表达式 。
    /// </summary>
    public class DbObjectMemberExpression : DbMemberExpression
    {
        /// <summary>
        /// 创建对象成员表达式。
        /// </summary>
        /// <param name="context">翻译上下文。</param>
        /// <param name="member">成员CLR对象。</param>
        /// <param name="source">源表达式对象。</param>
        public DbObjectMemberExpression(TranslationContext context, MemberInfo member, DbExpression source)
            : base(member, source)
        {
            switch (source)
            {
                case DbDataItemExpression dataitem:
                    var metadata = context.Context.Configuration.Metadata;
                    switch (dataitem.Unit.ExpressionType)
                    {
                        case EExpressionType.DataSet:
                            var ds = (DbDataSetExpression)dataitem.Unit;
                            var dstable = metadata.Table(ds);
                            InitialNavigate(dstable.Navigates[member], source);
                            break;
                        case EExpressionType.CollectionMember:
                            var collection = (DbCollectionMemberExpression)dataitem.Unit;
                            var coltable = metadata.Table(collection.TargetSet);
                            InitialNavigate(coltable.Navigates[member], source);
                            break;
                        default:
                            throw new NotSupportedException(string.Format(Res.NotSupportedUnitExpressionToObjectMember, dataitem.Unit.ExpressionType));
                    }
                    break;
                case DbObjectMemberExpression objectMember:
                    InitialNavigate(objectMember.Metadata.Target.Navigates[member], source);
                    break;
            }
        }
        /// <summary>
        /// 当前成员是否可以为空。
        /// </summary>
        public bool Nullable { get; private set; }
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
        /// 初始化导航元数据及关系信息。
        /// </summary>
        /// <param name="metadata">导航元数据。</param>
        /// <param name="parent">父级表达式。</param>
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
                    var left = new DbMemberExpression(a.ForeignKey.Member, parent);
                    var right = new DbMemberExpression(a.PrincipalKey.Member, TargetSet);
                    return new DbJoinKeyPairExpression(left, right);
                }).ToArray();
            }
            Nullable = Metadata.Pairs.Select(a => a.ForeignKey.Member)
               .OfType<PropertyInfo>().All(a => a.PropertyType.IsNullable());
        }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.ObjectMember;
    }
}