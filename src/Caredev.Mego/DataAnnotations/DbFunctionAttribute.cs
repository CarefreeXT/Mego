// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    using Res = Properties.Resources;
    /// <summary>
    /// 自定义数据库函数映射特性，该特性用于映射CLR中定义的静态函数
    /// 及实例函数到数据库函数，自定义函数优先级会低于系统函数。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DbFunctionAttribute : Attribute
    {
        /// <summary>
        /// 创建自定义函数映射特性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="schema"></param>
        public DbFunctionAttribute(string name, string schema = "")
        {
            if (string.IsNullOrEmpty(name)
#if !NET35
                || string.IsNullOrWhiteSpace(name)
#endif
                )
            {
                throw new ArgumentNullException(Res.ExceptionFunctionNameCannotEmpty, nameof(name));
            }
            Name = name;
        }
        /// <summary>
        /// 映射函数名。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 映射函数架构名。
        /// </summary>
        public string Schema { get; set; }
        /// <summary>
        /// 是否为系统函数，如果为是则会直接输出<see cref="Name"/>字符。
        /// </summary>
        public bool IsSystemFunction { get; set; }
    }
}
