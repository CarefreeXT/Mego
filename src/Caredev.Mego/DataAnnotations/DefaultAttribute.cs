// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    /// <summary>
    /// 数据列默认值。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultAttribute : Attribute, IColumnAnnotation
    {
        /// <summary>
        /// 创建默认值特性。
        /// </summary>
        /// <param name="content">默认值内容。</param>
        public DefaultAttribute(string content)
        {
            Content = content;
        }
        /// <summary>
        /// 默认值内容。
        /// </summary>
        public string Content { get; }
    }
}