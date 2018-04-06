// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Operates;
    /// <summary>
    /// 公共生成数据对象。
    /// </summary>
    public class GenerateData
    {
        /// <summary>
        /// 创建数据对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">当前操作对象。</param>
        internal GenerateData(GenerateContext context, DbOperateBase operate)
        {
            GenerateContext = context;
            var operateContext = operate.Executor;
            Operate = operate;
            OperateCommand = operateContext.CurrentCommand;
            DataContext = operateContext.Context;
        }
        /// <summary>
        /// 生成上下文。
        /// </summary>
        public GenerateContext GenerateContext { get; }
        /// <summary>
        /// 当前操作对象。
        /// </summary>
        public DbOperateBase Operate { get; }
        /// <summary>
        /// 当前操作相应的命令对象。
        /// </summary>
        public DbOperateCommand OperateCommand { get; }
        /// <summary>
        /// 当前数据上下文。
        /// </summary>
        public DbContext DataContext { get; }
        /// <summary>
        /// 初始化当前数据对象。
        /// </summary>
        /// <param name="content">操作关联的表达式。</param>
        public virtual void Inititalze(DbExpression content = null) { }
    }
}