// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.

namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.Common;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;
    /// <summary>
    /// 类型元数据，该类为本系统的操作数据类型实例的核心方法，数据类型的若干属性都会
    /// 按指定规则对应到相应的索引上，所以对属性的操作都会转化为对索引值的操作。
    /// </summary>
    public partial class TypeMetadata : TypeMetadataBase
    {
        private static readonly Dictionary<Type, MethodInfo> DataReaderMethodMap;
        private static readonly MethodInfo getItemMethod = typeof(DbDataReader).GetMethod("get_Item", new[] { typeof(int) });
        /// <summary>
        /// 初始化<see cref="DbDataReader"/>函数映射。
        /// </summary>
        static TypeMetadata()
        {
            DataReaderMethodMap = new Dictionary<Type, MethodInfo>()
            {
                { typeof(bool?),SupportMembers.DataReader.GetBoolean },
                { typeof(byte?),SupportMembers.DataReader.GetByte },
                { typeof(char?),SupportMembers.DataReader.GetChar },
                { typeof(DateTime?),SupportMembers.DataReader.GetDateTime },
                { typeof(decimal?),SupportMembers.DataReader.GetDecimal },
                { typeof(double?),SupportMembers.DataReader.GetDouble },
                { typeof(float?),SupportMembers.DataReader.GetFloat },
                { typeof(Guid?),SupportMembers.DataReader.GetGuid },
                { typeof(short?),SupportMembers.DataReader.GetInt16 },
                { typeof(int?),SupportMembers.DataReader.GetInt32 },
                { typeof(long?),SupportMembers.DataReader.GetInt64 },
                { typeof(bool),SupportMembers.DataReader.GetBoolean },
                { typeof(byte),SupportMembers.DataReader.GetByte },
                { typeof(char),SupportMembers.DataReader.GetChar },
                { typeof(DateTime),SupportMembers.DataReader.GetDateTime },
                { typeof(decimal),SupportMembers.DataReader.GetDecimal },
                { typeof(double),SupportMembers.DataReader.GetDouble },
                { typeof(float),SupportMembers.DataReader.GetFloat },
                { typeof(Guid),SupportMembers.DataReader.GetGuid },
                { typeof(short),SupportMembers.DataReader.GetInt16 },
                { typeof(int),SupportMembers.DataReader.GetInt32 },
                { typeof(long),SupportMembers.DataReader.GetInt64 },
                { typeof(string),SupportMembers.DataReader.GetString },
                //{ typeof(byte[]),SupportMembers.DataReader.GetBytes },
                //{ typeof(char[]),SupportMembers.DataReader.GetChars }
            };
        }
        /// <summary>
        /// 主键索引信息。
        /// </summary>
        public readonly int[] KeyIndexs;
        /// <summary>
        /// 创建类型元数据。
        /// </summary>
        /// <param name="itemType">数据项的CLR类型。</param>
        /// <param name="engine">元数据引擎。</param>
        public TypeMetadata(Type itemType, MetadataEngine engine)
            : base(itemType, engine)
        {
            ComplexMembers = new MemberCollection<MemberMetadata>();
            List<MemberMetadata> primarys = null;
            if (itemType.IsAnonymous())
            {
                var constructor = itemType.GetConstructors()[0];
                var parameters = constructor.GetParameters();
                var query = from a in parameters
                            join b in itemType.GetProperties() on a.ParameterType equals b.PropertyType
                            select b;
                foreach (var property in query)
                {
                    var propertyType = property.PropertyType;
                    if (propertyType.IsComplexCollection())
                        ComplexMembers.Add(new MemberMetadata(property, MemberKind.Collection));
                    else if (propertyType.IsObject())
                        ComplexMembers.Add(new MemberMetadata(property, MemberKind.Object));
                }
                primarys = itemType.GetProperties()
                    .Where(a => a.PropertyType.IsPrimary()).Sort()
                    .Select(a => new MemberMetadata(a, MemberKind.Primary)).ToList();
            }
            else
            {
                foreach (var property in itemType.GetProperties().Sort())
                {
                    var propertyType = property.PropertyType;
                    if (propertyType.IsComplexCollection())
                        ComplexMembers.Add(new MemberMetadata(property, MemberKind.Collection));
                    else if (propertyType.IsObject())
                        ComplexMembers.Add(new MemberMetadata(property, MemberKind.Object));
                }
                var table = engine.TryGetTable(itemType);
                if (table.InheritSets.Length == 0)
                    primarys = table.Members.Select(a => new MemberMetadata(a.Member, MemberKind.Primary)).ToList();
                else
                    primarys = table.InheritSets
                        .SelectMany(a => a.Members)
                        .Union(table.Members).Select(b => new MemberMetadata(b.Member, MemberKind.Primary)).ToList();
                KeyIndexs = new int[table.Keys.Length];
                for (int i = 0; i < KeyIndexs.Length; i++)
                    KeyIndexs[i] = i;
            }
            PrimaryMembers = new MemberCollection<MemberMetadata>(primarys);
        }
        /// <summary>
        /// 基元类型成员集合。
        /// </summary>
        public MemberCollection<MemberMetadata> PrimaryMembers { get; }
        /// <summary>
        /// 复合类型成员集合。
        /// </summary>
        public MemberCollection<MemberMetadata> ComplexMembers { get; }
        /// <summary>
        /// 用于EMIT运行时生成代码的基类。
        /// </summary>
        /// <typeparam name="TMethod">生成的目标方法类型。</typeparam>
        private abstract class MethodILGenerateor<TMethod>
        {
            /// <summary>
            /// 创建IL代码生成器。
            /// </summary>
            /// <param name="metadata"></param>
            public MethodILGenerateor(TypeMetadata metadata)
            {
                Metadata = metadata;
                var type = typeof(TMethod);
                var typedefinition = type.GetGenericTypeDefinition();
                var typeArgus = type.GetGenericArguments();
                Type[] parameterTypes = null;
                if (typedefinition == typeof(Func<,,>) || typedefinition == typeof(Func<,,,>))
                {
                    var returnType = typeArgus[typeArgus.Length - 1];
                    parameterTypes = new Type[typeArgus.Length - 1];
                    Array.Copy(typeArgus, parameterTypes, parameterTypes.Length);
                    Method = new DynamicMethod(GetType().Name, returnType, parameterTypes, true);
                }
                else if (typedefinition == typeof(Action<,>) || typedefinition == typeof(Action<,,>) || typedefinition == typeof(Action<,,,>))
                {
                    parameterTypes = typeArgus;
                    Method = new DynamicMethod(GetType().Name, typeof(void), typeArgus, true);
                }
                if (parameterTypes != null)
                {
#if !NETSTANDARD2_0
                    for (int i = 1; i <= parameterTypes.Length; i++)
                        Method.DefineParameter(i, ParameterAttributes.In, "p" + i.ToString()); 
#endif
                }
                il = Method.GetILGenerator();
            }
            /// <summary>
            /// 动态编译代码。
            /// </summary>
            /// <returns>编译后的委托。</returns>
            public virtual Delegate Compile()
            {
                return Method.CreateDelegate(typeof(TMethod));
            }
            private DynamicMethod Method;
            protected readonly ILGenerator il;
            protected readonly TypeMetadata Metadata;
            [Conditional("DEBUG")]
            protected void WriteLine(Type type)
            {
                var method = typeof(Debug).GetMethod("WriteLine", new Type[] { typeof(object) });
                if (type.IsValueType)
                    il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Call, method);
            }
            [Conditional("DEBUG")]
            protected void WriteLine(LocalBuilder local)
            {
                var method = typeof(Debug).GetMethod("WriteLine", new Type[] { typeof(object) });
                il.Emit(OpCodes.Ldloc, local);
                if (local.LocalType.IsValueType)
                    il.Emit(OpCodes.Box, local.LocalType);
                il.Emit(OpCodes.Call, method);
            }
            [Conditional("DEBUG")]
            protected void WriteLine(string content)
            {
                var method = typeof(Debug).GetMethod("WriteLine", new Type[] { typeof(string) });
                il.Emit(OpCodes.Ldstr, content);
                il.Emit(OpCodes.Call, method);
            }
        }
        /// <summary>
        /// 生成指定类型默认值帮助类。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        private class DefaultGenerator<T>
        {
            /// <summary>
            /// 获取默认值。
            /// </summary>
            /// <returns>目标类型。</returns>
            public static T GetDefault()
            {
                return default(T);
            }
        }
    }
}