// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.ValueGenerates
{
    using System;
    /// <summary>
    /// 内存值生成器接口。
    /// </summary>
    public interface IMemoryValueGenerator
    {
        /// <summary>
        /// 获取生成的值。
        /// </summary>
        /// <param name="item">当前操作数据对象。</param>
        /// <param name="targetType">生成值的数据类型。</param>
        /// <returns>生成后的值，该值的类型必须是ADO支持或兼容的数据类型。</returns>
        object NextValue(object item, Type targetType);
    }
}