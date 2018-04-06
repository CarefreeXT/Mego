// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    using System.Linq;
    using Res = Properties.Resources;
    /// <summary>
    /// 内连接操作表达式。
    /// </summary>
    public class DbInnerJoinExpression : DbSetOperationExpression
    {
        /// <summary>
        /// 创建内连接操作表达式。
        /// </summary>
        /// <param name="source">源表达式。</param>
        /// <param name="target">目标表达式。</param>
        /// <param name="left">左端匹配键表达式。</param>
        /// <param name="right">左端匹配键表达式。</param>
        /// <param name="newExp">连接后输出的新对象表达式，该成员是LINQ连接体系中所必须的。</param>
        public DbInnerJoinExpression(DbUnitTypeExpression source, DbUnitTypeExpression target, DbExpression left, DbExpression right, DbUnitItemTypeExpression newExp)
            : base(typeof(IQueryable<>).MakeGenericType(newExp.ClrType), source, newExp)
        {
            Target = target;
            switch (left.ExpressionType)
            {
                case EExpressionType.MemberAccess:
                    KeyPairs = new DbJoinKeyPairExpression[] { new DbJoinKeyPairExpression(left, right) };
                    break;
                case EExpressionType.New:
                    KeyPairs = (from a in ((DbNewExpression)left).Members
                                join b in ((DbNewExpression)right).Members on a.Key equals b.Key
                                select new DbJoinKeyPairExpression(a.Value, b.Value)).ToArray();
                    break;
                default:
                    throw new NotSupportedException(string.Format(Res.NotSupportedExpressionParseInnerJoinKeyPairs, left.ExpressionType));
            }
        }
        /// <summary>
        /// 连接目标表达式。
        /// </summary>
        public DbUnitTypeExpression Target { get; private set; }
        /// <summary>
        /// 连接匹配键的集合。
        /// </summary>
        public DbJoinKeyPairExpression[] KeyPairs { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.InnerJoin;
    }
}