// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using System.Data.Common;
    /// <summary>
    /// 单个值输出信息对象。
    /// </summary>
    public class SingleValueOutputInfo : OutputInfoBase, ISingleOutput
    {
        /// <inheritdoc/>
        public override EOutputType Type => EOutputType.SingleValue;
        /// <summary>
        /// 获取结果。
        /// </summary>
        /// <param name="reader">数据读取对象。</param>
        /// <returns>返回读取的对象。</returns>
        public object GetResult(DbDataReader reader)
        {
            reader.Read();
            return reader.IsDBNull(0) ? null : reader.GetValue(0);
        }
    }
}