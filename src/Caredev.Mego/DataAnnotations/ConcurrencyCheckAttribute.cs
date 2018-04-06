// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    /// <summary>
    /// 指定属性参与乐观并发检查。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConcurrencyCheckAttribute : Attribute, IConcurrencyCheck
    {
    }
}