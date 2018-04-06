// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    /// <summary>
    /// 字符串描述特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class StringAttribute : LengthAttribute
    {
        /// <summary>
        /// 创建字符串描述特性。
        /// </summary>
        /// <param name="length">字符串长度。</param>
        /// <param name="isfixed">是否固定长度字符串。</param>
        public StringAttribute(int length = 0, bool isfixed = false)
            : base(length, isfixed)
        {
        }
        /// <summary>
        /// 字符串是否 Unicode 编码。
        /// </summary>
        public bool IsUnicode { get; set; } = true;
    }
}