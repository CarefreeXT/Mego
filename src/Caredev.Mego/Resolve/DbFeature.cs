// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    using Caredev.Mego.Resolve.Operates;
    /// <summary>
    /// 数据库特性描述对象。
    /// </summary>
    public sealed class DbFeature
    {
        /// <summary>
        /// 数据库能力描述。
        /// </summary>
        public EDbCapable Capability { get; set; }
        /// <summary>
        /// 提交数据操作时，对于没有继承自<see cref="DbObjectsOperateBase"/>
        /// 的单个操作所包含参数的最大数量。
        /// </summary>
        public byte MaxParameterCountForOperate { get; set; } = 50;
        /// <summary>
        /// 支持的最大参数数量，为空表示无限数量。
        /// </summary>
        public int? MaxParameterCount { get; set; } = 2098;
        /// <summary>
        /// INSERT VALUES语句中支持的最大行数，为空表示无限行数。
        /// </summary>
        public int? MaxInsertRowCount { get; set; } = 1000;
        /// <summary>
        /// 默认数据架构名。
        /// </summary>
        public string DefaultSchema { get; set; } = string.Empty;
        /// <summary>
        /// 判断数据库是否拥有指定能力。
        /// </summary>
        /// <param name="capable">指定能力。</param>
        /// <returns>拥有返回 true，否则返回 false。</returns>
        public bool HasCapable(EDbCapable capable)
        {
            return (Capability & capable) == capable;
        }
        /// <summary>
        /// 创建当前<see cref="DbFeature"/>的浅表副本。
        /// </summary>
        /// <returns></returns>
        internal DbFeature Clone()
        {
            return (DbFeature)MemberwiseClone();
        }
    }
}