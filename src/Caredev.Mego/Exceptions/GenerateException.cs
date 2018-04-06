// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Exceptions
{
    using Caredev.Mego.Resolve.Outputs;
    using System;
    using System.Linq.Expressions;
    /// <summary>
    /// 生成代码异常。
    /// </summary>
    [Serializable]
    public class GenerateException : Exception
    {
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        public GenerateException()
            : base()
        {
        }
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public GenerateException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="message">异常消息。</param>
        /// <param name="innerException">内部异常。</param>
        public GenerateException( string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
