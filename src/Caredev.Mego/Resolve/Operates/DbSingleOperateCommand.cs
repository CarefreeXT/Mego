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
        /// <summary>
        /// 是否以块形式执行。
        /// </summary>
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
        internal override int ExecuteImp(DatabaseExecutor executor)
        {
            var command = executor.CreateCommand(_Statement);
            if (Parameters.Count > 0)
            {
                command.Parameters.AddRange(Parameters.Values.ToArray());
            }
            OutputCommand(command);
            var operate = Operate;
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
            return recordsAffectCount;
        }
    }
}
