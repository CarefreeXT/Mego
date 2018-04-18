// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using Caredev.Mego.Resolve;
    using System;
    /// <summary>
    /// 数据关系操作行为特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RelationActionAttribute : Attribute
    {
        /// <summary>
        /// 创建行为特性。
        /// </summary>
        /// <param name="update">更新时行为。</param>
        /// <param name="delete">删除时行为。</param>
        public RelationActionAttribute(EReferenceAction? update, EReferenceAction? delete)
        {
            Update = update;
            Delete = delete;
        }
        /// <summary>
        /// 更新时行为。
        /// </summary>
        public EReferenceAction? Update { get; }
        /// <summary>
        /// 删除时行为。
        /// </summary>
        public EReferenceAction? Delete { get; }
    }
}
