// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    /// <summary>
    /// ADO.NET命令对象的执行模式，由于受限于具体的ADO.NET实现，区别不
    /// 同数据源的执行命令的方式。
    /// </summary>
    public enum EExecutionMode
    {
        /// <summary>
        /// 合并操作模式，一次可以执行任意数量的语句。
        /// </summary>
        MergeOperations,
        /// <summary>
        /// 单个操作模式，一次可以执行一个操作相关的语句或语句块。
        /// </summary>
        SingleOperation,
        /// <summary>
        /// 单语句模式，一次只能执行一条语句。
        /// </summary>
        SingleStatement,
    }
}
