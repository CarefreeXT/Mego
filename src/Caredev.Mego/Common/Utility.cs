// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Common
{
    using System;
    /// <summary>
    /// 内部使用工具类对象。
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// 创建指定元素类型的数组对象，并用指定值初始化数据元素。
        /// </summary>
        /// <typeparam name="T">数组元素类型。</typeparam>
        /// <param name="length">数组长度。</param>
        /// <param name="value">初始化的值。</param>
        /// <returns>初始化后的数组。</returns>
        public static T[] Array<T>(int length, T value)
        {
            var result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value;
            }
            return result;
        }
        /// <summary>
        /// 创建指定元素类型的数组对象，并用函数初始化数据元素。
        /// </summary>
        /// <typeparam name="T">数组元素类型。</typeparam>
        /// <param name="length">数组长度。</param>
        /// <param name="valueCreator">初始化值的函数。</param>
        /// <returns></returns>
        public static T[] Array<T>(int length, Func<T> valueCreator)
        {
            NotNull(valueCreator, nameof(valueCreator));

            var result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = valueCreator();
            }
            return result;
        }
        /// <summary>
        /// 检查指定的参数是否为空。
        /// </summary>
        /// <typeparam name="T">检查参数的类型。</typeparam>
        /// <param name="value">参数值。</param>
        /// <param name="parameterName">参数名。</param>
        /// <returns>如果参数不为空则返回参数值。</returns>
        /// <exception cref="ArgumentNullException">当参数为空时抛出异常。</exception>
        public static T NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }
        /// <summary>
        /// 检查指定的参数是否为空，该参数类型为<see cref="Nullable{T}"/>的泛型类型。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value">参数值。</param>
        /// <param name="parameterName">参数名。</param>
        /// <returns>如果参数不为空则返回参数值。</returns>
        /// <exception cref="ArgumentNullException">当参数为空时抛出异常。</exception>
        public static T? NotNull<T>(T? value, string parameterName) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }
    }
}