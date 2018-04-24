// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Fragments
{
    /// <summary>
    /// SQL语句片段对象。
    /// </summary>
    public abstract class SqlFragment : ISqlFragment
    {
        /// <summary>
        /// 创建语句片段对象。
        /// </summary>
        /// <param name="context">语句生成上下文。</param>
        public SqlFragment(GenerateContext context)
        {
            _Context = context;
        }
        /// <summary>
        /// 语句生成上下文。
        /// </summary>
        public GenerateContext Context => _Context;
        private GenerateContext _Context;
        /// <summary>
        /// 有结束符。
        /// </summary>
        public virtual bool HasTerminator => true;
        /// <summary>
        /// 写入SQL语句。
        /// </summary>
        /// <param name="writer">SQL语句写入器。</param>
        public void WriteSql(SqlWriter writer) => _Context.WriteFragment(writer, this);
        /// <summary>
        /// 生成SQL语句。
        /// </summary>
        /// <returns>基本当前片段的完成语句内容。</returns>
        public override string ToString()
        {
            var writer = new SqlWriter(_Context);
            _Context.WriteFragment(writer, this);
            if (HasTerminator)
            {
                _Context.WriteTerminator(writer, this);
            }
            return writer.ToString();
        }
    }
}