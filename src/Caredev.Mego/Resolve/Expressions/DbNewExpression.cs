// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    /// <summary>
    /// 新建对象表达式，通常用于做字段映射。
    /// </summary>
    public class DbNewExpression : DbUnitItemTypeExpression
    {
        /// <summary>
        /// 创建新建对象表达式。
        /// </summary>
        /// <param name="type"></param>
        public DbNewExpression(Type type) : base(type)
        {
            Members = new Dictionary<MemberInfo, DbExpression>();
        }
        /// <summary>
        /// 成员表达式集合。
        /// </summary>
        public Dictionary<MemberInfo, DbExpression> Members { get; private set; }
        /// <inheritdoc/>
        public override EExpressionType ExpressionType => EExpressionType.New;
    }
}