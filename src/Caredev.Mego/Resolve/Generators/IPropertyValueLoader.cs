// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using System.Reflection;
    /// <summary>
    /// 属性值加载器接口。
    /// </summary>
    public interface IPropertyValueLoader
    {
        /// <summary>
        /// 通过指定索引获取值。
        /// </summary>
        /// <param name="index">索引值。</param>
        /// <returns>相应位置的值。</returns>
        object this[int index] { get; }
        /// <summary>
        /// 加载对象值。
        /// </summary>
        /// <param name="item">加载的对象。</param>
        /// <returns>返回当前对象。</returns>
        IPropertyValueLoader Load(object item);
        /// <summary>
        /// 根据成员获取所在当前加载器的索引位置。
        /// </summary>
        /// <param name="member">指定成员。</param>
        /// <returns>索引位置。</returns>
        int IndexOf(MemberInfo member);
    }
}