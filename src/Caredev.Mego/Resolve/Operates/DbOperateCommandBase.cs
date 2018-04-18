// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using Caredev.Mego.Exceptions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Diagnostics;
    using System.Text;
    using Res = Properties.Resources;
    /// <summary>
    /// 数据库操作命令对象。
    /// </summary>
    internal abstract class DbOperateCommandBase
    {
        private int currentVariableNameIndex = 0;
        private int currentParameterNameIndex = 0;
        private readonly DbProviderFactory factory;
        private readonly int maxParameterCount;
        protected readonly Dictionary<object, DbParameter> parameters = new Dictionary<object, DbParameter>();
        private readonly Dictionary<object, int> variables = new Dictionary<object, int>();
        /// <summary>
        /// 创建数据库操作命令对象。
        /// </summary>
        /// <param name="context">操作上下文。</param>
        public DbOperateCommandBase(DbOperateContext context)
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
        public string AddParameter(object value, string prefix)
        {
#if !DEBUG
            if (value == null)
            {
                value = DBNull.Value;
            }  
#endif
            if (currentParameterNameIndex >= maxParameterCount)
                throw new Exception("Parameter count");
            if (!parameters.TryGetValue(value, out DbParameter parameter))
            {
                parameter = factory.CreateParameter();
                parameter.Value = value;
                parameter.ParameterName = prefix + (currentParameterNameIndex++).ToString("X");
                parameters.Add(value, parameter);
            }
            return parameter.ParameterName;
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
        internal abstract int Execute(DatabaseExecutor excutor);

        protected class SplitIndexLength
        {
            public IDbSplitObjectsOperate Operate;
            public int Index;
            public int Length;
        }
    }
    /// <summary>
    /// 多个数据库操作命令对象。
    /// </summary>
    internal class DbMultiOperateCommand : DbOperateCommandBase, IEnumerable<DbOperateBase>
    {
        private readonly IList<DbOperateBase> operates = new List<DbOperateBase>();
        private Dictionary<DbOperateBase, SplitIndexLength> splitesOperates;
        private readonly StringBuilder commandBuilder = new StringBuilder();
        /// <summary>
        /// 创建命令对象。
        /// </summary>
        /// <param name="context">操作执行上下文</param>
        public DbMultiOperateCommand(DbOperateContext context)
            : base(context) { }
        /// <inheritdoc/>
        public override bool IsEmpty => operates.Count == 0;
        /// <inheritdoc/>
        internal override int Execute(DatabaseExecutor executor)
        {
            var command = executor.CreateCommand(commandBuilder.ToString());
            if (parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.Values.ToArray());
            }
            var readOperates = operates.Where(operate => operate.HasResult && operate.Output != null).ToArray();
            int recordsAffectCount = 0;
#if DEBUG
            var start = DateTime.Now;
            Debug.WriteLine($"---------- 开始执行 ：{start.ToString("HH:mm:ss.fff")} ----------");
            Debug.WriteLine("");
            Debug.WriteLine(command.CommandText);
            Debug.WriteLine("");
            foreach (DbParameter p in command.Parameters)
            {
                Debug.WriteLine($"\t{p.ParameterName}\t:{p.Value}");
            }
#endif
            if (readOperates.Length > 0)
            {
                using (var reader = command.ExecuteReader())
                {
                    if (splitesOperates != null)
                    {
                        foreach (var operate in readOperates)
                        {
                            if (splitesOperates.TryGetValue(operate, out SplitIndexLength value))
                            {
                                value.Operate.Split(value.Index, value.Length, () => operate.Read(reader));
                            }
                            else
                            {
                                operate.Read(reader);
                            }
                            reader.NextResult();
                        }
                    }
                    else
                    {
                        foreach (var operate in readOperates)
                        {
                            operate.Read(reader);
                            reader.NextResult();
                        }
                    }
                    recordsAffectCount = reader.RecordsAffected;
                }
            }
            else
            {
                recordsAffectCount = command.ExecuteNonQuery();
            }
#if DEBUG
            var end = DateTime.Now;
            var space = end - start;
            Debug.WriteLine("");
            Debug.WriteLine($"---------- 执行完成 ：{end.ToString("HH:mm:ss.fff")}，执行耗时：{space.TotalMilliseconds}毫秒 ----------");
            Debug.WriteLine("");
#endif
            if (ConcurrencyExpectCount > 0 && ConcurrencyExpectCount != recordsAffectCount)
            {
                throw new DbCommitConcurrencyException(Res.ExceptionCommitConcurrency);
            }
            return recordsAffectCount;
        }
        /// <inheritdoc/>
        internal override void RegisteOperate(DbOperateBase operate)
        {
            operates.Add(operate);
            commandBuilder.AppendLine(operate.GenerateSql());
        }
        /// <inheritdoc/>
        internal override void RegisteOperate(IDbSplitObjectsOperate operate, int index, int length)
        {
            var operateInstance = (DbOperateBase)operate;
            operates.Add(operateInstance);
            commandBuilder.AppendLine(operateInstance.GenerateSql());
            if (splitesOperates == null)
            {
                splitesOperates = new Dictionary<DbOperateBase, SplitIndexLength>();
            }
            if (!splitesOperates.ContainsKey(operateInstance))
            {
                splitesOperates.Add(operateInstance, new SplitIndexLength()
                {
                    Operate = operate,
                    Index = index,
                    Length = length
                });
            }
        }
        /// <summary>
        /// <see cref="IEnumerable{T}"/>接口实现。
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<DbOperateBase> GetEnumerator()
        {
            return operates.GetEnumerator();
        }
        /// <summary>
        /// <see cref="IEnumerable"/>接口实现。
        /// </summary>
        /// <returns>枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    /// <summary>
    /// 单个数据库操作命令对象。
    /// </summary>
    internal class DbSingleOperateCommand : DbOperateCommandBase
    {
        private string _Statement;
        private SplitIndexLength _SplitInfo;
        /// <summary>
        /// 创建命令对象。
        /// </summary>
        /// <param name="context">操作执行上下文</param>
        public DbSingleOperateCommand(DbOperateContext context)
            : base(context) { }
        /// <inheritdoc/>
        public override bool IsEmpty => Operate == null;
        /// <summary>
        /// 当前操作对象。
        /// </summary>
        public DbOperateBase Operate { get; private set; }

        public bool IsBlockStatement { get; set; }

        public bool IsLoopExecution { get; }
        /// <inheritdoc/>
        internal override void RegisteOperate(DbOperateBase operate)
        {
            Operate = operate;
            _Statement = operate.GenerateSql();
        }
        /// <inheritdoc/>
        internal override void RegisteOperate(IDbSplitObjectsOperate operate, int index, int length)
        {
            var operateInstance = (DbOperateBase)operate;
            RegisteOperate(operateInstance);
            _SplitInfo = new SplitIndexLength() { Operate = operate, Index = index, Length = length };
        }
        /// <inheritdoc/>
        internal override int Execute(DatabaseExecutor executor)
        {
            var operate = Operate;
            var command = executor.CreateCommand(_Statement);
            if (parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.Values.ToArray());
            }
            int recordsAffectCount = 0;
            if (operate.HasResult && operate.Output != null)
            {
                using (var reader = command.ExecuteReader())
                {
                    if (_SplitInfo != null)
                    {
                        _SplitInfo.Operate.Split(_SplitInfo.Index, _SplitInfo.Length, () => operate.Read(reader));
                    }
                    else
                    {
                        operate.Read(reader);
                        reader.NextResult();
                    }
                    recordsAffectCount = reader.RecordsAffected;
                }
            }
            else
            {
                recordsAffectCount = command.ExecuteNonQuery();
            }
            if (ConcurrencyExpectCount > 0 && ConcurrencyExpectCount != recordsAffectCount)
            {
                throw new DbCommitConcurrencyException(Res.ExceptionCommitConcurrency);
            }
            return recordsAffectCount;
        }
    }
}