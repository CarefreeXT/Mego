// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using System.Collections.Generic;
    using System.Data.Common;
    /// <summary>
    /// 多个基元值输出信息对象。
    /// </summary>
    public class MultiValueOutputInfo : OutputInfoBase, IMultiOutput
    {
        /// <inheritdoc/>
        public override EOutputType Type => EOutputType.MultiValue;
        /// <summary>
        /// 获取执行结果。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <returns>多个基元值枚举。</returns>
        public IEnumerable<object> GetResult(DbDataReader reader)
        {
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    yield return reader.GetValue(0);
                }
            }
        }
    }
}