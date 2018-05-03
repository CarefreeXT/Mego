// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Providers
{
    using Caredev.Mego.Resolve.Commands;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Reflection;
    using System.Text.RegularExpressions;
    internal class MicrosoftCustomCommand : IMicrosoftCustomCommand
    {
        public MicrosoftCustomCommand(DbProviderFactory factory)
        {
            _Factory = factory;
        }
        private readonly DbProviderFactory _Factory;
        private int currentParameterNameIndex = 0;
        private List<DbParameter> _Parameters = new List<DbParameter>();
        private IPropertyValueLoader _Loader;
        private Dictionary<object, DbParameter> _SimpleParameters = new Dictionary<object, DbParameter>();
        private Dictionary<MemberInfo, ParameterBody> _ParameterBody = new Dictionary<MemberInfo, ParameterBody>();
        private struct ParameterBody
        {
            public DbParameter Parameter;
            public ColumnMetadata Metadata;
            public int Index;
        }
        /// <summary>
        /// 创建参数。
        /// </summary>
        /// <param name="value">参数值。</param>
        /// <param name="prefix">参数名前缀。</param>
        /// <returns>参数对象。</returns>
        private DbParameter CreateParameter(object value, string prefix = "p")
        {
            var parameter = _Factory.CreateParameter();
            parameter.Value = value;
            parameter.ParameterName = prefix + (currentParameterNameIndex++).ToString("X");
            _Parameters.Add(parameter);
            return parameter;
        }
        /// <summary>
        /// 添加普通值参数。
        /// </summary>
        /// <param name="value">参数值。</param>
        /// <param name="prefix">参数前缀。</param>
        /// <returns></returns>
        public string AddParameter(object value, string prefix)
        {
            if (value == null)
            {
                value = DBNull.Value;
            }
            if (_SimpleParameters.TryGetValue(value, out DbParameter para))
            {
                return para.ParameterName;
            }
            para = CreateParameter(value, prefix);
            _SimpleParameters.Add(value, para);
            return para.ParameterName;
        }
        /// <summary>
        /// 添加成员参数。
        /// </summary>
        /// <param name="member"></param>
        /// <param name="loader"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string AddParameter(ColumnMetadata member, IPropertyValueLoader loader, int index)
        {
            if (_ParameterBody.TryGetValue(member.Member, out ParameterBody value))
            {
                return value.Parameter.ParameterName;
            }
            _Loader = loader;
            value.Parameter = CreateParameter(null);
            value.Index = index;
            _ParameterBody.Add(member.Member, value);
            return value.Parameter.ParameterName;
        }
        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="command">指定的命令对象。</param>
        /// <param name="operate">当前操作对象。</param>
        /// <returns>执行后的影响行数。</returns>
        public int Execute(DbCommand command, DbOperateBase operate)
        {
            if (_Parameters.Count > 0)
            {
                command.Parameters.AddRange(_Parameters.ToArray());
            }
            var strs = command.CommandText.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (!(operate is IDbSplitObjectsOperate objects))
            {
                throw new InvalidOperationException();
            }
            var recordsAffectedCount = 0;

            var bodys = _ParameterBody.Values;
            if (operate.HasResult && operate.Output != null)
            {
                var objectsOperate = (DbObjectsOperateBase)operate;
                foreach (var obj in objects)
                {
                    _Loader.Load(obj);
                    foreach (var body in bodys)
                    {
                        body.Parameter.Value = _Loader[body.Index];
                    }
                    var returnCommand = RunSpliteCommand(command, strs, out int affectedCount);
                    using (var reader = returnCommand.ExecuteReader())
                    {
                        objectsOperate.Read(reader, obj);
                        recordsAffectedCount += affectedCount + reader.RecordsAffected;
                    }
                }
            }
            else
            {
                foreach (var obj in objects)
                {
                    _Loader.Load(obj);
                    foreach (var body in bodys)
                    {
                        body.Parameter.Value = _Loader[body.Index];
                    }
                    var returnCommand = RunSpliteCommand(command, strs, out int affectedCount);
                    recordsAffectedCount += affectedCount + returnCommand.ExecuteNonQuery();
                }
            }
            return recordsAffectedCount;
        }
        /// <summary>
        /// 分割SQL语句依次执行命令。
        /// </summary>
        /// <param name="command">命令对象。</param>
        /// <param name="strs">SQL语句集合。</param>
        /// <param name="affectedCount">输出影响行数。</param>
        /// <returns></returns>
        protected virtual DbCommand RunSpliteCommand(DbCommand command, string[] strs, out int affectedCount)
        {
            var recordsAffectedCount = 0;
            if (strs.Length > 1)
            {
                var length = strs.Length - 1;
                for (int i = 0; i < length; i++)
                {
                    command.CommandText = strs[i];
                    recordsAffectedCount += command.ExecuteNonQuery();
                }
                command.CommandText = strs[length];
            }
            affectedCount = recordsAffectedCount;
            return command;
        }

        protected DbCommand Clone(DbCommand command, string content, params DbParameter[] parameters)
        {
            var com = _Factory.CreateCommand();
            com.Transaction = command.Transaction;
            com.Connection = command.Connection;
            com.CommandText = content;
            com.Parameters.AddRange(parameters);
            return com;
        }

        protected DbParameter Clone(DbParameter parameter)
        {
            var para = _Factory.CreateParameter();
            para.ParameterName = parameter.ParameterName;
            para.Value = parameter.Value;
            return para;
        }

        protected DbParameter[] CreateSelect(DbCommand source, out DbCommand result)
        {
            var content = source.CommandText;
            result = Clone(source, content);
            var matchs = Regex.Matches(content, @"\@(?<name>p\w+)");
            List<DbParameter> parameters = new List<DbParameter>();
            foreach (Match m in matchs)
            {
                var name = m.Groups["name"].Value;
                var para = source.Parameters[name];
                if (!parameters.Contains(para))
                {
                    parameters.Add(para);
                    result.Parameters.Add(Clone(para));
                }
            }
            return parameters.ToArray();
        }
        protected void CopyValue(DbCommand target, DbParameter[] parameters)
        {
            var count = target.Parameters.Count;
            for (int i = 0; i < count; i++)
            {
                target.Parameters[i].Value = parameters[i].Value;
            }
        }
    }
    internal interface IMicrosoftCustomCommand : ICustomCommand
    {
        string AddParameter(ColumnMetadata member, IPropertyValueLoader loader, int index);
    }
}
