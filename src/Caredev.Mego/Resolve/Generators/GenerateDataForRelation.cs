// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    /// <summary>
    /// 用于关系新增或删除的生成数据对象。
    /// </summary>
    public class GenerateDataForRelation : GenerateData
    {
        /// <summary>
        /// 创建数据对象。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="operate">当前操作对象。</param>
        internal GenerateDataForRelation(GenerateContext context, DbRelationOperateBase operate)
            : base(context, operate)
        {
            Items = operate;
            IsAddRelation = operate.Type == EOperateType.AddRelation;

            var navigate = operate.Navigate;
            Source = navigate.Source;
            Target = navigate.Target;
            var firstloader = new PropertyValueLoader(context, Source, Source.Keys);
            var secondloader = new PropertyValueLoader(context, Target, Target.Keys);
            bool isreverse = false;
            if (navigate.IsComposite)
            {
                var composite = (CompositeNavigateMetadata)navigate;
                Table = composite.RelationTable;
            }
            else
            {
                if (navigate.IsForeign == true)
                {
                    Table = navigate.Source;
                }
                else
                {
                    Target = navigate.Source;
                    Source = navigate.Target;

                    firstloader = new PropertyValueLoader(context, Source, Source.Keys);
                    secondloader = new PropertyValueLoader(context, Target, Target.Keys);

                    Table = navigate.Target;
                    isreverse = true;
                }
                if (!IsAddRelation) secondloader = null;
            }
            Loader = new RelationPropertyValueLoader(firstloader, secondloader) { IsReverse = isreverse };
            CommitObject = new CommitObjectFragment(context, Loader);
        }
        /// <summary>
        /// 实际作用表的元数据。
        /// </summary>
        public TableMetadata Table { get; }
        /// <summary>
        /// 关系的源表元数据。
        /// </summary>
        public TableMetadata Source { get; }
        /// <summary>
        /// 关系的目标表元数据。
        /// </summary>
        public TableMetadata Target { get; }
        /// <summary>
        /// 当前关联的数据集合对象。
        /// </summary>
        internal DbRelationOperateBase Items { get; }
        /// <summary>
        /// 属性值加载对象。
        /// </summary>
        public IPropertyValueLoader Loader { get; }
        /// <summary>
        /// 用于数据提交的对象。
        /// </summary>
        public CommitObjectFragment CommitObject { get; }
        /// <summary>
        /// 是否为新建关系操作。
        /// </summary>
        public bool IsAddRelation { get; }
        /// <inheritdoc/>
        public override void Inititalze(DbExpression content = null)
        {
            if (Items.Count == 1)
            {
                foreach (var obj in Items)
                {
                    Loader.Load(obj);
                }
            }
        }
    }
}