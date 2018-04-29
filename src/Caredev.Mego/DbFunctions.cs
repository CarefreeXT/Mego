// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    using System;
    using Res = Properties.Resources;
    /// <summary>
    /// 数据库函数映射。
    /// </summary>
    public static class DbFunctions
    {
        /// <summary>
        /// 对于支持 Identity 插入数据的数据，该函数用于最后插入记录的自增列的值。
        /// </summary>
        /// <returns></returns>
        public static int GetIdentity() => throw new InvalidOperationException(Res.ExceptionDisableInvode);
        /// <summary>
        /// 获取序列的当前值。
        /// </summary>
        /// <param name="name">序列名。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>当前值。</returns>
        public static int SequenceValue(string name, string schema) => throw new InvalidOperationException(Res.ExceptionDisableInvode);
        /// <summary>
        /// 获取序列的下一个值。
        /// </summary>
        /// <param name="name">序列名。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>下一个值。</returns>
        public static int SequenceNext(string name, string schema) => throw new InvalidOperationException(Res.ExceptionDisableInvode);
    }
}