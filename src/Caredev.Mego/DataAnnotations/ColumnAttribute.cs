// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    /// <summary>
    /// 数据库列特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute, IColumnAnnotation
    {
        /// <summary>
        /// 创建数据库列特性。
        /// </summary>
        /// <param name="name">列名。</param>
        /// <param name="typename">数据库类型名。</param>
        public ColumnAttribute(string name, string typename = null)
        {
            if (string.IsNullOrEmpty(name)
#if !NET35
                || string.IsNullOrWhiteSpace(name)
#endif
                )
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            TypeName = typename;
        }
        /// <summary>
        /// 列名。
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// 列的顺序。
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 类型名。
        /// </summary>
        public string TypeName { get; }
    }
}