// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System.Linq.Expressions;
    /// <summary>
    /// 数据删除语句操作。
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    internal class DbDeleteStatementOperate<T> : DbStatementOperateBase
        where T : class
    {
        /// <summary>
        /// 创建数据删除语句操作。
        /// </summary>
        /// <param name="target">操作目标。</param>
        /// <param name="expression">删除表达式。</param>
        internal DbDeleteStatementOperate(DbSet<T> target, Expression expression)
            : base(target, expression, typeof(T))
        {
        }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.DeleteStatement;
    }
}