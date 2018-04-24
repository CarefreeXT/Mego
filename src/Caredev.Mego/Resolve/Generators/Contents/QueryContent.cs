// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Resolve.Operates;
    /// <summary>
    /// 查询操作内容。
    /// </summary>
    public class QueryContent : OperateContentBase
    {
        /// <summary>
        /// 创建内容对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">操作对象。</param>
        internal QueryContent(GenerateContext context, DbQueryOperateBase operate)
            : base(context, operate)
        {
            Query = operate;
        }
        /// <summary>
        /// 查询对象。
        /// </summary>
        internal DbQueryOperateBase Query { get; }
    }
}