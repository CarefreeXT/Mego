// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    /// <summary>
    /// 数据列长度特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LengthAttribute : Attribute, IColumnAnnotation
    {
        /// <summary>
        /// 创建长度特性。
        /// </summary>
        /// <param name="length">长度值。</param>
        /// <param name="isfixed">是否固定长度。</param>
        public LengthAttribute(int length = 0, bool isfixed = false)
        {
            Length = length;
            IsFixed = isfixed;
        }
        /// <summary>
        /// 当前数据列长度，默认为 0 ，如果小于或等于 0 则表示不限长度。
        /// </summary>
        public int Length { get; }
        /// <summary>
        /// 是否固定长度。
        /// </summary>
        public bool IsFixed { get; }
    }
}