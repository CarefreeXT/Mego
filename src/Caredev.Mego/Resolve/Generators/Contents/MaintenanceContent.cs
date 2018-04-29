// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using Caredev.Mego.Resolve.Operates;
    /// <summary>
    /// 结构维护内容对象。
    /// </summary>
    public class MaintenanceContent : OperateContentBase
    {
        /// <summary>
        /// 创建内容对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">操作对象。</param>
        internal MaintenanceContent(GenerateContext context, DbMaintenanceOperateBase operate)
            : base(context, operate)
        {
            Maintenance = operate;
        }
        /// <summary>
        /// 维护操作对象。
        /// </summary>
        public DbMaintenanceOperateBase Maintenance { get; }
    }
}
