// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 所有数据表达式都需要实现的接口。
    /// </summary>
    public interface IDbExpression
    {
        /// <summary>
        /// 数据表达式类型。
        /// </summary>
        EExpressionType ExpressionType { get; }
    }
}