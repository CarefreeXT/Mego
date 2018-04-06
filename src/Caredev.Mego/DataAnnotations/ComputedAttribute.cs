// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    /// <summary>
    /// 计算列特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputedAttribute : Attribute, IColumnAnnotation
    {
        /// <summary>
        /// 创建计算列特性。
        /// </summary>
        /// <param name="expression">数据库表达式。</param>
        public ComputedAttribute(string expression)
        {
            Expression = expression;
        }
        /// <summary>
        /// 数据库计算表达式。
        /// </summary>
        public string Expression { get; }
        /// <summary>
        /// 是否持久化值。
        /// </summary>
        public bool IsPersisted { get; set; }
    }
}