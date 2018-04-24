// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using Caredev.Mego.Resolve.ValueGenerates;

namespace Caredev.Mego.DataAnnotations
{
    /// <summary>
    /// 使用序列生成值。
    /// </summary>
    public sealed class SequenceAttribute : GeneratedExpressionAttribute
    {
        /// <summary>
        /// 创建序列生成特性。
        /// </summary>
        /// <param name="name">序列名。</param>
        /// <param name="purpose">生成目的。</param>
        public SequenceAttribute(string name, EGeneratedPurpose purpose = EGeneratedPurpose.Insert)
            : base(purpose)
        {
        }
        /// <summary>
        /// 序列名。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 序列架构名。
        /// </summary>
        public string Schema { get; set; }
    }
}
