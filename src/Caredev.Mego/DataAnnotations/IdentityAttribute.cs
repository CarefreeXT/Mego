// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    using Caredev.Mego.Resolve.ValueGenerates;
    /// <summary>
    /// 标识列值生成特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IdentityAttribute : GeneratedValueBaseAttribute, IColumnAnnotation
    {
        /// <summary>
        /// 创建标识列值生成特性。
        /// </summary>
        /// <param name="seed">标识种子。</param>
        /// <param name="increment">标识增量。</param>
        public IdentityAttribute(int seed = 1, int increment = 1)
            : base(EGeneratedPurpose.Insert)
        {
            Seed = seed;
            Increment = increment;
        }
        /// <summary>
        /// 标识种子。
        /// </summary>
        public int Seed { get; }
        /// <summary>
        /// 标识增量。
        /// </summary>
        public int Increment { get; }
        /// <summary>
        /// 生成选项。
        /// </summary>
        public override EGeneratedOption GeneratedOption => EGeneratedOption.Identity;
    }
}