// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.Common;
    using System;
    using System.Linq;
    using System.Data.Common;
    using System.Reflection;
    using System.Reflection.Emit;
    public partial class TypeMetadata
    {
        //创建对象（区分普通对象和匿名对象）
        private Func<DbDataReader, int[], object[], object> CreateInstanceMethod;
        /// <summary>
        /// 使用<see cref="DbDataReader"/>中的数据创建当前类型实例。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <param name="fields">字段索引列表。</param>
        /// <param name="complexs">读取值的暂存数组。</param>
        /// <returns></returns>
        public object CreateInstance(DbDataReader reader, int[] fields, object[] complexs)
        {
            if (KeyIndexs != null && reader.IsDBNull(fields[KeyIndexs[0]]))
                return null;
            if (CreateInstanceMethod == null)
            {
                if (ClrType.IsAnonymous())
                    CreateInstanceMethod = (Func<DbDataReader, int[], object[], object>)new CreateAnonymousInstanceGenerator(this).Compile();
                else
                    CreateInstanceMethod = (Func<DbDataReader, int[], object[], object>)new CreateSimpleInstanceGenerator(this).Compile();
            }
            return CreateInstanceMethod(reader, fields, complexs);
        }
        /// <summary>
        /// 创建匿名对象代码生成器。
        /// </summary>
        private sealed class CreateAnonymousInstanceGenerator : MethodILGenerateor<Func<DbDataReader, int[], object[], object>>
        {
            public CreateAnonymousInstanceGenerator(TypeMetadata metadata)
                : base(metadata)
            {
            }

            private LocalBuilder field;

            public override Delegate Compile()
            {
                field = il.DeclareLocal(typeof(int));

                var itemType = Metadata.ClrType;
                var constructor = itemType.GetConstructors()[0];
                var parameters = constructor.GetParameters();

                var primarys = Metadata.PrimaryMembers;
                var complexs = Metadata.ComplexMembers;

                //var indexPrimary = 0;
                var indexComplex = 0;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var primary = primarys.FirstOrDefault(a => a.Member.PropertyType == parameter.ParameterType && a.Member.Name == parameter.Name);
                    if (primary != null)
                    {
                        SimpleProperty(parameter, primary.Member, primarys.IndexOf(primary.Member));
                    }
                    else if (indexComplex < complexs.Count && parameter.ParameterType == complexs[indexComplex].Member.PropertyType)
                    {
                        ComplexProperty(parameter, complexs[indexComplex].Member, indexComplex++);
                    }
                    else
                    {
                        Default(parameter.ParameterType);
                    }
                }

                il.Emit(OpCodes.Newobj, constructor);
                il.Emit(OpCodes.Ret);
                return base.Compile();
            }

            private void SimpleProperty(ParameterInfo parameter, PropertyInfo propertyInfo, int currentIndex)
            {
                Label endLabel = il.DefineLabel();
                Label defaultLabel = il.DefineLabel();
                //var field = fields[index];
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldc_I4, currentIndex);
                il.Emit(OpCodes.Ldelem_I4);
                il.Emit(OpCodes.Stloc, field);
                //if(field >= 0)
                il.Emit(OpCodes.Ldloc, field);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Blt_S, defaultLabel);

                var propertyType = propertyInfo.PropertyType;

                //if(!reader.IsDBNull(field))
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc, field);
                il.Emit(OpCodes.Callvirt, SupportMembers.DataReader.IsDBNull);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brfalse_S, defaultLabel);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc, field);

                if (DataReaderMethodMap.TryGetValue(propertyInfo.PropertyType, out MethodInfo getMethod))
                {
                    il.Emit(OpCodes.Callvirt, getMethod);
                }
                else
                {
                    il.Emit(OpCodes.Callvirt, getItemMethod);
                    il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                }
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    var propetyType = propertyInfo.PropertyType;
                    var constructor = propetyType.GetConstructor(new Type[] { propetyType.GetGenericArguments()[0] });
                    il.Emit(OpCodes.Newobj, constructor);
                }

                il.Emit(OpCodes.Br_S, endLabel);

                il.MarkLabel(defaultLabel);
                Default(parameter.ParameterType);
                il.MarkLabel(endLabel);
            }

            private void ComplexProperty(ParameterInfo parameter, PropertyInfo property, int currentIndex)
            {
                //complexs[index2];
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldc_I4, currentIndex);
                il.Emit(OpCodes.Ldelem_Ref);
                il.Emit(OpCodes.Castclass, property.PropertyType);
            }

            private void Default(Type type)
            {
                if (type.IsValueType)
                {
                    var method = typeof(DefaultGenerator<>).MakeGenericType(type).GetMethod("GetDefault");
                    il.Emit(OpCodes.Call, method);
                }
                else
                    il.Emit(OpCodes.Ldnull);
            }
        }
        /// <summary>
        /// 创建普通对象代码生成器。
        /// </summary>
        private sealed class CreateSimpleInstanceGenerator : MethodILGenerateor<Func<DbDataReader, int[], object[], object>>
        {
            public CreateSimpleInstanceGenerator(TypeMetadata metadata)
                : base(metadata)
            {
            }

            private LocalBuilder item;
            private LocalBuilder field;

            public override Delegate Compile()
            {
                var itemType = Metadata.ClrType;
                item = il.DeclareLocal(itemType);
                field = il.DeclareLocal(typeof(int));

                il.Emit(OpCodes.Newobj, itemType.GetConstructor(new Type[] { }));
                il.Emit(OpCodes.Stloc, item);

                var members = Metadata.PrimaryMembers;
                for (int i = 0; i < members.Count; i++)
                    SimpleProperty(members[i].Member, i);

                var complexs = Metadata.ComplexMembers;
                for (int i = 0; i < complexs.Count; i++)
                    ComplexProperty(complexs[i].Member, i);

                il.Emit(OpCodes.Ldloc, item);
                il.Emit(OpCodes.Ret);
                return base.Compile();
            }

            private void ComplexProperty(PropertyInfo property, int currentIndex)
            {
                //item.Property = complexs[index2];
                il.Emit(OpCodes.Ldloc, item);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldc_I4, currentIndex);
                il.Emit(OpCodes.Ldelem_Ref);
                il.Emit(OpCodes.Castclass, property.PropertyType);
                il.Emit(OpCodes.Callvirt, property.GetSetMethod());
            }

            private void SimpleProperty(PropertyInfo property, int currentIndex)
            {
                Label judgeLabel = il.DefineLabel();
                //var field = fields[index];
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldc_I4, currentIndex);
                il.Emit(OpCodes.Ldelem_I4);
                il.Emit(OpCodes.Stloc, field);
                //if(field >= 0)
                il.Emit(OpCodes.Ldloc, field);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Blt_S, judgeLabel);

                var propertyType = property.PropertyType;
                if (propertyType.IsNullable())
                {
                    //if(!reader.IsDBNull(field))
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldloc, field);
                    il.Emit(OpCodes.Callvirt, SupportMembers.DataReader.IsDBNull);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Brfalse_S, judgeLabel);
                }

                InvokeSet(property);

                il.MarkLabel(judgeLabel);
            }

            private void InvokeSet(PropertyInfo propertyInfo)
            {
                var propertyType = propertyInfo.PropertyType;

                il.Emit(OpCodes.Ldloc, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc, field);

                if (DataReaderMethodMap.TryGetValue(propertyInfo.PropertyType, out MethodInfo getMethod))
                {
                    il.Emit(OpCodes.Callvirt, getMethod);
                }
                else
                {
                    il.Emit(OpCodes.Callvirt, getItemMethod);
                    il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                }
                if (propertyType.IsGenericType
                       && propertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                       )
                {
                    var propetyType = propertyInfo.PropertyType;
                    var constructor = propetyType.GetConstructor(new Type[] { propetyType.GetGenericArguments()[0] });
                    il.Emit(OpCodes.Newobj, constructor);
                }

                il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod(true));
            }
        }
    }
}