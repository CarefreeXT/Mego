// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    using Res = Properties.Resources;
    /// <summary>
    /// 数据库表特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// 创建数据库表特性。
        /// </summary>
        /// <param name="name">映射到数据库的表名。</param>
        /// <param name="inheritedProperties">是否映射父类数据表的属性。</param>
        public TableAttribute(string name, bool inheritedProperties = false)
        {
            if (string.IsNullOrEmpty(name)
#if !NET35
                || string.IsNullOrWhiteSpace(name)
#endif
                )
            {
                throw new ArgumentNullException(Res.ExceptionTableNameCannotEmpty, nameof(name));
            }
            Name = name;
            MapInheritedProperties = inheritedProperties;
        }
        /// <summary>
        /// 数据库表名。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 数据库架构名。
        /// </summary>
        public string Schema { get; set; }
        /// <summary>
        /// 是否映射父类数据表的属性，如果该属性为 True 则表示父类的所有属性不
        /// 在当前映射的数据表中存储，当前数据表与父类数据表通过主键关联。
        /// </summary>
        public bool MapInheritedProperties { get; }
    }
}