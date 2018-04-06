// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Reflection;
    /// <summary>
    /// 分组表达式。
    /// </summary>
    public class DbGroupByExpression : DbUnitTypeExpression
    {
        /// <summary>
        /// 创建分组表达式。
        /// </summary>
        /// <param name="source">源表达式。</param>
        /// <param name="keypro">分组键的CLR描述对象。</param>
        /// <param name="key">分组键表达式。</param>
        public DbGroupByExpression(DbUnitTypeExpression source, PropertyInfo keypro, DbExpression key)
            : this(source, keypro, key, source.Item)
        {
        }
        /// <summary>
        /// 创建分组表达式。
        /// </summary>
        /// <param name="source">源表达式。</param>
        /// <param name="keypro">分组键的CLR描述对象。</param>
        /// <param name="key">分组键表达式。</param>
        /// <param name="item">单元项表达式，用于自定义创建分组项表达式。</param>
        public DbGroupByExpression(DbUnitTypeExpression source, PropertyInfo keypro, DbExpression key, DbUnitItemTypeExpression item)
            : base(source.ClrType, new DbGroupItemExpression(item))
        {
            Source = source;
            KeyProperty = keypro;
            Key = key;
        }
        /// <summary>
        /// 源表达式。
        /// </summary>
        public DbUnitTypeExpression Source { get; }
        /// <summary>
        /// 分组键的CLR描述对象。
        /// </summary>
        public PropertyInfo KeyProperty { get; }
        /// <summary>
        /// 分组键表达式。
        /// </summary>
        public DbExpression Key { get; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.GroupBy;
    }
}