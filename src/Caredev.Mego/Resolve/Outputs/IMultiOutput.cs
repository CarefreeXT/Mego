// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using System.Collections.Generic;
    using System.Data.Common;
    /// <summary>
    /// 多值输出对象接口。
    /// </summary>
    public interface IMultiOutput
    {
        /// <summary>
        /// 获取执行结果。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <returns>执行结果。</returns>
        IEnumerable<object> GetResult(DbDataReader reader);
    }
}