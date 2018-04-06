// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    /// <summary>
    /// 可为空特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NullableAttribute : Attribute, IColumnAnnotation
    {
        /// <summary>
        /// 创建可为空特性。
        /// </summary>
        /// <param name="value"></param>
        public NullableAttribute(bool value)
        {
            Value = value;
        }
        /// <summary>
        /// 可为空值。
        /// </summary>
        public bool Value { get; }
    }
}