// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using System.Data.Common;
    /// <summary>
    /// 单个值输出接口。
    /// </summary>
    public interface ISingleOutput
    {
        /// <summary>
        /// 获取执行结果。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <returns>执行结果。</returns>
        object GetResult(DbDataReader reader);
    }
}