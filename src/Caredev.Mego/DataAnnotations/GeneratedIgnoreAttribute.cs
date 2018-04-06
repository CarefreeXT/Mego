// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using Caredev.Mego.Resolve.ValueGenerates;
    using System;
    /// <summary>
    /// 忽略值生成特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class GeneratedIgnoreAttribute : GeneratedValueBaseAttribute
    {
        /// <summary>
        /// 创建忽略值生成特性。
        /// </summary>
        /// <param name="purpose">生成时的目的。</param>
        public GeneratedIgnoreAttribute(EGeneratedPurpose purpose)
            : base(purpose)
        {
        }
        /// <summary>
        /// 生成选项。
        /// </summary>
        public override EGeneratedOption GeneratedOption => EGeneratedOption.Ignore;
    }
}