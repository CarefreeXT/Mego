// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.ValueGenerates
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// 内存中值生成定义对象。
    /// </summary>
    internal sealed class ValueGenerateMemory : ValueGenerateBase
    {
        private readonly static Dictionary<Type, IMemoryValueGenerator> _Generators
            = new Dictionary<Type, IMemoryValueGenerator>();
        /// <summary>
        /// 创建内存中值生成定义对象。
        /// </summary>
        /// <param name="type">值生成器类型。</param>
        public ValueGenerateMemory(Type type)
            : base()
        {
            if (!_Generators.TryGetValue(type, out IMemoryValueGenerator generator))
            {
                generator = Activator.CreateInstance(type) as IMemoryValueGenerator;
                _Generators.Add(type, generator);
            }
            Generator = generator;
        }
        /// <summary>
        /// 值生成器实例。
        /// </summary>
        public IMemoryValueGenerator Generator { get; }
        /// <inheritdoc/>
        public override EGeneratedOption GeneratedOption => EGeneratedOption.Memory;
    }
}