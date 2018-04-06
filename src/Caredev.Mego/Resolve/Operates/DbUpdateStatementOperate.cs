// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System.Linq.Expressions;
    /// <summary>
    /// 表达式语句更新操作。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DbUpdateStatementOperate<T> : DbStatementOperateBase
        where T : class
    {
        /// <summary>
        /// 使用指定的表达式创建更新操作。
        /// </summary>
        /// <param name="target">操作目标。</param>
        /// <param name="expression">用于更新的表达式。</param>
        internal DbUpdateStatementOperate(DbSet<T> target, Expression expression)
            : base(target, expression, typeof(T))
        {
        }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.UpdateStatement;
    }
}