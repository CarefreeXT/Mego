// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Commands
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
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Common;
    using System.Data;
    using System.Reflection;
    using Caredev.Mego.Resolve.ValueConversion;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Providers;

    /// <summary>
    /// 单个数据库操作命令对象。
    /// </summary>
    internal class SingleOperateCommand : OperateCommandBase
    {
        private string _Statement;
        private SplitIndexLength _SplitInfo;
        /// <summary>
        /// 创建命令对象。
        /// </summary>
        /// <param name="context">操作执行上下文。</param>
        /// <param name="mode">执行模式。</param>
        public SingleOperateCommand(DbOperateContext context, EExecutionMode mode)
            : base(context)
        {
            if (mode == EExecutionMode.SingleOperation)
            {
                ExecuteMode = ECommandExecuteMode.Simple;
            }
            else
            {
                ExecuteMode = ECommandExecuteMode.Split;
            }
        }
        /// <inheritdoc/>
        public override bool IsEmpty => Operate == null;
        /// <summary>
        /// 当前操作对象。
        /// </summary>
        public DbOperateBase Operate { get; private set; }
        /// <summary>
        /// 命令的执行模式。
        /// </summary>
        public ECommandExecuteMode ExecuteMode { get; }
        /// <inheritdoc/>
        public override string AddParameter(object value, string prefix)
        {
            if (_CustomCommand != null)
            {
                return _CustomCommand.AddParameter(value, prefix);
            }
            return base.AddParameter(value, prefix);
        }
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
            _SplitInfo = new SplitIndexLength(operate, index, length);
        }
        /// <inheritdoc/>
        internal override int ExecuteImp(DatabaseExecutor executor)
        {
            var command = executor.CreateCommand(_Statement);
            if (_SplitInfo != null)
            {
                int recordsAffectCount = 0;
                _SplitInfo.Operate.Split(_SplitInfo.Index, _SplitInfo.Length, delegate ()
                {
                    recordsAffectCount = ExecuteCommand(command);
                });
                return recordsAffectCount;
            }
            else
            {
                return ExecuteCommand(command);
            }
        }
        private int ExecuteCommand(DbCommand command)
        {
            var operate = Operate;
            if (_CustomCommand != null)
            {
                return _CustomCommand.Execute(command, operate);
            }
            else
            {
                if (Parameters.Count > 0)
                {
                    command.Parameters.AddRange(Parameters.Values.ToArray());
                }
                if (operate.HasResult && operate.Output != null)
                {
                    using (var reader = command.ExecuteReader())
                    {
                        operate.Read(reader);
                        reader.NextResult();
                        return reader.RecordsAffected;
                    }
                }
                else
                {
                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}