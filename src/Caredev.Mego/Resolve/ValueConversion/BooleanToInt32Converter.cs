// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.ValueConversion
{
    using System;
    using System.Reflection;
    /// <summary>
    /// <see cref="Boolean"/>向<see cref="Int32"/>转换器。
    /// </summary>
    public class BooleanToInt32Converter
    {
        private readonly static object TrueValue = 1;
        private readonly static object FalseValue = 0;
        /// <summary>
        /// 向数据对象转换值。
        /// </summary>
        /// <param name="value">转换值。</param>
        /// <returns>转换结果</returns>
        public static bool ConvertToObject(int value)
        {
            return value != 0;
        }
        /// <summary>
        /// 向存储转换值。
        /// </summary>
        /// <param name="value">转换值。</param>
        /// <returns>转换结果</returns>
        public static object ConvertToStorage(bool value)
        {
            return value ? TrueValue : FalseValue;
        }
    }
}