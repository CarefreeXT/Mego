// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Exceptions
{
    using System;
    /// <summary>
    /// 在查找 SQL 代码生成器时没有找到时会抛出该异常。
    /// </summary>
    [Serializable]
    public class GeneratorNotFoundException : Exception
    {
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        public GeneratorNotFoundException()
            : base()
        { }
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public GeneratorNotFoundException(string message)
            : base(message)
        { }
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="message">异常消息。</param>
        /// <param name="innerException">内部异常。</param>
        public GeneratorNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}