// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    /// <summary>
    /// 实现该接口的数据列特性，将表示所作用到的数据列将参与乐观并发检查。
    /// </summary>
    public interface IConcurrencyCheck : IColumnAnnotation
    {
    }
}