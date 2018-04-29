// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Resolve.Commands;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Operates;
    /// <summary>
    /// 数据操作内容基类。
    /// </summary>
    public class OperateContentBase
    {
        /// <summary>
        /// 创建数据操作内容。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">操作对象。</param>
        internal OperateContentBase(GenerateContext context, DbOperateBase operate)
        {
            GenerateContext = context;
            var operateContext = operate.Executor;
            Operate = operate;
            OperateCommand = operateContext.CurrentCommand;
            DataContext = operateContext.Context;
        }
        /// <summary>
        /// 数据上下文。
        /// </summary>
        public DbContext DataContext { get; }
        /// <summary>
        /// 生成上下文。
        /// </summary>
        public GenerateContext GenerateContext { get; }
        /// <summary>
        /// 数据操作对象。
        /// </summary>
        public DbOperateBase Operate { get; }
        /// <summary>
        /// 数据操作关联的命令对象。
        /// </summary>
        internal OperateCommandBase OperateCommand { get; }
        /// <summary>
        /// 初始化内容对象。
        /// </summary>
        /// <param name="content">操作关联的表达式。</param>
        public virtual void Inititalze(DbExpression content = null) { }
    }
}