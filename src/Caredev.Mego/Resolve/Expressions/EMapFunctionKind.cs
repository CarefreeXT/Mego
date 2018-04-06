// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    /// <summary>
    /// 函数映射种类。
    /// </summary>
    public enum EMapFunctionKind
    {
        /// <summary>
        /// 函数（通常对应到数据库标题函数或存储过程等）。
        /// </summary>
        Function,
        /// <summary>
        /// 过程（通常对应到数据库存储过程）。
        /// </summary>
        Action
    }
}
