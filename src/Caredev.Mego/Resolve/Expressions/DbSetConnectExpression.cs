// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 数据单元连接操作表达式。
    /// </summary>
    public class DbSetConnectExpression : DbSetOperationExpression
    {
        /// <summary>
        /// 创建数据集连接操作。
        /// </summary>
        /// <param name="source">源数据单元表达式。</param>
        /// <param name="target">目标数据单元表达式。</param>
        /// <param name="kind">连接种类。</param>
        public DbSetConnectExpression(DbUnitTypeExpression source, DbUnitTypeExpression target, EConnectKind kind)
            : base(source.ClrType, source, new DbDataItemExpression(source.Item.ClrType))
        {
            Target = target;
            Kind = kind;
        }
        /// <summary>
        /// 目标数据单元表达式。
        /// </summary>
        public DbUnitTypeExpression Target { get; private set; }
        /// <summary>
        /// 连接操作种类。
        /// </summary>
        public EConnectKind Kind { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.SetConnect;
    }
}