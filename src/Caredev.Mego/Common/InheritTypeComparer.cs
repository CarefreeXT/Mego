// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Common
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// 类型继承关系比较器。
    /// </summary>
    internal class InheritTypeComparer : IComparer<Type>
    {
        /// <summary>
        /// 比较器当前实例。
        /// </summary>
        public static readonly InheritTypeComparer Instance = new InheritTypeComparer();
        /// <summary>
        /// 单例模式，私有化比较器构造函数。
        /// </summary>
        private InheritTypeComparer() { }
        /// <summary>
        /// 比较两个类型的继承关系，如果类型x是类型y的父级则返回 -1，反之返回 1，相同则返回 0。
        /// </summary>
        /// <param name="x">参与比较的类型。</param>
        /// <param name="y">参与比较的类型。</param>
        /// <returns>相同返回 0，如果 y 继承自 x 返回 1，否则返回 -1。</returns>
        public int Compare(Type x, Type y)
        {
            if (x.Equals(y))
                return 0;
            return x.IsAssignableFrom(y) ? -1 : 1;
        }
    }
}