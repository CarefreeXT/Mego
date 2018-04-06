// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// 查询表达式基类。
    /// </summary>
    public abstract class DbExpression : IDbExpression
    {
        private Dictionary<object, DbExpression> _Children;
        /// <summary>
        /// 数据表达式类型。
        /// </summary>
        public abstract EExpressionType ExpressionType { get; }
        /// <summary>
        /// 获取或设置当前表达式的子表达式。
        /// </summary>
        /// <param name="key">查找键值。</param>
        /// <param name="creator">创建子表达式。</param>
        /// <returns>查找或创建的表达式。</returns>
        public DbExpression GetOrSetChildren(object key, Func<DbExpression> creator)
        {
            if (_Children == null)
                _Children = new Dictionary<object, DbExpression>();
            if (_Children.TryGetValue(key, out DbExpression value))
                return value;
            value = creator();
            _Children.Add(key, value);
            return value;
        }
    }
}