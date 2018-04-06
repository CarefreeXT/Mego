// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego
{
    using System;
    using Res = Properties.Resources;
    /// <summary>
    /// 数据库函数映射。
    /// </summary>
    public static class DbFunctions
    {
        /// <summary>
        /// 对于支持 Identity 插入数据的数据，该函数用于最后插入记录的自增列的值。
        /// </summary>
        /// <returns></returns>
        public static int GetIdentity()
        {
            throw new InvalidOperationException(Res.ExceptionDisableInvode);
        }
    }
}