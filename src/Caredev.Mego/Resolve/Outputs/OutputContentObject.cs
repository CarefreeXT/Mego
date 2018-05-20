// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using System.Collections.Generic;
    /// <summary>
    /// 输出内容项，用于输出对象时产生的中间对象。
    /// </summary>
    public class OutputContentObject : IOutputContent
    {
        /// <summary>
        /// 当前输出内容。
        /// </summary>
        public object Content { get; set; }
        /// <summary>
        /// 当前输出的集合成员。
        /// </summary>
        public Dictionary<ComplexOutputInfo, IOutputContent> Members
            => _Members ?? (_Members = new Dictionary<ComplexOutputInfo, IOutputContent>());
        private Dictionary<ComplexOutputInfo, IOutputContent> _Members;
    }
}