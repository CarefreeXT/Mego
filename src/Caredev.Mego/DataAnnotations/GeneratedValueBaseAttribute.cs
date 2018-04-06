// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using Caredev.Mego.Resolve.ValueGenerates;
    using System;
    /// <summary>
    /// 生成值特性，这也是所有生成值的基类，在一个属性上只能有一个生成值特性。
    /// </summary>
    public abstract class GeneratedValueBaseAttribute : Attribute
    {
        /// <summary>
        /// 创建生成值特性。
        /// </summary>
        /// <param name="purpose">生成值的目的。</param>
        public GeneratedValueBaseAttribute(EGeneratedPurpose purpose)
        {
            GeneratedPurpose = purpose;
        }
        /// <summary>
        /// 值生成目的。
        /// </summary>
        public EGeneratedPurpose GeneratedPurpose { get; }
        /// <summary>
        /// 值生成选项。
        /// </summary>
        public abstract EGeneratedOption GeneratedOption { get; }
    }
}