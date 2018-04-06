// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System.Collections.Generic;
    /// <summary>
    /// 当添加数据和关系存在关联时，系统会自动同时提交相关操作，
    /// 该接口用于在插入操作对象中声明同时提交的关系操作对象集合。
    /// </summary>
    internal interface IInsertReferenceRelation
    {
        /// <summary>
        /// 插入数据时同时提交的关系数据。
        /// </summary>
        ICollection<DbRelationOperateBase> Relations { get; }
    }
}