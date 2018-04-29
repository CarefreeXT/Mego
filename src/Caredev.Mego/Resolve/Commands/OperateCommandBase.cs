// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Commands
{
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Diagnostics;
    using Res = Properties.Resources;
    /// <summary>
    /// 数据库操作命令对象。
    /// </summary>
    internal abstract class OperateCommandBase
    {
        private int currentVariableNameIndex = 0;
        private int currentParameterNameIndex = 0;
        private readonly DbProviderFactory factory;
        private readonly int maxParameterCount;
        private readonly Dictionary<object, int> variables = new Dictionary<object, int>();
        /// <summary>
        /// 当前命令注册的常规参数。
        /// </summary>
        protected readonly Dictionary<object, DbParameter> Parameters = new Dictionary<object, DbParameter>();
        /// <summary>
        /// 创建数据库操作命令对象。
        /// </summary>
        /// <param name="context">操作上下文。</param>
        public OperateCommandBase(DbOperateContext context)
        {
            Executor = context;
            var database = context.Context.Database;
            var provider = database.Provider;
            factory = provider.Factory;
            var feature = database.Feature;
            if (feature.MaxParameterCount.HasValue)
            {
                maxParameterCount = feature.MaxParameterCount.Value;
            }
            else
            {
                maxParameterCount = short.MaxValue;
            }
        }
        /// <summary>
        /// 当前命令可用的参数数量。
        /// </summary>
        public int ParameterCount
        {
            get { return maxParameterCount - currentParameterNameIndex; }
        }
        /// <summary>
        /// 当前命令的操作上下文。
        /// </summary>
        public DbOperateContext Executor { get; }
        /// <summary>
        /// 并发期望影响行数。
        /// </summary>
        public int ConcurrencyExpectCount { get; internal set; }
        /// <summary>
        /// 当前命令对象是否为空，即不包含任何操作。
        /// </summary>
        public abstract bool IsEmpty { get; }
        /// <summary>
        /// 添加参数。
        /// </summary>
        /// <param name="value">参数值。</param>
        /// <param name="prefix">参数名前缀。</param>
        /// <returns>返回参数名。</returns>
        public virtual string AddParameter(object value, string prefix)
        {
#if !DEBUG
            if (value == null)
            {
                value = DBNull.Value;
            }  
#endif
            if (currentParameterNameIndex >= maxParameterCount)
                throw new Exception("Parameter count");
            if (!Parameters.TryGetValue(value, out DbParameter parameter))
            {
                parameter = CreateParameter(value, prefix);
                Parameters.Add(value, parameter);
            }
            return parameter.ParameterName;
        }
        /// <summary>
        /// 获取自定义命令。
        /// </summary>
        /// <typeparam name="TCommand">命令类型。</typeparam>
        /// <returns>创建的命令。</returns>
        public TCommand GetCustomCommand<TCommand>() where TCommand : ICustomCommand
        {
            if (_CustomCommand == null)
            {
                _CustomCommand = this.Executor.Context.Database.Provider.CreateCustomCommand();
            }
            return (TCommand)_CustomCommand;
        }
        internal protected ICustomCommand _CustomCommand;
        /// <summary>
        /// 创建参数。
        /// </summary>
        /// <param name="value">参数值。</param>
        /// <param name="prefix">参数名前缀。</param>
        /// <returns>参数对象。</returns>
        internal protected DbParameter CreateParameter(object value, string prefix = "p")
        {
            var parameter = factory.CreateParameter();
            parameter.Value = value;
            parameter.ParameterName = prefix + (currentParameterNameIndex++).ToString("X");
            return parameter;
        }
        /// <summary>
        /// 获取唯一变量索引。
        /// </summary>
        /// <returns>变量索引值。</returns>
        public int GetVariableName(object key)
        {
            if (!variables.TryGetValue(key, out int value))
            {
                value = currentVariableNameIndex++;
                variables.Add(key, value);
            }
            return value;
        }
        /// <summary>
        /// 注册操作。
        /// </summary>
        /// <param name="operate">操作对象。</param>
        internal abstract void RegisteOperate(DbOperateBase operate);
        /// <summary>
        /// 注册操作。
        /// </summary>
        /// <param name="operate">操作对象。</param>
        /// <param name="index">分割操作起始索引。</param>
        /// <param name="length">分割操作长度。</param>
        internal abstract void RegisteOperate(IDbSplitObjectsOperate operate, int index, int length);
        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="executor">数据库执行对象。</param>
        /// <returns>对数据库实际的影响行数。</returns>
        internal int Execute(DatabaseExecutor executor)
        {
            var start = DateTime.Now;
            BeginExecute(start);
            var recordsAffectCount = ExecuteImp(executor);
            EndExecute(start);
            if (ConcurrencyExpectCount > 0 && ConcurrencyExpectCount != recordsAffectCount)
            {
                throw new DbCommitConcurrencyException(Res.ExceptionCommitConcurrency);
            }
            return recordsAffectCount;
        }

        [Conditional("DEUBG")]
        internal static void OutputCommand(DbCommand command)
        {
            Debug.Write("Command Content : ");
            Debug.WriteLine(command.CommandText);
            Debug.WriteLine("");
            foreach (DbParameter p in command.Parameters)
            {
                Debug.WriteLine($"\t{p.ParameterName}\t:{p.Value}");
            }
        }

        [Conditional("DEUBG")]
        private static void BeginExecute(DateTime start)
        {
            Debug.WriteLine($"---------- 开始执行 ：{start.ToString("HH:mm:ss.fff")} ----------");
            Debug.WriteLine("");
        }

        [Conditional("DEUBG")]
        private static void EndExecute(DateTime start)
        {
            var end = DateTime.Now;
            var space = end - start;
            Debug.WriteLine("");
            Debug.WriteLine($"---------- 执行完成 ：{end.ToString("HH:mm:ss.fff")}，执行耗时：{space.TotalMilliseconds}毫秒 ----------");
            Debug.WriteLine("");
        }

        internal abstract int ExecuteImp(DatabaseExecutor executor);

    }
    internal class SplitIndexLength
    {
        public SplitIndexLength(IDbSplitObjectsOperate operate, int index, int length)
        {
            Operate = operate;
            Index = index;
            Length = length;
        }

        public IDbSplitObjectsOperate Operate { get; }
        public int Index { get; }
        public int Length { get; }
    }
}