// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    /// <summary>
    /// 数字列精度特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PrecisionAttribute : Attribute, IColumnAnnotation
    {
        /// <summary>
        /// 创建精度特性。
        /// </summary>
        /// <param name="precision">精度。</param>
        /// <param name="scale">小数位数。</param>
        public PrecisionAttribute(byte precision, byte scale = 0)
        {
            Precision = precision;
            Scale = scale;
        }
        /// <summary>
        /// 精度。
        /// </summary>
        public byte Precision { get; }
        /// <summary>
        /// 小数位数。
        /// </summary>
        public byte Scale { get; }
    }
}