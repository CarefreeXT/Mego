// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Commands
{
    using Caredev.Mego.Resolve.Operates;
    using System.Data.Common;
    /// <summary>
    /// 自定义命令接口，对于特定场景下数据提供
    /// 程序需要指定的数据提交逻辑。
    /// </summary>
    public interface ICustomCommand
    {
        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="command">指定的命令对象。</param>
        /// <param name="operate">当前操作对象。</param>
        /// <returns>执行后的影响行数。</returns>
        int Execute(DbCommand command, DbOperateBase operate);
        /// <summary>
        /// 添加普通参数。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="prefix">参数前级</param>
        /// <returns>参数名。</returns>
        string AddParameter(object value, string prefix);
    }
}