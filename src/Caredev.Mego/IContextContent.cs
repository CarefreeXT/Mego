// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    /// <summary>
    /// 实现该接口的对象可以提供当前数据上下文的访问属性。
    /// </summary>
    public interface IContextContent
    {
        /// <summary>
        /// 数据上下文对象。
        /// </summary>
        DbContext Context { get; }
    }
}