// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.ValueConversion;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    internal static class TypeEmitHelper
    {
        static TypeEmitHelper()
        {
            ValueConvertMethod = typeof(TypeEmitHelper).GetMethod(nameof(ValueConvert),
                BindingFlags.NonPublic | BindingFlags.Static);
        }
        private readonly static MethodInfo ValueConvertMethod;
        private static T ValueConvert<T>(object obj)
        {
            var type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }
            return (T)Convert.ChangeType(obj, type);
        }

        //var field = fields[index];
        public static void GetFieldByIndex(this ILGenerator il, LocalBuilder field, int index, int argIndex = 1)
        {
            il.LoadArgument(argIndex);
            il.Emit(OpCodes.Ldc_I4, index);
            il.Emit(OpCodes.Ldelem_I4);
            il.Emit(OpCodes.Stloc, field);
        }
        //if (field >= 0)
        public static void IfFieldGreaterEqualZero(this ILGenerator il, LocalBuilder field, ref Label judge)
        {
            il.Emit(OpCodes.Ldloc, field);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Blt_S, judge);
        }
        //if(!reader.IsDBNull(field))
        public static void IfReaderIsDbNull(this ILGenerator il, Type type, LocalBuilder field, ref Label judge, int argIndex = 0)
        {
            if (type.IsNullable())
            {
                il.LoadArgument(argIndex);
                il.Emit(OpCodes.Ldloc, field);
                il.Emit(OpCodes.Callvirt, SupportMembers.DataReader.IsDBNull);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brfalse_S, judge);
            }
        }
        //reader.GetValue(field)
        public static void ReaderGetValue(this ILGenerator il, TypeMetadata metadata, PropertyInfo property, LocalBuilder field, int argIndex = 0)
        {
            var autoconvert = metadata.Engine.AutoTypeConversion;

            var type = property.PropertyType;
            var isConvert = metadata.Engine.TryGetConversion(type, out ConversionInfo info);
            if (isConvert)
            {
                type = info.StorageType;
            }

            il.LoadArgument(argIndex);
            il.Emit(OpCodes.Ldloc, field);

            if (autoconvert)
            {
                il.Emit(OpCodes.Callvirt, SupportMembers.DataReader.GetValue);
                var method = ValueConvertMethod.MakeGenericMethod(type);
                il.Emit(OpCodes.Call, method);
            }
            else
            {
                if (TypeMetadata.DataReaderMethodMap.TryGetValue(type, out MethodInfo getMethod))
                {
                    il.Emit(OpCodes.Callvirt, getMethod);
                }
                else
                {
                    il.Emit(OpCodes.Callvirt, SupportMembers.DataReader.GetValue);
                    il.Emit(OpCodes.Castclass, type);
                }
                if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    var constructor = type.GetConstructor(new Type[] { type.GetGenericArguments()[0] });
                    il.Emit(OpCodes.Newobj, constructor);
                }
            }
            if (isConvert)
            {
                il.Emit(OpCodes.Call, info.ConvertToObject);
            }
        }
        //values[field]
        public static void ArrayGetValue(this ILGenerator il, TypeMetadata metadata, PropertyInfo property, LocalBuilder field, int argIndex = 0)
        {
            var type = property.PropertyType;
            var isConvert = metadata.Engine.TryGetConversion(type, out ConversionInfo info);
            if (isConvert)
            {
                type = info.StorageType;
            }

            il.LoadArgument(argIndex);
            il.Emit(OpCodes.Ldloc, field);
            il.Emit(OpCodes.Ldelem_Ref);
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
            if (type.IsGenericType
                   && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                   )
            {
                var constructor = type.GetConstructor(new Type[] { type.GetGenericArguments()[0] });
                il.Emit(OpCodes.Newobj, constructor);
            }
            if (isConvert)
            {
                il.Emit(OpCodes.Call, info.ConvertToObject);
            }
        }
        //item.Property = ?
        public static void PropertySetValue(this ILGenerator il, PropertyInfo property, LocalBuilder item, Action action)
        {
            il.Emit(OpCodes.Ldloc, item);
            action();
            il.Emit(OpCodes.Callvirt, property.GetSetMethod(true));
        }

        private static void LoadArgument(this ILGenerator il, int index)
        {
            switch (index)
            {
                case 0: il.Emit(OpCodes.Ldarg_0); break;
                case 1: il.Emit(OpCodes.Ldarg_1); break;
                case 2: il.Emit(OpCodes.Ldarg_2); break;
                case 3: il.Emit(OpCodes.Ldarg_3); break;
                default: throw new InvalidOperationException();
            }
        }

        public static void ConvertToObject(this ILGenerator il, TypeMetadata metadata, PropertyInfo property)
        {

        }
    }
}