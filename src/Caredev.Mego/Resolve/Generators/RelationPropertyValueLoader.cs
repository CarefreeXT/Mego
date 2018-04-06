// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Reflection;
    using Res = Properties.Resources;
    /// <summary>
    /// 关系数据属性值加载器。
    /// </summary>
    internal class RelationPropertyValueLoader : IPropertyValueLoader
    {
        private readonly PropertyValueLoader FirstLoader;
        private readonly PropertyValueLoader SecondLoader;
        /// <summary>
        /// 创建关系数据属性值加载器。
        /// </summary>
        /// <param name="first">关系源属性值加载器。</param>
        /// <param name="second">关系目标属性值加载器。</param>
        public RelationPropertyValueLoader(PropertyValueLoader first, PropertyValueLoader second)
        {
            FirstLoader = first;
            SecondLoader = second;
        }
        /// <summary>
        /// 是否需要反转。
        /// </summary>
        public bool IsReverse { get; set; }
        /// <summary>
        /// 通过指定索引获取值。
        /// </summary>
        /// <param name="index">索引值。</param>
        /// <returns>相应位置的值。</returns>
        public object this[int index]
        {
            get
            {
                if (index < FirstLoader.Length)
                    return FirstLoader[index];
                else
                    return SecondLoader[index - FirstLoader.Length];
            }
        }
        /// <summary>
        /// 根据成员获取所在当前加载器的索引位置。
        /// </summary>
        /// <param name="member">指定成员。</param>
        /// <returns>索引位置。</returns>
        public int IndexOf(MemberInfo member)
        {
            if (FirstLoader.Metadata.ClrType == member.DeclaringType)
                return FirstLoader.IndexOf(member);
            else
                return SecondLoader.IndexOf(member) + FirstLoader.Length;
        }
        /// <summary>
        /// 加载关系数据。
        /// </summary>
        /// <param name="item">关系数据项。</param>
        /// <returns>返回当前加载器。</returns>
        public IPropertyValueLoader Load(object item)
        {
            var relation = item as DbRelationItem;
            if (relation != null)
            {
                if (IsReverse)
                {
                    FirstLoader.Load(relation.Target);
                    if (SecondLoader != null)
                        SecondLoader.Load(relation.Source);
                }
                else
                {
                    FirstLoader.Load(relation.Source);
                    if (SecondLoader != null)
                        SecondLoader.Load(relation.Target);
                }
                return this;
            }
            throw new NotSupportedException(Res.NotSupportedLoadNotRelationItem);
        }
    }
}