﻿// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.Outputs;
    using Caredev.Mego.Resolve.ValueGenerates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// 用于提交数据的内容对象基类。
    /// </summary>
    public abstract class ContentUnitBase : ContentBase, IContentUnit
    {
        /// <summary>
        /// 创建内容对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">操作对象。</param>
        internal ContentUnitBase(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        {
        }
        /// <summary>
        /// 提交的数据单元。
        /// </summary>
        public CommitUnitBase Unit { get; protected set; }
        /// <summary>
        /// 提交数据的列元数据集合。
        /// </summary>
        public IEnumerable<ColumnMetadata> Columns => Table.Members;
        /// <summary>
        /// 根据列元数据获取相应的值生成对象。
        /// </summary>
        /// <param name="column">列元数据。</param>
        /// <returns>值生成对象。</returns>
        public abstract ValueGenerateBase GetValueGenerator(ColumnMetadata column);
        /// <summary>
        /// 需要返回结果的成员集合。
        /// </summary>
        public IEnumerable<CommitMember> ReturnMembers => _ReturnMembers;
        private List<CommitMember> _ReturnMembers = new List<CommitMember>();
        /// <summary>
        /// 向当前内容对象注册需要返回的成员。
        /// </summary>
        /// <param name="member">注册的成员对象。</param>
        /// <returns>注册对象。</returns>
        public CommitMember ReisterReturnMember(CommitMember member)
        {
            if (Items.HasResult)
            {
                _ReturnMembers.Add(member);
            }
            return member;
        }
    }
}