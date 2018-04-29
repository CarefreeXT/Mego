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
    public class GuidToStringConverter
    {
        /// <summary>
        /// 向数据对象转换值。
        /// </summary>
        /// <param name="value">转换值。</param>
        /// <returns>转换结果</returns>
        public static Guid ConvertToObject(string value)
        {
            return new Guid(value);
        }
        /// <summary>
        /// 向存储转换值。
        /// </summary>
        /// <param name="value">转换值。</param>
        /// <returns>转换结果</returns>
        public static object ConvertToStorage(Guid value)
        {
            return value.ToString();
        }
    }
}