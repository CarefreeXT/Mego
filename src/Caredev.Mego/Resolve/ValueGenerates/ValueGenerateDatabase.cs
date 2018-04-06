// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.ValueGenerates
{
    /// <summary>
    /// 数据库值生成对象。
    /// </summary>
    internal sealed class ValueGenerateDatabase : ValueGenerateBase
    {
        /// <inheritdoc/>
        public override EGeneratedOption GeneratedOption => EGeneratedOption.Database;
    }
}
