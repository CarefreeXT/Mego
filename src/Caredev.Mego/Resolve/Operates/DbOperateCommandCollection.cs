// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System.Collections;
    using System.Collections.Generic;
    /// <summary>
    /// 操作命令集合对象。
    /// </summary>
    internal class DbOperateCommandCollection : IEnumerable<DbOperateCommand>
    {
        private readonly DbOperateContext operationContext;
        private readonly List<DbOperateCommand> commands;
        private DbOperateCommand current;
        /// <summary>
        /// 创建操作命令集合对象。
        /// </summary>
        /// <param name="context">操作上下文。</param>
        public DbOperateCommandCollection(DbOperateContext context)
        {
            operationContext = context;
            commands = new List<DbOperateCommand>();
            CheckParameterCount();
        }
        /// <summary>
        /// 检查当前操作集合中参数数量是否足够完成数据提交。
        /// </summary>
        /// <param name="count">检查的数量。</param>
        /// <returns>返回可满足的操作命令。</returns>
        public DbOperateCommand CheckParameterCount(int count = 50)
        {
            if (current == null || current.ParameterCount < count)
            {
                current = NextCommand();
            }
            return current;
        }
        /// <summary>
        /// 生成一个新的操作命令对象。
        /// </summary>
        /// <returns>命令对象。</returns>
        public DbOperateCommand NextCommand()
        {
            current = new DbOperateCommand(operationContext);
            commands.Add(current);
            operationContext.CurrentCommand = current;
            return current;
        }
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现
        /// </summary>
        /// <returns>枚举对象。</returns>
        public IEnumerator<DbOperateCommand> GetEnumerator()
        {
            return commands.GetEnumerator();
        }
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现
        /// </summary>
        /// <returns>枚举对象。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return commands.GetEnumerator();
        }
    }
}