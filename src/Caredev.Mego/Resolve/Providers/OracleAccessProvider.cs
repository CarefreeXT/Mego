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
                InitialOracleCompnent(type);
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
            }
            
            private DbProviderFactory _Factory;
            private int currentParameterNameIndex = 0;
            private DbParameter AffectCountParameter = null;
            private IPropertyValueLoader Loader = null;
            private List<DbParameter> SpecialParameters;
            private Dictionary<MemberInfo, ParameterBody> MemberParameters;
            private Dictionary<MemberInfo, ParameterBody> ReturnParameters;
            private Dictionary<object, DbParameter> SimpleParameters = new Dictionary<object, DbParameter>();

            /// <summary>
            /// 创建参数。
            /// </summary>
            /// <param name="value">参数值。</param>
            /// <returns>参数对象。</returns>
            private DbParameter CreateParameter(object value, string prefix = "p")
            {
                var parameter = _Factory.CreateParameter();
                parameter.Value = value;
                parameter.ParameterName = prefix + (currentParameterNameIndex++).ToString("X");
                return parameter;
            }
            /// <summary>
            /// 注册提交数据的成员参数。
            /// </summary>
            /// <param name="member">注册成员。</param>
            /// <param name="loader">加载器。</param>
            /// <param name="index">取值索引。</param>
            /// <returns>参数名。</returns>
            public string RegisterParameter(ColumnMetadata member, IPropertyValueLoader loader = null, int index = -1)
            {
                if (SpecialParameters == null)
                {
                    SpecialParameters = new List<DbParameter>();
                    MemberParameters = new Dictionary<MemberInfo, ParameterBody>();
                    ReturnParameters = new Dictionary<MemberInfo, ParameterBody>();
                }

                if (loader == null)
                {
                    if (!ReturnParameters.TryGetValue(member.Member, out ParameterBody value))
                    {
                        var para = CreateParameter(null);
                        para.Direction = ParameterDirection.Output;
                        value.Parameter = para;
                        value.Metadata = member;
                        ReturnParameters.Add(member.Member, value);
                    }
                    return value.Parameter.ParameterName;
                }
                else
                {
                    Loader = loader;
                    if (!MemberParameters.TryGetValue(member.Member, out ParameterBody value))
                    {
                        var para = CreateParameter(null);
                        para.Direction = ParameterDirection.Input;
                        value.Parameter = para;
                        value.Metadata = member;
                        value.Index = index;
                        MemberParameters.Add(member.Member, value);
                    }
                    return value.Parameter.ParameterName;
                }
            }

            public string AddParameter(object value, string prefix)
            {
                if (!SimpleParameters.TryGetValue(value, out DbParameter dbParameter))
                {
                    var para = CreateParameter(value, prefix);
                    SpecialParameters.Add(para);
                    dbParameter = para;
                    para.Direction = ParameterDirection.Input;
                    SimpleParameters.Add(value, para);
                }
                return dbParameter.ParameterName;
            }
            /// <summary>
            /// 获取参数名。
            /// </summary>
            /// <param name="member">成员信息对象。</param>
            /// <param name="isreturn">是否为返回参数。</param>
            /// <returns>参数名。</returns>
            public string GetParameter(MemberInfo member, bool isreturn)
            {
                DbParameter para;
                if (isreturn)
                {

                    para = ReturnParameters[member].Parameter;
                }
                else
                {
                    para = MemberParameters[member].Parameter;
                }
                if (!SpecialParameters.Contains(para))
                {
                    SpecialParameters.Add(para);
                }
                return para.ParameterName;
            }
            private struct ParameterBody
            {
                public DbParameter Parameter;
                public ColumnMetadata Metadata;
                public int Index;
            }
            public int Execute(DbCommand command, DbOperateBase operate)
            {
                if (SpecialParameters.Count > 0)
                {
                    command.Parameters.AddRange(SpecialParameters.ToArray());
                }
                LoadParameter(command, operate);
                var isoutput = operate.HasResult && operate.Output != null;
                if (isoutput)
                {
                    var parameters = ReturnParameters.Values.Select(a => a.Parameter).ToArray();
                    if (parameters.Length > 0)
                    {
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
                foreach (var p in MemberParameters.Values)
                {
                    p.Parameter.Value = loader[p.Index];
                }
            }
            //向参数以数组的形式加载所有数据对象
            private void LoadParameter(DbCommand command, DbOperateBase operate)
            {
                if (MemberParameters != null)
                {
                    var items = (IDbSplitObjectsOperate)operate;
                    if (items.Count == 1)
                    {
                        foreach (var item in items)
                        {
                            LoadParameter(item);
                        }
                        foreach (var parameter in ReturnParameters.Values)
                        {
                            parameter.Parameter.Value = GetDefaultValue(parameter.Metadata.StorageType);
                        }
                    }
                    else
                    {
                        var memberPrameters = MemberParameters.Values.ToArray();
                        var index = 0;
                        var arrayList = memberPrameters.ToDictionary(a => a.Parameter,
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
                            foreach (var p in memberPrameters)
                            {
                                arrayList[p.Parameter].SetValue(Loader[p.Index], index);
                            }
                            index++;
                        }

                        OracleAccessProvider.SetArrayBindCount(command, items.Count);
                        int[] shortArray = null, longArray = null;
                        foreach (var member in ReturnParameters.Values)
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
                        foreach (var para in SimpleParameters.Values)
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

        string RegisterParameter(ColumnMetadata member, IPropertyValueLoader loader = null, int index = -1);

        string GetParameter(MemberInfo member, bool isreturn);
    }
}