// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    using Caredev.Mego.Resolve.ValueGenerates;
    /// <summary>
    /// 内存中生成值特性，通过指定类型的生成器生成值。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class GeneratedMemoryValueAttribute : GeneratedValueBaseAttribute
    {
        /// <summary>
        /// 创建内存中生成值特性。
        /// </summary>
        /// <param name="generatorType">生成器类型，该类型必须实现<see cref="IMemoryValueGenerator"/>接口。</param>
        /// <param name="purpose">生成目的。</param>
        public GeneratedMemoryValueAttribute(Type generatorType, EGeneratedPurpose purpose = EGeneratedPurpose.Insert)
            : base(purpose)
        {
            GeneratorType = generatorType;
        }
        /// <summary>
        /// 值生成器类型。
        /// </summary>
        public Type GeneratorType { get; }
        /// <summary>
        /// 生成选项。
        /// </summary>
        public override EGeneratedOption GeneratedOption => EGeneratedOption.Memory;
    }
}