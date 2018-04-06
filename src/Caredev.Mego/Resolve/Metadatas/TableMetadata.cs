// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    /// <summary>
    /// 数据表元数据。
    /// </summary>
    public class TableMetadata : TypeMetadataBase
    {
        private readonly static Type NotMappedAttributeType = typeof(NotMappedAttribute);
        private readonly static Type TableAttributeType = typeof(TableAttribute);
        private const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance;
        private const BindingFlags NavigatePropertyBindingFlags = BindingFlags.DeclaredOnly | PropertyBindingFlags;
        /// <summary>
        /// 创建数据表元数据。
        /// </summary>
        /// <param name="itemType">数据项CLR类型。</param>
        /// <param name="attr">表的特性。</param>
        /// <param name="engine">元数据引擎。</param>
        public TableMetadata(Type itemType, TableAttribute attr, MetadataEngine engine)
            : this(itemType, attr.Name, engine, attr.MapInheritedProperties)
        {
            if (!string.IsNullOrEmpty(attr.Schema))
                Schema = attr.Schema;
        }
        /// <summary>
        /// 创建数据表元数据。
        /// </summary>
        /// <param name="itemType">数据项CLR类型。</param>
        /// <param name="name">强制表名 。</param>
        /// <param name="engine">元数据引擎。</param>
        /// <param name="inheritedProperties">是否继承父级属性。</param>
        public TableMetadata(Type itemType, string name, MetadataEngine engine, bool inheritedProperties = true)
            : base(itemType, engine)
        {
            Name = name;
            var columns = InitialMembers(itemType, inheritedProperties, out TableMetadata keyOwner);
            if (keyOwner == this)
            {
                Keys = InitialKeys(keyOwner, ref columns);
            }
            else
            {
                Keys = keyOwner.Keys;
            }
            Members = new MemberCollection<ColumnMetadata>(columns);
            RefreshConcurrencys();
        }
        /// <summary>
        /// 数据表架构名。
        /// </summary>
        public string Schema { get; }
        /// <summary>
        /// 数据表名。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 主键成员集合。
        /// </summary>
        public ColumnMetadata[] Keys { get; private set; }
        /// <summary>
        /// 参与并发检查成员的集合。
        /// </summary>
        public ColumnMetadata[] Concurrencys { get; private set; }
        /// <summary>
        /// 当前数据表继承链数据表元数据集合。
        /// </summary>
        public TableMetadata[] InheritSets { get; private set; }
        /// <summary>
        /// 当前数据表的成员集合，如果该数据表为继承的表，则成员中不会包含父级的成员。
        /// </summary>
        public MemberCollection<ColumnMetadata> Members { get; }
        /// <summary>
        /// 导航成员元数据集合。
        /// </summary>
        public Dictionary<MemberInfo, NavigateMetadata> Navigates
        {
            get
            {
                if (_Navigates == null)
                {
                    _Navigates = new Dictionary<MemberInfo, NavigateMetadata>();
                    var propertys = ClrType.GetProperties(NavigatePropertyBindingFlags)
                        .Where(a => a.PropertyType.IsComplexCollection() || a.PropertyType.IsObject());
                    var sourceTypeMetadata = Engine.Type(ClrType);
                    foreach (var property in propertys)
                    {
                        var source = this;
                        var targetType = property.PropertyType;
                        bool isCollectionProperty = targetType.IsComplexCollection();
                        if (isCollectionProperty)
                        {
                            targetType = targetType.ElementType();
                        }
                        var target = Engine.Table(targetType);
                        var targetTypeMetadata = Engine.Type(targetType);

                        var attr1 = property.GetCustomAttribute<ForeignKeyAttribute>();
                        if (attr1 != null)
                        {
                            var pairs = CreatePair(source, target, attr1.Names, attr1.Principals);
                            var navigate = new NavigateMetadata(source, target, pairs) { IsForeign = true };
                            navigate.Initial(sourceTypeMetadata, targetTypeMetadata, property, isCollectionProperty);
                            _Navigates.Add(property, navigate);
                            continue;
                        }
                        var attr2 = property.GetCustomAttribute<InversePropertyAttribute>();
                        if (attr2 != null)
                        {
                            var pairs = CreatePair(target, source, attr2.Names, attr2.Principals);
                            var navigate = new NavigateMetadata(source, target, pairs) { IsForeign = false };
                            navigate.Initial(sourceTypeMetadata, targetTypeMetadata, property, isCollectionProperty);
                            _Navigates.Add(property, navigate);
                            continue;
                        }
                        var attr3 = property.GetCustomAttribute<RelationshipAttribute>();
                        if (attr3 != null)
                        {
                            var navigate = CreateNavigate(source, target, Engine.Table(attr3.RelationType), attr3);
                            navigate.Initial(sourceTypeMetadata, targetTypeMetadata, property, isCollectionProperty);
                            _Navigates.Add(property, navigate);
                            continue;
                        }
                    }
                }
                return _Navigates;
            }
        }
        private Dictionary<MemberInfo, NavigateMetadata> _Navigates;
        /// <summary>
        /// 刷新并发成员
        /// </summary>
        internal void RefreshConcurrencys()
        {
            Concurrencys = Members.Where(a => !a.IsKey && a.IsConcurrencyCheck).ToArray();
        }
        /// <summary>
        /// 创建复合导航关系。
        /// </summary>
        /// <param name="source">源数据表。</param>
        /// <param name="target">目标数据表。</param>
        /// <param name="table">所在的表元数据对象。</param>
        /// <param name="attr">关系特性对象。</param>
        /// <returns>复合导航关系。</returns>
        private CompositeNavigateMetadata CreateNavigate(TableMetadata source, TableMetadata target, TableMetadata table, RelationshipAttribute attr)
        {
            var pairs = CreatePair(table, source, attr.LeftNames, attr.LeftPrincipals);
            var cpairs = CreatePair(table, target, attr.RightNames, attr.RightPrincipals);
            return new CompositeNavigateMetadata(source, target, table, pairs, cpairs);
        }
        //创建主外键对。
        private ForeignPrincipalPair[] CreatePair(TableMetadata foreign, TableMetadata principal, string[] names, string[] principals)
        {
            var pairs = new ForeignPrincipalPair[names.Length];
            for (int i = 0; i < pairs.Length; i++)
            {
                pairs[i] = new ForeignPrincipalPair(
                   foreign.GetColumnByName(names[i]),
                   principal.GetColumnByName(principals[i])
                   );
            }
            return pairs;
        }
        //通过列名获取数据列元数据。
        private ColumnMetadata GetColumnByName(string name)
        {
            var column = Members.Where(a => a.Member.Name == name).Select(a => a).FirstOrDefault();
            if (column != null)
                return column;
            for (int i = InheritSets.Length - 1; i >= 0; i--)
            {
                column = InheritSets[i].Members.Where(a => a.Member.Name == name).Select(a => a).FirstOrDefault();
                if (column != null)
                    return column;
            }
            throw new KeyNotFoundException();
        }
        //初始化当前成员集合。
        private IEnumerable<ColumnMetadata> InitialMembers(Type itemType, bool inheritedProperties, out TableMetadata keyOwner)
        {
            var engine = Engine;
            IEnumerable<ColumnMetadata> columns;
            IEnumerable<PropertyInfo> propertyCollection = null;
            if (itemType.IsAnonymous())
            {
                propertyCollection = itemType.GetProperties(PropertyBindingFlags)
                    .Where(a => a.CanRead && a.PropertyType.IsPrimary())
                    .Sort();
            }
            else
            {
                propertyCollection = itemType.GetProperties(PropertyBindingFlags)
                    .Where(a => !a.IsDefined(NotMappedAttributeType) && a.CanRead && a.CanWrite && a.PropertyType.IsPrimary())
                    .Sort();
            }
            keyOwner = this;
            if (inheritedProperties)
            {
                List<Type> inheritedTypes = new List<Type>();
                columns = propertyCollection.Where(a => a.DeclaringType == itemType).Select(a => new ColumnMetadata(this, a));
                InheritSets = propertyCollection
                    .Where(a => a.DeclaringType != itemType)
                    .Select(a => a.DeclaringType)
                    .Distinct()
                    .OrderBy(a => a, InheritTypeComparer.Instance)
                    .Select(a => engine.Table(a))
                    .ToArray();
                if (InheritSets.Length > 0)
                    keyOwner = InheritSets[0];
            }
            else
            {
                columns = propertyCollection.Select(a => new ColumnMetadata(this, a));
                InheritSets = new TableMetadata[0];
            }
            return columns.ToArray();
        }
        //初始化主键集合。
        private ColumnMetadata[] InitialKeys(TableMetadata keyOwner, ref IEnumerable<ColumnMetadata> columns)
        {
            var keys = columns.Where(a => Attribute.IsDefined(a.Member, typeof(KeyAttribute)))
                .OrderBy(a => a.Order)
                .ThenBy(a => a.Name).ToArray();
            if (keys.Length == 0)
            {
                var name = ClrType.Name + "Id";
                var key = columns.Where(a => a.Member.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Select(a => a).FirstOrDefault();
                if (key == null)
                {
                    name = "Id";
                    key = columns.Where(a => a.Member.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase)).Select(a => a).FirstOrDefault();
                }
                if (key != null)
                    keys = new ColumnMetadata[] { key };
            }
            if (keys.Length == 0)
            {
                throw new NotSupportedException("未找到主键");
            }
            foreach (var key in keys)
                key.IsKey = true;
            columns = keys.Union(columns.Where(a => !a.IsKey));
            return keys;
        }
    }
}