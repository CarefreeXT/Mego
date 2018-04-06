// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 二元运算操作种类。
    /// </summary>
    public enum EBinaryKind
    {
        /// <summary>
        /// 算术加法运算。
        /// </summary>
        Add                 = 0x01,
        /// <summary>
        /// 算术除法运算。
        /// </summary>
        Divide              = 0x02,
        /// <summary>
        /// 算术余数运算。
        /// </summary>
        Modulo              = 0x03,
        /// <summary>
        /// 算术乘法运算。
        /// </summary>
        Multiply            = 0x04,
        /// <summary>
        /// 幂运算。
        /// </summary>
        Power               = 0x05,
        /// <summary>
        /// 算术减法运算。
        /// </summary>
        Subtract            = 0x06,
        /// <summary>
        /// 按位与运算。
        /// </summary>
        And                 = 0x11,
        /// <summary>
        /// 按位或运算。
        /// </summary>
        Or                  = 0x12,
        /// <summary>
        /// 按异或运算
        /// </summary>
        ExclusiveOr         = 0x13,
        /// <summary>
        /// 按位左移运算。
        /// </summary>
        LeftShift           = 0x21,
        /// <summary>
        /// 按位右移运算。
        /// </summary>
        RightShift          = 0x22,
        /// <summary>
        /// 逻辑与运算。
        /// </summary>
        AndAlso             = 0x31,
        /// <summary>
        /// 逻辑或运算。
        /// </summary>
        OrElse              = 0x32,
        /// <summary>
        /// 相等比较。
        /// </summary>
        Equal               = 0x41,
        /// <summary>
        /// 不相等比较。
        /// </summary>
        NotEqual            = 0x42,
        /// <summary>
        /// “大于或等于”数值比较。
        /// </summary>
        GreaterThanOrEqual  = 0x43,
        /// <summary>
        /// “大于”数值比较。
        /// </summary>
        GreaterThan         = 0x44,
        /// <summary>
        /// “小于”数值比较。
        /// </summary>
        LessThan            = 0x45,
        /// <summary>
        /// “大于或等于”数值比较。
        /// </summary>
        LessThanOrEqual     = 0x46,
        /// <summary>
        /// 赋值运算。
        /// </summary>
        Assign              = 0x51,
    }
}