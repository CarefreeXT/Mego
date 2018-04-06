// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 一元操作符种类。
    /// </summary>
    public enum EUnaryKind
    {
        /// <summary>
        /// 一元正运算操作。
        /// </summary>
        UnaryPlus,
        /// <summary>
        /// 算术求反运算操作。
        /// </summary>
        Negate,
        /// <summary>
        /// 按位求补运算操作。
        /// </summary>
        Not,
        /// <summary>
        /// 类型转换运算操作。
        /// </summary>
        Convert
    }
}
