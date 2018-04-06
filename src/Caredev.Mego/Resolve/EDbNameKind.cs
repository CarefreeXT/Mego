// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    /// <summary>
    /// 数据库名称种类。
    /// </summary>
    public enum EDbNameKind
    {
        /// <summary>
        /// 架构名加名称组合成的名称，两种都会以安全名称的形式输出，例如[obj].[Customer]。
        /// </summary>
        NameSchema,
        /// <summary>
        /// 仅名称，以安全名称的形式输出，例如[Customer]。
        /// </summary>
        Name,
        /// <summary>
        /// 常量名称，直接输出指定的名称，例如#temp1。
        /// </summary>
        Contact
    }
}