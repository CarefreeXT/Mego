// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// Microsoft 本地数据库语句片段写入器。
    /// </summary>
    public abstract partial class MSLocalDbFragmentWriter : FragmentWriterBase
    {
        /// <summary>
        /// 创建 Microsoft 本地数据库语句片段写入器。
        /// </summary>
        /// <param name="generator">片段生成器。</param>
        public MSLocalDbFragmentWriter(SqlGeneratorBase generator)
            : base(generator)
        {
        }
        /// <inheritdoc/>
        public override void WriteDbName(SqlWriter writer, string name)
        {
            writer.Write('[');
            writer.Write(name);
            writer.Write(']');
        }
        /// <inheritdoc/>
        public override void WriteTerminator(SqlWriter writer, ISqlFragment fragment) { }
        /// <inheritdoc/>
        protected override void WriteFragmentForBlock(SqlWriter writer, ISqlFragment fragment)
        {
            var block = (BlockFragment)fragment;
            foreach (var item in block)
            {
                item.WriteSql(writer);
                if (item.HasTerminator)
                {
                    writer.Write(';');
                }
            }
        }
    }
}