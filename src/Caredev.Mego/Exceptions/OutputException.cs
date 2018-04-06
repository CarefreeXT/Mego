// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Exceptions
{
    using Caredev.Mego.Resolve.Outputs;
    using System;
    using System.Linq.Expressions;
    /// <summary>
    /// 翻译异常。
    /// </summary>
    [Serializable]
    public class OutputException : Exception
    {
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="output">当前输出对象。</param>
        public OutputException(OutputInfoBase output)
            : base()
        {
            Output = output;
        }
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="output">当前输出对象。</param>
        /// <param name="message">异常消息。</param>
        public OutputException(OutputInfoBase output, string message)
            : base(message)
        {
            Output = output;
        }
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="output">当前输出对象。</param>
        /// <param name="message">异常消息。</param>
        /// <param name="innerException">内部异常。</param>
        public OutputException(OutputInfoBase output, string message, Exception innerException)
            : base(message, innerException)
        {
            Output = output;
        }
        /// <summary>
        /// 当前翻译的表达式对象。
        /// </summary>
        public OutputInfoBase Output { get; }
    }
}
