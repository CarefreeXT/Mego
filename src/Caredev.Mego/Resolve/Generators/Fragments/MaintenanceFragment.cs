// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Fragments
{
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// 判断对象存在语句片段。
    /// </summary>
    public class ObjectExsitFragment : SqlFragment
    {
        /// <summary>
        /// 创建判断对象存在。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="name">对象名称。</param>
        /// <param name="kind">对象种类。</param>
        public ObjectExsitFragment(GenerateContext context, ObjectNameFragment name, EDatabaseObject kind)
            : base(context)
        {
            Name = name;
            Kind = kind;
        }
        /// <summary>
        /// 名称对象。
        /// </summary>
        public ObjectNameFragment Name { get; }
        /// <summary>
        /// 对象种类。
        /// </summary>
        public EDatabaseObject Kind { get; }
    }
    /// <summary>
    /// 创建关系语句片段。
    /// </summary>
    public class CreateRelationFragment : SqlFragment
    {
        /// <summary>
        /// 初始化创建关系。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="name">关系名称。</param>
        public CreateRelationFragment(GenerateContext context, string name)
            : base(context)
        {
            Name = name;
        }
        /// <summary>
        /// 关系名称。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 关系主表名称。
        /// </summary>
        public ObjectNameFragment Principal { get; set; }
        /// <summary>
        /// 关系主键名称集合。
        /// </summary>
        public string[] PrincipalKeys { get; set; }
        /// <summary>
        /// 关系外键表名称。
        /// </summary>
        public ObjectNameFragment Foreign { get; internal set; }
        /// <summary>
        /// 关系外键名称集合。
        /// </summary>
        public string[] ForeignKeys { get; set; }
    }
    /// <summary>
    /// 创建表语句片段基类。
    /// </summary>
    public abstract class CreateTableBaseFragment : SqlFragment, ICreateFragment
    {
        /// <summary>
        /// 实例化创建表语句片段对象。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        public CreateTableBaseFragment(GenerateContext context)
            : base(context)
        {
        }
        /// <summary>
        /// 表成员列表。
        /// </summary>
        public IList<ICreateFragment> Members { get; } = new List<ICreateFragment>();
        /// <summary>
        /// 当前是否为临时表。
        /// </summary>
        public abstract bool IsTemporary { get; }
    }
    /// <summary>
    /// 创建普通表语句片段。
    /// </summary>
    public class CreateTableFragment : CreateTableBaseFragment
    {
        /// <summary>
        /// 初始化创建普通表。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="table">表元数据。</param>
        public CreateTableFragment(GenerateContext context, TableMetadata table)
            : base(context)
        {
            Metadata = table;
            Table = new ObjectNameFragment(context, table.Name, table.Schema);
            foreach (var col in table.Members)
            {
                Members.Add(new CreateColumnFragment(context, col, this));
            }
        }
        /// <summary>
        /// 表元数据。
        /// </summary>
        public TableMetadata Metadata { get; }
        /// <summary>
        /// 表名称。
        /// </summary>
        public ObjectNameFragment Table { get; }
        /// <inheritdoc/>
        public override bool IsTemporary => false;
    }
    /// <summary>
    /// 创建临时表语句片段。
    /// </summary>
    public class CreateTempTableFragment : CreateTableBaseFragment
    {
        /// <summary>
        /// 初始化创建临时表。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="table">仔在临时表。</param>
        public CreateTempTableFragment(GenerateContext context, TemporaryTableFragment table)
            : base(context)
        {
            Table = table;
            IsVariable = context.Feature.HasCapable(EDbCapable.TableVariable);
        }
        /// <summary>
        /// 初始化创建临时表。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="columns">指定的数据列。</param>
        public CreateTempTableFragment(GenerateContext context, IEnumerable<ColumnMetadata> columns)
         : this(context, new TemporaryTableFragment(context, columns))
        {
            foreach (var col in columns)
            {
                Table.CreateMember(null, col);
                Members.Add(new CreateColumnFragment(context, col, this));
            }
        }
        /// <summary>
        /// 当前临时表对象。
        /// </summary>
        public TemporaryTableFragment Table { get; }
        /// <inheritdoc/>
        public override bool IsTemporary => true;
        /// <summary>
        /// 是否为表变量，如果数据库支持则会自动转为 True
        /// </summary>
        public bool IsVariable { get; set; }
    }
    /// <summary>
    /// 创建列语句片段。
    /// </summary>
    public class CreateColumnFragment : SqlFragment, ICreateFragment
    {
        /// <summary>
        /// 初始化创建列。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="column">列元数据。</param>
        /// <param name="table">创建表的语句对象。</param>
        /// <param name="name">强制列名。</param>
        public CreateColumnFragment(GenerateContext context, ColumnMetadata column, CreateTableBaseFragment table, string name = "")
            : base(context)
        {
            Metadata = column;
            Table = table;
            if (name == "")
                Name = column.Name;
            else
                Name = name;
        }
        /// <summary>
        /// 创建表的语句对象。
        /// </summary>
        public CreateTableBaseFragment Table { get; }
        /// <summary>
        /// 数据列元数据。
        /// </summary>
        public ColumnMetadata Metadata { get; }
        /// <summary>
        /// 当前生成列名。
        /// </summary>
        public string Name { get; }
    }
    /// <summary>
    /// 创建变量语句片段。
    /// </summary>
    public class CreateVariableFragment : SqlFragment, ICreateFragment
    {
        /// <summary>
        /// 初始化创建变量。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="type">变量类型。</param>
        /// <param name="name">存在的变量名称。</param>
        public CreateVariableFragment(GenerateContext context, Type type, VariableFragment name)
            : base(context)
        {
            Name = name;
            ClrType = type;
        }
        /// <summary>
        /// 初始化创建变量。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="type">变量类型。</param>
        public CreateVariableFragment(GenerateContext context, Type type)
            : this(context, type, new VariableFragment(context))
        {
        }
        /// <summary>
        /// 变量名称。
        /// </summary>
        public VariableFragment Name { get; }
        /// <summary>
        /// 变量CLR类型。
        /// </summary>
        public Type ClrType { get; }
    }
}