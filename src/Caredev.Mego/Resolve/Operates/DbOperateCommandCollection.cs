// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    /// <summary>
    /// 操作命令集合对象。
    /// </summary>
    internal class DbOperateCommandCollection : IEnumerable<DbOperateCommandBase>
    {
        private readonly DbOperateContext operationContext;
        private readonly List<DbOperateCommandBase> commands;
        private DbOperateCommandBase current;
        private readonly EExecutionMode _ExecutionMode;
        /// <summary>
        /// 创建操作命令集合对象。
        /// </summary>
        /// <param name="context">操作上下文。</param>
        public DbOperateCommandCollection(DbOperateContext context)
        {
            _ExecutionMode = context.Context.Database.Provider.ExecutionMode;
            operationContext = context;
            commands = new List<DbOperateCommandBase>();
            CheckParameterCount();
        }
        /// <summary>
        /// 检查当前操作集合中参数数量是否足够完成数据提交。
        /// </summary>
        /// <param name="count">检查的数量。</param>
        /// <returns>返回可满足的操作命令。</returns>
        public DbOperateCommandBase CheckParameterCount(int count = 50)
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
        public DbOperateCommandBase NextCommand()
        {
            if (current != null && current.IsEmpty)
            {
                return current;
            }
            if (_ExecutionMode == EExecutionMode.MergeOperations)
            {
                current = new DbMultiOperateCommand(operationContext);
            }
            else
            {
                current = new DbSingleOperateCommand(operationContext)
                {
                    IsBlockStatement = _ExecutionMode == EExecutionMode.SingleOperation
                };
            }
            commands.Add(current);
            operationContext.CurrentCommand = current;
            return current;
        }
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现
        /// </summary>
        /// <returns>枚举对象。</returns>
        public IEnumerator<DbOperateCommandBase> GetEnumerator()
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
        /// <summary>
        /// 向命令集合注册数据库操作。
        /// </summary>
        /// <param name="operate">注册的操作。</param>
        /// <param name="parametercount">最大参数数量。</param>
        public void Register(DbOperateBase operate, int parametercount)
        {
            var command = this.CheckParameterCount(parametercount);
            if (command.ConcurrencyExpectCount > 0 || command is DbSingleOperateCommand)
            {
                //如果当前命令包含并发检查操作，则移动到下一个命令中。
                command = this.NextCommand();
            }
            command.RegisteOperate(operate);
            if (operate is IConcurrencyCheckOperate concurrency2 && concurrency2.NeedCheck)
            {
                command.ConcurrencyExpectCount += concurrency2.ExpectCount;
            }
        }
        /// <summary>
        /// 向命令集合注册数据库可分割的数据集合操作。
        /// </summary>
        /// <param name="itemoperate">注册的数据集合操作。</param>
        public void Register(IDbSplitObjectsOperate itemoperate)
        {
            var operate = (DbOperateBase)itemoperate;
            int index = 0, length = 0, count = 0, sumcount = itemoperate.Count;
            do
            {
                if (current == null || index > 0 || current is DbSingleOperateCommand)
                {
                    current = this.NextCommand();
                }
                count = current.ParameterCount / itemoperate.ItemParameterCount;
                length = Math.Min(count, sumcount - index);
                itemoperate.Split(index, length, () => current.RegisteOperate(itemoperate, index, length));
                if (operate is IConcurrencyCheckOperate concurrency2 && concurrency2.NeedCheck)
                {
                    //累加并发检查期望数
                    current.ConcurrencyExpectCount += concurrency2.ExpectCount;
                }
                index += length;
            } while (index < sumcount - 1);
        }
    }
}