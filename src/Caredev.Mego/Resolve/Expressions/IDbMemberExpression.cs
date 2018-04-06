// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System.Reflection;
    /// <summary>
    /// 表示访问成员表达式。
    /// </summary>
    public interface IDbMemberExpression : IDbExpression
    {
        /// <summary>
        /// 访问的目标表式。
        /// </summary>
        DbExpression Expression { get; }
        /// <summary>
        /// 访问的成员信息对象。
        /// </summary>
        MemberInfo Member { get; }
    }
}