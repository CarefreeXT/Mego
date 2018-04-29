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
    /// <summary>
    /// 多个数据库操作命令对象。
    /// </summary>
    internal class MultiOperateCommand : OperateCommandBase, IEnumerable<DbOperateBase>
    {
        private readonly IList<DbOperateBase> operates = new List<DbOperateBase>();
        private Dictionary<DbOperateBase, SplitIndexLength> splitesOperates;
        private readonly StringBuilder commandBuilder = new StringBuilder();
        /// <summary>
        /// 创建命令对象。
        /// </summary>
        /// <param name="context">操作执行上下文</param>
        public MultiOperateCommand(DbOperateContext context)
            : base(context) { }
        /// <inheritdoc/>
        public override bool IsEmpty => operates.Count == 0;
        /// <inheritdoc/>
        internal override int ExecuteImp(DatabaseExecutor executor)
        {
            var command = executor.CreateCommand(commandBuilder.ToString());
            if (Parameters.Count > 0)
            {
                command.Parameters.AddRange(Parameters.Values.ToArray());
            }
            OutputCommand(command);
            var readOperates = operates.Where(operate => operate.HasResult && operate.Output != null).ToArray();
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
                    return reader.RecordsAffected;
                }
            }
            else
            {
                return command.ExecuteNonQuery();
            }
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
                splitesOperates.Add(operateInstance, new SplitIndexLength(operate, index, length));
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
}
