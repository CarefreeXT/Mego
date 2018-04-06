// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    /// <summary>
    /// <see cref="Type"/>扩展方法。
    /// </summary>
    internal static class TypeExtensions
    {
        private readonly static HashSet<Type> PrimaryTypes = new HashSet<Type>()
        {
            typeof(bool), typeof(byte), typeof(DateTime), typeof(decimal), typeof(double),
            typeof(float), typeof(Guid),typeof(short), typeof(int), typeof(long),
            typeof(bool?), typeof(byte?), typeof(DateTime?), typeof(decimal?), typeof(double?),
            typeof(float?), typeof(Guid?),typeof(short?), typeof(int?), typeof(long?),
            typeof(string),typeof(byte[]),typeof(char[])
        };
        private readonly static HashSet<Type> NullableTypes = new HashSet<Type>()
        {
            typeof(bool?), typeof(byte?), typeof(DateTime?), typeof(decimal?), typeof(double?),
            typeof(float?), typeof(Guid?),typeof(short?), typeof(int?), typeof(long?),
            typeof(string),typeof(byte[]),typeof(char[])
        };
        private readonly static Type EnumerableType = typeof(IEnumerable);
        /// <summary>
        /// 是否是内置系统基础数据类型。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果是则返回 true，否则返回 false。</returns>
        public static bool IsPrimary(this Type type)
        {
            return PrimaryTypes.Contains(type);
        }
        /// <summary>
        /// 是否可以为空的类型。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果是则返回 true，否则返回 false。</returns>
        public static bool IsNullable(this Type type)
        {
            return NullableTypes.Contains(type);
        }
        /// <summary>
        /// 是否集合类型。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果是则返回 true，否则返回 false。</returns>
        public static bool IsCollection(this Type type)
        {
            return ElementType(type) != null;
        }
        /// <summary>
        /// 是否集合类型，且项是复杂对象。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果是则返回 true，否则返回 false。</returns>
        public static bool IsComplexCollection(this Type type)
        {
            var elementType = ElementType(type);
            return elementType != null && IsObject(elementType);
        }
        /// <summary>
        /// 是否为对象类型。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果是则返回 true，否则返回 false。</returns>
        public static bool IsObject(this Type type)
        {
            return !EnumerableType.IsAssignableFrom(type) && !type.IsValueType && !PrimaryTypes.Contains(type);
        }
        /// <summary>
        /// 获取集合类型的项类型。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果是则返回 true，否则返回 false。</returns>
        public static Type ElementType(this Type type)
        {
            if (type.IsArray && type.GetArrayRank() == 1)
                return type.GetElementType();
            else if (type.IsGenericType)
            {
                var elementTypes = type.GetGenericArguments();
                if (type.GetGenericTypeDefinition() == typeof(IGrouping<,>))
                    return elementTypes[1];
                else if (elementTypes.Length == 1)
                    return elementTypes[0];
            }
            return null;
        }
        /// <summary>
        /// 是否是匿名类型
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果是则返回 true，否则返回 false。</returns>
        public static bool IsAnonymous(this Type type)
        {
            return type.Namespace == null;
        }
        /// <summary>
        /// 排序属性集合，所有数据对象的属性排序规则按以下优先级
        ///     继承层级 -> 属性名
        /// </summary>
        /// <param name="propertys">属性集合。</param>
        /// <returns>返回排序后的结果。</returns>
        public static IEnumerable<PropertyInfo> Sort(this IEnumerable<PropertyInfo> propertys)
        {
            return propertys.OrderBy(a => a.DeclaringType, InheritTypeComparer.Instance).ThenBy(a => a.Name);
        }
        /// <summary>
        /// 获取枚举或<see cref="Nullable{T}"/>所对应的实际类型
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>返回最终的类型。</returns>
        public static Type GetRealType(this Type type)
        {
            if (type.IsEnum)
            {
                type = type.GetEnumUnderlyingType();
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }
            return type;
        }
#if NET35
        /// <summary>
        /// 获取枚举类型的定义类型。
        /// </summary>
        /// <param name="enumType">枚举类型。</param>
        /// <returns>返回枚举实际定义的类型。</returns>
        public static Type GetEnumUnderlyingType(this Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(Res.ExceptionTypeIsNotEnum, nameof(enumType));
            }
            return Enum.GetUnderlyingType(enumType);
        }
#endif
    }
}