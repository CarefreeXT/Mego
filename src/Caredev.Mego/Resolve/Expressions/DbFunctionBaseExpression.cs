// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    using System.Reflection;
    /// <summary>
    /// 函数表达式基类。
    /// </summary>
    public abstract class DbFunctionBaseExpression : DbExpression
    {
        /// <summary>
        /// 创建函数表达式基类。
        /// </summary>
        /// <param name="function"></param>
        /// <param name="arguments"></param>
        public DbFunctionBaseExpression(MemberInfo function, DbExpression[] arguments)
        {
            Function = function;
            Arguments = arguments;
            switch (function)
            {
                case MethodInfo method:
                    ClrType = method.ReturnType;
                    break;
                case PropertyInfo proerty:
                    ClrType = proerty.PropertyType;
                    break;
            }
        }
        /// <summary>
        /// 函数参数表达式集合。
        /// </summary>
        public DbExpression[] Arguments { get; private set; }
        /// <summary>
        /// 函数的CLR描述对象。
        /// </summary>
        public MemberInfo Function { get; private set; }
        /// <summary>
        /// 函数返回值的CLR类型。
        /// </summary>
        public Type ClrType { get; private set; }
    }
}