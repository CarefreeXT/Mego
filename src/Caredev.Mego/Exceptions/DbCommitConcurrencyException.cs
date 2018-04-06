// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Exceptions
{
    using System;
    /// <summary>
    /// 数据提交并发异常，当预期若干数据对象的提交操作，实际上未影响到
    /// 数据库中指定数量的行数时引发异常。此时表示所提交的若干对象中，
    /// 有数个对象已被并发更新导致当前数据的并发标记不再匹配。
    /// </summary>
    [Serializable]
    public class DbCommitConcurrencyException : Exception
    {
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        public DbCommitConcurrencyException()
            : base()
        {
        }
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public DbCommitConcurrencyException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// 创建异常对象。
        /// </summary>
        /// <param name="message">异常消息。</param>
        /// <param name="innerException">内部异常。</param>
        public DbCommitConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}