// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
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
        /// <param name="type">关系的数据对象类型。</param>
        /// <param name="name">视图名称。</param>
        /// <param name="expression">视图相关的查询表达式。</param>
        internal DbCreateViewOperate(DbContext context, Type type, DbName name, Expression expression)
             : base(context, type, name)
        {
            _Expression = expression;
        }
        /// <summary>
        /// 初始化创建视图操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="type">关系的数据对象类型。</param>
        /// <param name="name">视图名称。</param>
        /// <param name="statement">内容语句。</param>
        internal DbCreateViewOperate(DbContext context, Type type, DbName name, string statement)
            : base(context, type, name)
        {
            Statement = statement;
        }
        /// <summary>
        /// 视图内容语句。
        /// </summary>
        public string Statement { get; private set; }
        /// <inheritdoc/>
        public override EOperateType Type => EOperateType.CreateView;
        private readonly Expression _Expression;
        /// <inheritdoc/>
        internal override string GenerateSql()
        {
            if (string.IsNullOrEmpty(Statement))
            {
                var operate = new DbQueryCollectionOperate(Executor.Context, _Expression, ClrType);
                Statement = operate.GenerateSql().TrimEnd(';', ' ');
            }
            return base.GenerateSql();
        }
    }
}