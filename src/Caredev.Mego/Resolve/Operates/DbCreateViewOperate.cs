// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System.Linq.Expressions;
    /// <summary>
    /// 创建视图操作。
    /// </summary>
    internal class DbCreateViewOperate : DbMaintenanceOperateBase
    {
        /// <summary>
        /// 初始化创建视图操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="expression">视图相关的查询表达式。</param>
        internal DbCreateViewOperate(DbContext context, Expression expression)
             : base(context, expression.Type)
        {
        }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.CreateView;
    }
}