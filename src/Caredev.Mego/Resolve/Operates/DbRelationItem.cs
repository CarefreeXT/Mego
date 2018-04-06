// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    /// <summary>
    /// 关系操作的数据项。
    /// </summary>
    public sealed class DbRelationItem
    {
        /// <summary>
        /// 创建关系操作的数据项。
        /// </summary>
        /// <param name="source">源对象。</param>
        /// <param name="target">目标对象。</param>
        public DbRelationItem(object source, object target)
        {
            Source = source;
            Target = target;
        }
        /// <summary>
        /// 源对象。
        /// </summary>
        public object Source { get; }
        /// <summary>
        /// 目标对象。
        /// </summary>
        public object Target { get; }
        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is DbRelationItem)
            {
                var value = (DbRelationItem)obj;
                return value.Source == Source && value.Target == Target;
            }
            return false;
        }
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Source.GetHashCode() ^ Target.GetHashCode();
        }
    }
}