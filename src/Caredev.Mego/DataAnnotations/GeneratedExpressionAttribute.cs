// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System.Linq.Expressions;
    using Caredev.Mego.Resolve.ValueGenerates;
    /// <summary>
    /// 表达式生成值特性，该特性标识当前属性在提交数据时按指定表达式生成值。
    /// </summary>
    public abstract class GeneratedExpressionAttribute : GeneratedValueBaseAttribute
    {
        /// <summary>
        /// 创建表达式生成值特性。
        /// </summary>
        /// <param name="purpose">生成时的目的。</param>
        public GeneratedExpressionAttribute(EGeneratedPurpose purpose)
            : base(purpose)
        {
        }
        /// <summary>
        /// 生成值使用的表达式。
        /// </summary>
        public Expression Expression { get; protected set; }
        /// <summary>
        /// 生成选项。
        /// </summary>
        public override EGeneratedOption GeneratedOption => EGeneratedOption.Expression;
    }
}