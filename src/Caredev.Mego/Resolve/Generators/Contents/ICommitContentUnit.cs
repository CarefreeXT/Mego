// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.ValueGenerates;
    using System.Collections.Generic;
    /// <summary>
    /// 提交内容单元接口
    /// </summary>
    public interface ICommitContentUnit
    {
        /// <summary>
        /// 生成上下文。
        /// </summary>
        GenerateContext GenerateContext { get; }
        /// <summary>
        /// 需要返回结果的成员。
        /// </summary>
        IEnumerable<CommitMember> ReturnMembers { get; }
        /// <summary>
        /// 所有列元数据。
        /// </summary>
        IEnumerable<ColumnMetadata> Columns { get; }
        /// <summary>
        /// 注册需要返回的成员。
        /// </summary>
        /// <param name="member">指定提交成员。</param>
        /// <returns>返回传入成员。</returns>
        CommitMember ReisterReturnMember(CommitMember member);
        /// <summary>
        /// 获取指定列的值生成对象。
        /// </summary>
        /// <param name="column">指定数据列元数据。</param>
        /// <returns>值生成对象。</returns>
        ValueGenerateBase GetValueGenerator(ColumnMetadata column);
    }
}