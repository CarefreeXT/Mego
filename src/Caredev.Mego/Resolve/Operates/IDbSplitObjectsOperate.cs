// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    /// <summary>
    /// 实现该接口的对象操作，当操作的对象过多无法一次性提交时，
    /// 可多次分开操作，该接口完成相应操作的数据成员及方法。
    /// </summary>
    internal interface IDbSplitObjectsOperate
    {
        /// <summary>
        /// 在指定函数调用过程中，从指定的索引分割当前操作为指定长度的集合。
        /// </summary>
        /// <param name="index">开始索引。</param>
        /// <param name="length">操作长度。</param>
        /// <param name="action">执行的操作。</param>
        void Split(int index, int length, Action action);
        /// <summary>
        /// 当前操作数量。
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 当前提交每个对象时可能包含的最大参数数量。
        /// </summary>
        int ItemParameterCount { get; }
    }
}