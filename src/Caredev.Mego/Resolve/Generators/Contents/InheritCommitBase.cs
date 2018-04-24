// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Contents
{
    using System;
    using System.Collections.Generic;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Res = Properties.Resources;
    /// <summary>
    /// 继承的提交数据内容对象。
    /// </summary>
    public abstract class InheritCommitBase : CommitContentBase
    {
        /// <summary>
        /// 创建提交内容对象。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="operate"></param>
        internal InheritCommitBase(GenerateContext context, DbObjectsOperateBase operate)
            : base(context, operate)
        { }
        /// <summary>
        /// 提交相关的表元数据。
        /// </summary>
        public TableMetadata[] Tables
        {
            get
            {
                if (_Metadatas == null)
                {
                    var inherits = Table.InheritSets;
                    _Metadatas = new TableMetadata[inherits.Length + 1];
                    Array.Copy(inherits, _Metadatas, inherits.Length);
                    _Metadatas[inherits.Length] = Table;
                }
                return _Metadatas;
            }
        }
        private TableMetadata[] _Metadatas;
        /// <inheritdoc/>
        public override IEnumerable<ColumnMetadata> ConcurrencyMembers
        {
            get
            {
                if (NeedConcurrencyCheck)
                {
                    foreach (var metadata in Tables)
                    {
                        foreach (var member in metadata.Concurrencys)
                        {
                            yield return member;
                        }
                    }
                }
            }
        }
        /// <inheritdoc/>
        public override void Inititalze(DbExpression content = null)
        {
            if (this.Items.DbSet.Name != null)
            {
                throw new NotSupportedException(Res.NotSupportedInheritSetUsingDbName);
            }
            base.Inititalze(content);
        }
    }
}