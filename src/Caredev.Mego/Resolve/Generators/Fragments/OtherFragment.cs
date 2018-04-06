// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Fragments
{
    using System.Collections;
    /// <summary>
    /// WHILE循环语句片段。
    /// </summary>
    public class WhileFragment : SqlFragment
    {
        /// <summary>
        /// 创建WHILE循环。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="condition">循环条件语句。</param>
        public WhileFragment(GenerateContext context, IExpressionFragment condition)
            : base(context)
        {
            Block = new BlockFragment(context);
            Condition = condition;
        }
        /// <summary>
        /// 循环条件语句。
        /// </summary>
        public IExpressionFragment Condition { get; }
        /// <summary>
        /// 循环体语句块。
        /// </summary>
        public BlockFragment Block { get; }
    }
    /// <summary>
    /// 根据操作数据项重复语句块。
    /// </summary>
    public class RepeatBlockFragment : SqlFragment
    {
        /// <summary>
        /// 创建重复语句块。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="items">数据项集合。</param>
        /// <param name="loader">属性值加载器。</param>
        /// <param name="block">重复的语句块。</param>
        public RepeatBlockFragment(GenerateContext context, IEnumerable items, IPropertyValueLoader loader = null, BlockFragment block = null)
           : base(context)
        {
            Block = block ?? new BlockFragment(context);
            Items = items;
            Loader = loader;
        }
        /// <inheritdoc/>
        public override bool HasTerminator => false;
        /// <summary>
        /// 数据项集合。
        /// </summary>
        public IEnumerable Items { get; }
        /// <summary>
        /// 重复的语句块。
        /// </summary>
        public BlockFragment Block { get; }
        /// <summary>
        /// 属性值加载器。
        /// </summary>
        public IPropertyValueLoader Loader { get; }
    }
}