// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.ValueGenerates
{
    using System.Linq.Expressions;
    /// <summary>
    /// 表达式值生成对象。
    /// </summary>
    public class ValueGenerateExpression : ValueGenerateBase
    {
        /// <summary>
        /// 创建表达式值生成对象。
        /// </summary>
        /// <param name="expression">生成值的表达式。</param>
        internal ValueGenerateExpression(Expression expression)
            : base()
        {
            Expression = expression;
        }
        /// <summary>
        /// 当前生成值的表达式。
        /// </summary>
        public Expression Expression { get; set; }
        /// <inheritdoc/>
        public override EGeneratedOption GeneratedOption => EGeneratedOption.Expression;
    }
}