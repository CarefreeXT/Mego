// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Fragments
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    /// <summary>
    /// 数据源语句块片段基类。
    /// </summary>
    public abstract class SourceFragment : SqlFragment, ISourceFragment
    {
        internal readonly SqlGeneratorBase Generator;
        /// <summary>
        /// 初始化数据源语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        public SourceFragment(GenerateContext context)
            : base(context)
        {
            Generator = context.Generator;
        }
        /// <summary>
        /// 数据源别名。
        /// </summary>
        public string AliasName => _AliasName ?? (_AliasName = Context.GetDataSourceAlias());
        private string _AliasName;
        /// <summary>
        /// 成员列表。
        /// </summary>
        public virtual List<IMemberFragment> Members { get; } = new List<IMemberFragment>();
        /// <summary>
        /// 父级数据源。
        /// </summary>
        public virtual ISourceFragment Parent { get; set; }
        /// <summary>
        /// 连接操作种类。
        /// </summary>
        public EJoinType? Join { get; set; }
        /// <summary>
        /// 连接条件。
        /// </summary>
        public IExpressionFragment Condition { get; set; }
        /// <summary>
        /// 根据成员CLR属性获取成员语句。
        /// </summary>
        /// <param name="member">CLR属性。</param>
        /// <returns>成员语句。</returns>
        public virtual IMemberFragment GetMember(MemberInfo member) => Members.First(a => a.Property == member);
        /// <summary>
        /// 根据列元数据获取成员语句。
        /// </summary>
        /// <param name="metadata">列元数据。</param>
        /// <returns>成员语句。</returns>
        public virtual IMemberFragment GetMember(ColumnMetadata metadata) => GetMember(metadata.Member);
    }
    /// <summary>
    /// 继承表语句块片段。
    /// </summary>
    public class InheritFragment : SourceFragment
    {
        /// <summary>
        /// 创建继承表语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="metadata">数据表元数据。</param>
        public InheritFragment(GenerateContext context, TableMetadata metadata)
            : base(context)
        {
            Metadata = metadata;
            var inheritSets = Metadata.InheritSets;
            Tables = new TableFragment[inheritSets.Length + 1];
            var index = 0;
            for (; index < inheritSets.Length; index++)
            {
                Tables[index] = new TableFragment(context, inheritSets[index]);
            }
            Tables[index] = new TableFragment(context, Metadata);
        }
        /// <summary>
        /// 数据表元数据。
        /// </summary>
        public TableMetadata Metadata { get; }
        /// <summary>
        /// 继承的表语句集合。
        /// </summary>
        public TableFragment[] Tables { get; }
        /// <inheritdoc/>
        public override List<IMemberFragment> Members
        {
            get
            {
                Initialize();
                return base.Members;
            }
        }
        private Dictionary<MemberInfo, IMemberFragment> _Members;
        /// <summary>
        /// 初始化继承语句块信息。
        /// </summary>
        /// <returns>返回当前对象。</returns>
        public InheritFragment Initialize()
        {
            if (_Members == null)
            {
                var generator = Generator;
                foreach (var item in Tables)
                {
                    item.Parent = this.Parent;
                    base.Members.AddRange(item.Members);
                }
                _Members = Tables.SelectMany(a => a.Members).ToDictionary(b => b.Property, b => b);
                var keys = (from a in Tables[0].Members
                            join b in Metadata.Keys on a.Property equals b.Member
                            select new
                            {
                                Member = a,
                                Column = b
                            }).ToArray();
                for (int i = 1; i < Tables.Length; i++)
                {
                    var table = Tables[i];
                    table.Join = EJoinType.InnerJoin;
                    table.Condition = keys.Select(item => new BinaryFragment(Context, EBinaryKind.Equal)
                    {
                        Left = item.Member,
                        Right = generator.CreateMember(Context, table, item.Column.Member, item.Column)
                    }).Merge();
                }
            }
            return this;
        }
        /// <inheritdoc/>
        public override IMemberFragment GetMember(MemberInfo member)
        {
            Initialize();
            if (!_Members.TryGetValue(member, out IMemberFragment result))
                throw new ArgumentException(nameof(member));
            return result;
        }
        /// <inheritdoc/>
        public override ISourceFragment Parent
        {
            get => Tables[0].Parent;
            set => Tables.ForEach(a => a.Parent = value);
        }
    }
    /// <summary>
    /// 数据表语句片段。
    /// </summary>
    public class TableFragment : SourceFragment
    {
        /// <summary>
        /// 创建数据表语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="dataset">数据集表达式。</param>
        public TableFragment(GenerateContext context, DbDataSetExpression dataset)
            : this(context, context.Metadata.Table(dataset.Item.ClrType), dataset.Name)
        {

        }
        /// <summary>
        /// 创建数据表语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="metadata">数据表元数据。</param>
        /// <param name="name">数据表名称。</param>
        public TableFragment(GenerateContext context, TableMetadata metadata, DbName name = null)
            : base(context)
        {
            Name = context.ConvertName(name);
            Metadata = metadata;

            _Members = metadata.Members.ToDictionary(a => (MemberInfo)a.Member,
                a => Generator.CreateMember(Context, this, a.Member, a));
            base.Members.AddRange(_Members.Values);
            if (metadata.InheritSets.Length > 0)
            {
                foreach (var a in metadata.Keys)
                {
                    _Members.Add(a.Member, Generator.CreateMember(Context, this, a.Member, a));
                }
            }
        }
        private readonly Dictionary<MemberInfo, IMemberFragment> _Members;
        /// <inheritdoc/>
        public override IMemberFragment GetMember(MemberInfo member)
        {
            if (_Members.TryGetValue(member, out IMemberFragment value))
                return value;
            throw new ArgumentException(nameof(member));
        }
        /// <summary>
        /// 数据表元数据。
        /// </summary>
        public TableMetadata Metadata { get; }
        /// <summary>
        /// 自定义名称。
        /// </summary>
        public INameFragment Name { get; set; }
    }
    /// <summary>
    /// 多个数据表连接集语句片段。
    /// </summary>
    public class SetFragment : SourceFragment
    {
        /// <summary>
        /// 创建多个数据表连接集。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="first">首个数据源。</param>
        public SetFragment(GenerateContext context, ISourceFragment first)
            : base(context)
        {
            _Sources.Add(first);
        }
        /// <summary>
        /// 当前所有的数据源语句集合。
        /// </summary>
        public IEnumerable<ISourceFragment> Sources => _Sources;
        private List<ISourceFragment> _Sources = new List<ISourceFragment>();
        private List<EConnectKind> ConnectKindValues = new List<EConnectKind>();
        /// <summary>
        /// 获取指定源的连接类型。
        /// </summary>
        /// <param name="source">指定数据语句。</param>
        /// <returns>连接种类。</returns>
        public EConnectKind this[ISourceFragment source]
        {
            get
            {
                var index = _Sources.IndexOf(source) - 1;
                if (index < 0)
                {
                    throw new IndexOutOfRangeException(Res.ExceptionInvalidIndexObject);
                }
                return ConnectKindValues[index];
            }
        }
        /// <summary>
        /// 添加数据源语句片段。
        /// </summary>
        /// <param name="source">指定数据源语句。</param>
        /// <param name="kind">连接种类。</param>
        /// <returns>返回当前对象。</returns>
        public SetFragment AddSource(ISourceFragment source, EConnectKind kind)
        {
            if (!_Sources.Contains(source))
            {
                _Sources.Add(source);
                ConnectKindValues.Add(kind);
            }
            return this;
        }
    }
    /// <summary>
    /// 值列表语句片段。
    /// </summary>
    public class ValuesFragment : SourceFragment
    {
        /// <summary>
        /// 创建语句片段
        /// </summary>
        /// <param name="context">生成上下文。></param>
        /// <param name="source">值列表提交对象语句块。</param>
        /// <param name="data">值列表数据集合。</param>
        /// <param name="metadata">相关表元数据。></param>
        public ValuesFragment(GenerateContext context, CommitObjectFragment source, IEnumerable data, TableMetadata metadata = null)
            : base(context)
        {
            Data = data;
            Source = source;
            _Metadata = metadata;
        }
        /// <summary>
        /// 用于插入的提交对象语句块。
        /// </summary>
        public CommitObjectFragment Source { get; }
        /// <summary>
        /// 列表数据集合。
        /// </summary>
        public IEnumerable Data { get; }
        /// <summary>
        /// 插入值语句集合。
        /// </summary>
        public IEnumerable<ISqlFragment> Values => _Values.Values;
        /// <inheritdoc/>
        public override IMemberFragment GetMember(MemberInfo member)
        {
            if (_Members.TryGetValue(member, out IMemberFragment value))
                return value;
            var column = _Metadata.Members[(PropertyInfo)member];
            value = this.CreateMember(member, column);
            _Values.Add(member, Source.GetMember(column));
            _Members.Add(member, value);
            return value;
        }
        private readonly TableMetadata _Metadata;
        private readonly Dictionary<MemberInfo, IMemberFragment> _Members = new Dictionary<MemberInfo, IMemberFragment>();
        private readonly Dictionary<MemberInfo, ISqlFragment> _Values = new Dictionary<MemberInfo, ISqlFragment>();
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
            if (!_Values.ContainsKey(member.Member))
            {
                _Members.Add(member.Member, this.CreateMember(null, member));
                _Values.Add(member.Member, fragment);
            }
            else
            {
                _Values[member.Member] = fragment;
            }
        }
    }
    /// <summary>
    /// 临时表或表变量语句片段。
    /// </summary>
    public class TemporaryTableFragment : SourceFragment
    {
        private readonly Dictionary<MemberInfo, IMemberFragment> _Members = new Dictionary<MemberInfo, IMemberFragment>();
        private readonly Dictionary<MemberInfo, ColumnMetadata> _MemberMetadatas;
        /// <summary>
        /// 创建临时表或表变量。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="metadatas">数据列元数据集合。</param>
        /// <param name="name">表名称。</param>
        public TemporaryTableFragment(GenerateContext context, IEnumerable<ColumnMetadata> metadatas, INameFragment name = null)
            : base(context)
        {
            if (name == null)
            {
                if (context.Feature.HasCapable(EDbCapable.TableVariable))
                {
                    Name = new VariableFragment(context, 't');
                }
                else
                {
                    Name = new TempTableNameFragment(context);
                }
            }
            else
            {
                Name = name;
            }
            _MemberMetadatas = metadatas.ToDictionary(a => (MemberInfo)a.Member, a => a);
        }
        /// <summary>
        /// 表名称。
        /// </summary>
        public INameFragment Name { get; }
        /// <summary>
        /// 复制当前对象。
        /// </summary>
        /// <returns></returns>
        public TemporaryTableFragment Clone()
        {
            return new TemporaryTableFragment(Context, _MemberMetadatas.Values, Name);
        }
        /// <inheritdoc/>
        public override IMemberFragment GetMember(MemberInfo member)
        {
            if (!_Members.TryGetValue(member, out IMemberFragment result))
            {
                if (!_MemberMetadatas.TryGetValue(member, out ColumnMetadata metadata))
                    throw new ArgumentException(nameof(member));
                result = this.CreateMember(null, metadata);
                _Members.Add(member, result);
            }
            return result;
        }
    }
    /// <summary>
    /// 提交对象语句片段。
    /// </summary>
    public class CommitObjectFragment : SourceFragment
    {
        private readonly Dictionary<MemberInfo, IMemberFragment> _Members;
        /// <summary>
        /// 创建提交对象语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="loader">属性加载器。</param>
        public CommitObjectFragment(GenerateContext context, IPropertyValueLoader loader)
            : base(context)
        {
            _Members = new Dictionary<MemberInfo, IMemberFragment>();
            Loader = loader;
        }
        /// <inheritdoc/>
        public override IMemberFragment GetMember(MemberInfo member)
        {
            if (_Members.TryGetValue(member, out IMemberFragment result))
                return result;
            var commitMember = new CommitMemberFragment(Context, Loader, this, member);
            this.Members.Add(commitMember);
            _Members.Add(member, commitMember);
            CreatedMember?.Invoke(member, commitMember);
            return commitMember;
        }
        /// <summary>
        /// 创建成员事件。
        /// </summary>
        public Action<MemberInfo, CommitMemberFragment> CreatedMember { get; set; }
        /// <summary>
        /// 属性加载器。
        /// </summary>
        public IPropertyValueLoader Loader { get; }
    }
    /// <summary>
    /// 虚拟数据源语句片段，该数据源实际是不存在的，它用表达数据集合聚合检索、分
    /// 组等查询操作的中间。
    /// </summary>
    public class VirtualSourceFragment : SourceFragment
    {
        /// <summary>
        /// 虚拟数据源语句
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">表达式。</param>
        /// <param name="container">所在SELECT语句容器。</param>
        /// <param name="source">原始数据源语句。</param>
        /// <param name="body">内联查询语句。</param>
        public VirtualSourceFragment(GenerateContext context, DbExpression expression,
            SelectFragment container, ISourceFragment source, SelectFragment body = null)
            : base(context)
        {
            Expression = expression;
            Container = container;
            Source = source;
            _Body = body;
        }
        /// <summary>
        /// 表达式。
        /// </summary>
        public DbExpression Expression { get; }
        /// <summary>
        /// 所在SELECT语句容器。
        /// </summary>
        public SelectFragment Container { get; }
        /// <summary>
        /// 原始数据源语句。
        /// </summary>
        public ISourceFragment Source { get; }
        /// <summary>
        /// 用于获取分组主体查询语句。
        /// </summary>
        /// <returns></returns>
        public SelectFragment GetBody()
        {
            if (_Body == null)
            {
                switch (Expression.ExpressionType)
                {
                    case EExpressionType.GroupJoin:
                        _Body = Generator.CreateVirtualBodyForGroupJoin(Context, this);
                        break;
                    case EExpressionType.CollectionMember:
                        _Body = Generator.CreateVirtualBodyForCollectionMember(Context, this);
                        break;
                    default:
                        throw new NotSupportedException(string.Format(Res.NotSupportedGetQueryBody, Expression.ExpressionType));
                }
            }
            return _Body;
        }
        private SelectFragment _Body = null;
        /// <summary>
        /// 获取输出集合列表查询语句。
        /// </summary>
        /// <returns></returns>
        public SelectFragment GetList()
        {
            if (_List == null)
            {
                switch (Expression.ExpressionType)
                {
                    case EExpressionType.CollectionMember:
                        _List = Generator.CreateVirtualListForCollectionMember(Context, this);
                        break;
                    case EExpressionType.GroupJoin:
                        _List = Generator.CreateVirtualListForGroupJoin(Context, this);
                        break;
                    case EExpressionType.GroupBy:
                        _List = Generator.CreateVirtualListForGroupBy(Context, this);
                        break;
                    default:
                        throw new NotSupportedException(string.Format(Res.NotSupportedGetQueryList, Expression.ExpressionType));
                }
            }
            return _List;
        }
        private SelectFragment _List;
        /// <summary>
        /// 当前表达式内容集合。
        /// </summary>
        public IDictionary<DbExpression, SelectFragment> Contents { get; }
    }
}