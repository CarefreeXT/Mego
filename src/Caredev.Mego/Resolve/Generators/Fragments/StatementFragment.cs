// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Fragments
{
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    /// <summary>
    /// 查询语句片段基类。
    /// </summary>
    public abstract class QueryBaseFragment : SourceFragment
    {
        /// <summary>
        /// 创建查询语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        public QueryBaseFragment(GenerateContext context)
            : base(context)
        {
        }
        /// <summary>
        /// 查询前N条数据。
        /// </summary>
        public int Take
        {
            get { return _Take; }
            set
            {
                if (value > 0)
                {
                    if (_Take == 0)
                        _Take = value;
                    else if (_Take > value)
                        _Take = value;
                }
            }
        }
        private int _Take = 0;
        /// <summary>
        /// 查询WHERE条件。
        /// </summary>
        public ILogicFragment Where { get; set; }
        /// <summary>
        /// 查询排序语句集合。
        /// </summary>
        public IList<IExpressionFragment> Sorts { get; } = new List<IExpressionFragment>();
        /// <summary>
        /// 查询数据源集合。
        /// </summary>
        public IEnumerable<ISourceFragment> Sources => _Sources;
        private List<ISourceFragment> _Sources = new List<ISourceFragment>();
        /// <summary>
        /// 添加指定数据源语句。
        /// </summary>
        /// <param name="sources">添加的多个数据源语句。</param>
        /// <returns>返回当前对象。</returns>
        public QueryBaseFragment AddSource(params ISourceFragment[] sources)
#if NET35
            => AddSourceImp(sources.OfType<ISourceFragment>());
#else
            => AddSourceImp(sources);
#endif
        /// <summary>
        /// 添加指定数据源语句。
        /// </summary>
        /// <param name="sources">添加的多个数据源语句。</param>
        /// <returns>返回当前对象。</returns>
        public QueryBaseFragment AddSource(IEnumerable<ISourceFragment> sources) => AddSourceImp(sources);
        //添加数据源实现。
        private QueryBaseFragment AddSourceImp(IEnumerable<ISourceFragment> sources)
        {
            _Sources.AddRange(sources);
            foreach (var source in sources)
            {
                source.Parent = this;
            }
            return this;
        }
    }
    /// <summary>
    /// SELECT查询语句片段。
    /// </summary>
    public class SelectFragment : QueryBaseFragment
    {
        /// <summary>
        /// 创建SELECT查询语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="sources">查询数据源语集合。</param>
        public SelectFragment(GenerateContext context, params ISourceFragment[] sources)
            : base(context)
        {
            AddSource(sources);
        }
        /// <summary>
        /// 是否去除重复。
        /// </summary>
        public bool Distinct { get; set; }
        /// <summary>
        /// 跳过N条查询数据。
        /// </summary>
        public int Skip
        {
            get { return _Skip; }
            set
            {
                if (value > 0) _Skip += value;
            }
        }
        private int _Skip = 0;
        /// <summary>
        /// 查询分组语句。
        /// </summary>
        public IList<ISqlFragment> GroupBys { get; } = new List<ISqlFragment>();
        /// <summary>
        /// 是否强制锁定当前语句。
        /// </summary>
        public bool IsForceLock => Skip > 0 || Take > 0 || Distinct || GroupBys.Count > 0;
        /// <summary>
        /// 是否建议锁定当前语句。
        /// </summary>
        public bool IsRecommandLock => IsForceLock || Sorts.Count > 0;
    }
    /// <summary>
    /// 插入语句片段。
    /// </summary>
    public class InsertFragment : SourceFragment
    {
        /// <summary>
        /// 创建插入语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="target">插入目标语句。</param>
        /// <param name="query">用于插入的查询语句。</param>
        public InsertFragment(GenerateContext context, SourceFragment target, SourceFragment query)
            : base(context)
        {
            Target = target;
            Query = query;
        }
        /// <summary>
        /// 用于插入的查询语句。
        /// </summary>
        public SourceFragment Query { get; }
        /// <summary>
        /// 插入目标语句。
        /// </summary>
        public SourceFragment Target { get; }
    }
    /// <summary>
    /// 插入值语句片段。
    /// </summary>
    public class InsertValueFragment : SourceFragment
    {
        /// <summary>
        /// 创值插入值语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="target">插入目标语句。</param>
        /// <param name="source">用于插入的提交对象语句块。</param>
        /// <param name="data">插入的数据集合。</param>
        public InsertValueFragment(GenerateContext context, ISourceFragment target, CommitObjectFragment source, IEnumerable data)
            : base(context)
        {
            Target = target;
            Data = data;
            Source = source;
            _KeyValues = new Dictionary<ColumnMetadata, ISqlFragment>();
        }
        /// <summary>
        /// 插入目标语句。
        /// </summary>
        public ISourceFragment Target { get; }
        /// <summary>
        /// 用于插入的提交对象语句块。
        /// </summary>
        public CommitObjectFragment Source { get; }
        /// <summary>
        /// 插入的数据集合。
        /// </summary>
        public IEnumerable Data { get; }
        /// <summary>
        /// 插入值语句集合。
        /// </summary>
        public IEnumerable<ISqlFragment> Values => _KeyValues.Values;
        private readonly Dictionary<ColumnMetadata, ISqlFragment> _KeyValues;
        /// <summary>
        /// 设置当前要插入的值。
        /// </summary>
        /// <param name="member">更新成员。</param>
        /// <param name="fragment">更新语句。</param>
        public void SetValue(ColumnMetadata member, ISqlFragment fragment = null)
        {
            if (fragment == null)
            {
                fragment = Source.GetMember(member);
            }

            if (!_KeyValues.ContainsKey(member))
            {
                this.CreateMember(null, member);
                _KeyValues.Add(member, fragment);
            }
            else
            {
                _KeyValues[member] = fragment;
            }
        }
    }
    /// <summary>
    /// 更新语句片段。
    /// </summary>
    public class UpdateFragment : QueryBaseFragment
    {
        /// <summary>
        /// 创建更新语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="target">更新目标语句。</param>
        public UpdateFragment(GenerateContext context, ISourceFragment target)
            : base(context)
        {
            Target = target;
            _KeyValues = new Dictionary<ColumnMetadata, ISqlFragment>();
        }
        /// <summary>
        /// 创建更新语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="metadata">更新目标表元数据。</param>
        /// <param name="name">更新目标名称。</param>
        public UpdateFragment(GenerateContext context, TableMetadata metadata, DbName name = null)
            : this(context, new TableFragment(context, metadata, name))
        { }
        /// <summary>
        /// 更新目标语句。
        /// </summary>
        public ISourceFragment Target { get; }
        /// <summary>
        /// 更新值语句集合。
        /// </summary>
        public IEnumerable<ISqlFragment> Values => _KeyValues.Values;
        private readonly Dictionary<ColumnMetadata, ISqlFragment> _KeyValues;
        /// <summary>
        /// 设置当前要更新的值。
        /// </summary>
        /// <param name="member">更新成员。</param>
        /// <param name="fragment">更新语句。</param>
        public void SetValue(ColumnMetadata member, ISqlFragment fragment)
        {
            if (!_KeyValues.ContainsKey(member))
            {
                this.CreateMember(null, Target.GetMember(member));
                _KeyValues.Add(member, fragment);
            }
            else
            {
                _KeyValues[member] = fragment;
            }
        }
    }
    /// <summary>
    /// 删除语句片段。
    /// </summary>
    public class DeleteFragment : QueryBaseFragment
    {
        /// <summary>
        /// 创建删除语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="target">删除目标语句。</param>
        public DeleteFragment(GenerateContext context, ISourceFragment target) : base(context)
        {
            AddSource(target); Target = target;
        }
        /// <summary>
        /// 创建删除语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="target">删除目标表元数据。</param>
        /// <param name="name">自定义对象名称。</param>
        public DeleteFragment(GenerateContext context, TableMetadata target, DbName name)
            : this(context, new TableFragment(context, target, name))
        {

        }
        /// <summary>
        /// 删除目标语句。
        /// </summary>
        public ISourceFragment Target { get; }
    }
}