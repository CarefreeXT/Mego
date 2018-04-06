// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Exceptions
{
    using System;
    using System.Linq.Expressions;
    /// <summary>
    /// 翻译异常。
    /// </summary>
    [Serializable]
    public class TranslateException : Exception
    {
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="exp">当前翻译的表达式对象。</param>
        public TranslateException(Expression exp)
            : base()
        {
            Expression = exp;
        }
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="exp">当前翻译的表达式对象。</param>
        /// <param name="message">异常消息。</param>
        public TranslateException(Expression exp, string message)
            : base(message)
        {
            Expression = exp;
        }
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="exp">当前翻译的表达式对象。</param>
        /// <param name="message">异常消息。</param>
        /// <param name="innerException">内部异常。</param>
        public TranslateException(Expression exp, string message, Exception innerException)
            : base(message, innerException)
        {
            Expression = exp;
        }
        /// <summary>
        /// 当前翻译的表达式对象。
        /// </summary>
        public Expression Expression { get; }
    }
}
