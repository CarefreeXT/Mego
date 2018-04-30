// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Providers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections;
    using System.Data.Common;
    using System.Collections.Generic;
    using Caredev.Mego.Resolve.Commands;
    using Caredev.Mego.Resolve.Metadatas;
    using System.Data;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Common;

    /// <summary>
    /// 访问Nuget组件:
    /// </summary>
    internal class OracleAccessProvider : DbAccessProvider
    {
        /// <inheritdoc/>
        public override string ProviderName => "Oracle.ManagedDataAccess.Client";
        /// <inheritdoc/>
        public override EExecutionMode ExecutionMode => EExecutionMode.SingleOperation;
        /// <inheritdoc/>
        public override ICustomCommand CreateCustomCommand()
        {
            return new OracleCustomCommand(this);
        }

        private static bool _IsInitialed = false;
        static void InitialOracleCompnent(Type factoryType)
        {
            if (!_IsInitialed)
            {
                try
                {
                    var dbtypeenum = factoryType.Assembly.GetType("Oracle.ManagedDataAccess.Client.OracleDbType");
                    var type = factoryType.Assembly.GetTypes().Where(a => a.Name == "OraDb_DbTypeTable").FirstOrDefault();
                    if (type != null)
                    {
                        var field = type.GetField("s_table", BindingFlags.NonPublic | BindingFlags.Static);
                        if (field != null)
                        {
                            var s_table = (Hashtable)field.GetValue(null);
                            if (s_table != null)
                            {
                                if (!s_table.ContainsKey(typeof(byte?))) s_table.Add(typeof(byte?), Enum.Parse(dbtypeenum, "Byte"));
                                if (!s_table.ContainsKey(typeof(char?))) s_table.Add(typeof(char?), Enum.Parse(dbtypeenum, "Varchar2"));
                                if (!s_table.ContainsKey(typeof(DateTime?))) s_table.Add(typeof(DateTime?), Enum.Parse(dbtypeenum, "TimeStamp"));
                                if (!s_table.ContainsKey(typeof(short?))) s_table.Add(typeof(short?), Enum.Parse(dbtypeenum, "Int16"));
                                if (!s_table.ContainsKey(typeof(int?))) s_table.Add(typeof(int?), Enum.Parse(dbtypeenum, "Int32"));
                                if (!s_table.ContainsKey(typeof(long?))) s_table.Add(typeof(long?), Enum.Parse(dbtypeenum, "Int64"));
                                if (!s_table.ContainsKey(typeof(float?))) s_table.Add(typeof(float?), Enum.Parse(dbtypeenum, "Single"));
                                if (!s_table.ContainsKey(typeof(double?))) s_table.Add(typeof(double?), Enum.Parse(dbtypeenum, "Double"));
                                if (!s_table.ContainsKey(typeof(decimal?))) s_table.Add(typeof(decimal?), Enum.Parse(dbtypeenum, "Decimal"));
                                if (!s_table.ContainsKey(typeof(TimeSpan?))) s_table.Add(typeof(TimeSpan?), Enum.Parse(dbtypeenum, "IntervalDS"));
                                if (!s_table.ContainsKey(typeof(bool?))) s_table.Add(typeof(bool?), Enum.Parse(dbtypeenum, "Boolean"));
                            }
                        }
                    }
                }
                catch (Exception) { }
                finally
                {
                    _IsInitialed = true;
                }
            }
        }
        internal static void SetArrayBindCount(DbCommand command, int value)
        {
            if (ArrayBindCountProperty == null)
            {
                var type = command.GetType();
                ArrayBindCountProperty = type.GetProperty("ArrayBindCount");
            }
            ArrayBindCountProperty.SetValue(command, value);
        }
        private static PropertyInfo ArrayBindCountProperty;
        internal static void SetArrayBindSize(DbParameter parameter, int[] value)
        {
            if (ArrayBindSizeProperty == null)
            {
                ArrayBindSizeProperty = parameter.GetType().GetProperty("ArrayBindSize");
            }
            ArrayBindSizeProperty.SetValue(parameter, value);
        }
        private static PropertyInfo ArrayBindSizeProperty;


        private class OracleCustomCommand : IOracleCustomCommand
        {
            public OracleCustomCommand(OracleAccessProvider provider)
            {
                _Factory = provider.Factory;
                InitialOracleCompnent(_Factory.GetType());
            }

            private DbProviderFactory _Factory;
            private int currentParameterNameIndex = 0;
            private DbParameter AffectCountParameter = null;
            private IPropertyValueLoader Loader = null;
            private List<DbParameter> Parameters = new List<DbParameter>();
            private List<ParameterBody> MemberParameters = new List<ParameterBody>();
            private List<ParameterBody> ReturnParameters = new List<ParameterBody>();
            private List<DbParameter> SimpleParameters = new List<DbParameter>();
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
                return parameter;
            }
            /// <summary>
            /// 添加数据成员参数。
            /// </summary>
            /// <param name="member">注册成员。</param>
            /// <param name="loader">加载器。</param>
            /// <param name="index">取值索引。</param>
            /// <returns>参数名。</returns>
            public string AddParameter(ColumnMetadata member, IPropertyValueLoader loader = null, int index = -1)
            {
                var para = CreateParameter(null);
                var value = new ParameterBody()
                {
                    Parameter = para,
                    Metadata = member
                };
                if (loader == null)
                {
                    para.Direction = ParameterDirection.Output;
                    ReturnParameters.Add(value);
                }
                else
                {
                    Loader = loader;
                    value.Index = index;
                    MemberParameters.Add(value);
                }
                Parameters.Add(para);
                return para.ParameterName;
            }
            /// <summary>
            /// 添加普通值参数。
            /// </summary>
            /// <param name="value">参数值。</param>
            /// <param name="prefix">参数前缀。</param>
            /// <returns></returns>
            public string AddParameter(object value, string prefix)
            {
                var para = CreateParameter(value, prefix);
                Parameters.Add(para);
                SimpleParameters.Add(para);
                return para.ParameterName;
            }
            /// <summary>
            /// 执行命令。
            /// </summary>
            /// <param name="command">命令对象。</param>
            /// <param name="operate">操作对象。</param>
            /// <returns>影响行数。</returns>
            public int Execute(DbCommand command, DbOperateBase operate)
            {
                if (Parameters.Count > 0)
                {
                    command.Parameters.AddRange(Parameters.ToArray());
                }
                LoadParameter(command, operate);
                var isoutput = operate.HasResult && operate.Output != null;
                if (isoutput)
                {
                    if (ReturnParameters.Count > 0)
                    {
                        var parameters = ReturnParameters.Select(a => a.Parameter).ToArray();
                        var result = command.ExecuteNonQuery();
                        var objectOperates = (DbObjectsOperateBase)operate;
                        objectOperates.Read(parameters);
                        return result;
                    }
                    else
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            operate.Read(reader);
                            return reader.RecordsAffected;
                        }
                    }
                }
                else
                {
                    return command.ExecuteNonQuery();
                }
            }
            //从指定对象加载对象参数
            private void LoadParameter(object item)
            {
                var loader = Loader;
                loader.Load(item);
                foreach (var p in MemberParameters)
                {
                    p.Parameter.Value = loader[p.Index];
                }
            }
            //向参数以数组的形式加载所有数据对象
            private void LoadParameter(DbCommand command, DbOperateBase operate)
            {
                if (MemberParameters.Count > 0)
                {
                    var items = (IDbSplitObjectsOperate)operate;
                    if (items.Count == 1)
                    {
                        foreach (var item in items)
                        {
                            LoadParameter(item);
                        }
                        foreach (var parameter in ReturnParameters)
                        {
                            parameter.Parameter.Value = GetDefaultValue(parameter.Metadata.StorageType);
                            var storageType = parameter.Metadata.StorageType;
                            if (!storageType.IsValueType)
                            {
                                parameter.Parameter.Size = 4000;
                            }
                        }
                    }
                    else
                    {
                        var index = 0;
                        var arrayList = MemberParameters.ToDictionary(a => a.Parameter,
                            a =>
                            {
                                var values = Array.CreateInstance(a.Metadata.StorageType, items.Count);
                                a.Parameter.Value = values;
                                return values;
                            });
                        var loader = Loader;
                        foreach (var item in items)
                        {
                            loader.Load(item);
                            foreach (var p in MemberParameters)
                            {
                                arrayList[p.Parameter].SetValue(Loader[p.Index], index);
                            }
                            index++;
                        }

                        OracleAccessProvider.SetArrayBindCount(command, items.Count);
                        int[] shortArray = null, longArray = null;
                        foreach (var member in ReturnParameters)
                        {
                            var storageType = member.Metadata.StorageType;
                            member.Parameter.Value = Array.CreateInstance(storageType, items.Count);
                            if (storageType.IsValueType)
                            {
                                shortArray = shortArray ?? Utility.Array(items.Count, 38);
                                OracleAccessProvider.SetArrayBindSize(member.Parameter, shortArray);
                            }
                            else
                            {
                                longArray = longArray ?? Utility.Array(items.Count, 4000);
                                OracleAccessProvider.SetArrayBindSize(member.Parameter, longArray);
                            }
                        }
                        foreach (var para in SimpleParameters)
                        {
                            var value = para.Value;
                            var values = Array.CreateInstance(value.GetType(), items.Count);
                            for (int i = 0; i < values.Length; i++)
                            {
                                values.SetValue(value, i);
                            }
                            para.Value = values;
                        }
                    }
                }
            }

            private object GetDefaultValue(Type type)
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
                else if (type == typeof(string))
                {
                    return string.Empty;
                }
                else if (type.IsArray)
                {
                    return Array.CreateInstance(type.GetElementType(), 0);
                }
                return null;
            }
        }
    }

    internal interface IOracleCustomCommand : ICustomCommand
    {

        string AddParameter(ColumnMetadata member, IPropertyValueLoader loader = null, int index = -1);
    }
}