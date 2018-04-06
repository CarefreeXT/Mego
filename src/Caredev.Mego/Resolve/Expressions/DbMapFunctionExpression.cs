// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Reflection;
    /// <summary>
    /// 函数映射表达式。
    /// </summary>
    public abstract class DbMapFunctionExpression : DbFunctionBaseExpression
    {
        /// <summary>
        /// 创建函数映射表达式。
        /// </summary>
        /// <param name="func">函数CLR描述对象。</param>
        /// <param name="kind">映射种类。</param>
        /// <param name="argus">函数参数。</param>
        public DbMapFunctionExpression(MemberInfo func, EMapFunctionKind kind, DbExpression[] argus)
            : base(func, argus)
        {
            Kind = kind;
        }
        /// <summary>
        /// 映射种类。
        /// </summary>
        public EMapFunctionKind Kind { get; private set; }
    }
}