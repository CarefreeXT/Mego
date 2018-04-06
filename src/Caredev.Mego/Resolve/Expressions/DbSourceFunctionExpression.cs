// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Reflection;
    /// <summary>
    /// 数据集合函数表达式基类，用于表示聚合、判断及检索相关函数的基类。
    /// </summary>
    public abstract class DbSourceFunctionExpression : DbFunctionBaseExpression
    {
        /// <summary>
        /// 初始化数据集合函数。
        /// </summary>
        /// <param name="source">单元表达式。</param>
        /// <param name="function">函数CLR对象。</param>
        /// <param name="arguments">函数参数。</param>
        public DbSourceFunctionExpression(IDbUnitTypeExpression source, MemberInfo function, params DbExpression[] arguments)
            : base(function, arguments)
        {
            Source = source;
        }
        /// <summary>
        /// 数据集合对象，即函数调用的目标对象。
        /// </summary>
        public IDbUnitTypeExpression Source { get; private set; }
        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is DbSourceFunctionExpression)
            {
                var value = (DbSourceFunctionExpression)obj;
                if (value.Source == Source &&
                    value.Function == Function &&
                    value.Arguments.Length == Arguments.Length)
                {
                    int i = 0;
                    while (Arguments.Length > i)
                    {
                        if (value.Arguments[i] != Arguments[i])
                            return false;
                        i++;
                    }
                    return true;
                }
            }
            return false;
        }
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var code = Source.GetHashCode() ^ Function.GetHashCode();
            foreach (var arg in Arguments)
                code ^= arg.GetHashCode();
            return code;
        }
    }
}