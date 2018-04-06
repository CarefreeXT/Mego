// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using Caredev.Mego.Resolve.ValueGenerates;
    using System;
    /// <summary>
    /// 数据库生成值特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class GeneratedValueAttribute : GeneratedValueBaseAttribute
    {
        /// <summary>
        /// 创建数据库生成值特性。
        /// </summary>
        /// <param name="purpose">生成目的。</param>
        public GeneratedValueAttribute(EGeneratedPurpose purpose = EGeneratedPurpose.Insert)
            : base(purpose)
        {
        }
        /// <summary>
        /// 生成选项。
        /// </summary>
        public override EGeneratedOption GeneratedOption => EGeneratedOption.Database;
    }
}