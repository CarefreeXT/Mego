// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.ValueConversion
{
    using System;
    using System.Reflection;
    using Res = Properties.Resources;
    /// <summary>
    /// 值转换信息对象。
    /// </summary>
    internal class ConversionInfo
    {
        /// <summary>
        /// 创建转换器信息对象。
        /// </summary>
        /// <param name="conversion">转换器类型。</param>
        public ConversionInfo(Type conversion)
        {
            var objectMethod = conversion.GetMethod("ConvertToObject", BindingFlags.Static | BindingFlags.Public);
            var storageMethod = conversion.GetMethod("ConvertToStorage", BindingFlags.Static | BindingFlags.Public);
            if (objectMethod == null || storageMethod == null)
            {
                throw new ArgumentException(Res.ExceptionInvalidConversion, nameof(conversion));
            }
            var objectArguments = objectMethod.GetParameters();
            var storageArguments = storageMethod.GetParameters();
            if (objectArguments.Length != 1 || storageArguments.Length != 1)
            {
                throw new ArgumentException(Res.ExceptionInvalidConversion, nameof(conversion));
            }
            ObjectType = storageArguments[0].ParameterType;
            StorageType = objectArguments[0].ParameterType;

            ConvertToObject = objectMethod;
            ConvertToStorage = storageMethod;
        }
        /// <summary>
        /// 对象属性数据类型。
        /// </summary>
        public Type ObjectType { get; }
        /// <summary>
        /// 存储数据类型。
        /// </summary>
        public Type StorageType { get; }
        /// <summary>
        /// 向对象的值转换方法。
        /// </summary>
        public MethodInfo ConvertToObject { get; }
        /// <summary>
        /// 向存储的值转换方法。
        /// </summary>
        public MethodInfo ConvertToStorage { get; }
    }
}