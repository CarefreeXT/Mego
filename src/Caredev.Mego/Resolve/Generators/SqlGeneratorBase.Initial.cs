// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Contents;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.Outputs;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    /// <summary>
    /// 用于创建数据源语句的委托
    /// </summary>
    /// <param name="context">生成上下文。</param>
    /// <param name="content">表达式。</param>
    /// <returns>数据源语句。</returns>
    public delegate ISourceFragment CreateSourceDelegate(GenerateContext context, DbExpression content);
    /// <summary>
    /// 用于创建表达式语句的委托
    /// </summary>
    /// <param name="context">生成上下文。</param>
    /// <param name="content">表达式。</param>
    /// <param name="source">当前数据源语句。</param>
    /// <returns>表达式语句。</returns>
    public delegate IExpressionFragment CreateExpressionDelegate(GenerateContext context, DbExpression content, ISourceFragment source);
    /// <summary>
    /// 用于创建成员语句的委托
    /// </summary>
    /// <param name="context">生成上下文。</param>
    /// <param name="source">当前数据源语句。</param>
    /// <param name="property">成员的CLR属性。</param>
    /// <param name="parameter">创建参数</param>
    /// <returns></returns>
    public delegate IMemberFragment CreateMemberDelegate(GenerateContext context, ISourceFragment source, MemberInfo property, object parameter);
    /// <summary>
    /// 根据指定表达式查找成员语句。
    /// </summary>
    /// <param name="context">生成上下文。</param>
    /// <param name="current">当前数据源语句。</param>
    /// <param name="expression">表达式。</param>
    /// <param name="property">成员CLR属性。</param>
    /// <param name="onlyRetrieal">是否仅检索。</param>
    /// <returns></returns>
    public delegate IMemberFragment RetrievalMemberDelegate(GenerateContext context, ISourceFragment current, DbExpression expression, MemberInfo property, bool onlyRetrieal);
    /// <summary>
    /// 根据指定表达式查找成员语句集合。
    /// </summary>
    /// <param name="context">生成上下文。</param>
    /// <param name="current">当前数据源语句。</param>
    /// <param name="expression">表达式。</param>
    /// <param name="onlyRetrieal">是否仅检索。</param>
    /// <returns></returns>
    public delegate IEnumerable<IMemberFragment> RetrievalMembersDelegate(GenerateContext context, ISourceFragment current, DbExpression expression, bool onlyRetrieal);
    /// <summary>
    /// 初始化成员语句集合。
    /// </summary>
    /// <param name="context">生成上下文。</param>
    /// <param name="current">当前数据源语句。</param>
    /// <param name="expression">表达式。</param>
    /// <param name="parent">父级数据源语句。</param>
    /// <returns>当前数据源语句。</returns>
    public delegate ISourceFragment InitialMembersDelegate(GenerateContext context, ISourceFragment current, DbExpression expression, ComplexOutputInfo parent);
    /// <summary>
    /// 根据表达式初始化聚合检索函数。
    /// </summary>
    /// <param name="context">生成上下文。</param>
    /// <param name="current">当前数据源语句。</param>
    /// <param name="expression">表达式。</param>
    /// <param name="parent">父级数据源语句。</param>
    public delegate void InitialRetrievalDelegate(GenerateContext context, ISourceFragment current, DbRetrievalFunctionExpression expression, ComplexOutputInfo parent);
    /// <summary>
    /// 数据结构维护操作委托。
    /// </summary>
    /// <param name="context">生成上下文。</param>
    /// <returns>语句片段。</returns>
    public delegate SqlFragment MaintenanceOperateDelegate(GenerateContext context);
    /// <summary>
    /// 生成语句片段委托。
    /// </summary>
    /// <param name="context">生成上下文。</param>
    /// <param name="content">表达式内容。</param>
    /// <returns>语句片段。</returns>
    public delegate SqlFragment GenerateFragmentDelegate(GenerateContext context, DbExpression content);
    //初始化语句生成器对象。
    public partial class SqlGeneratorBase
    {
        private readonly IDictionary<EExpressionType, CreateSourceDelegate> _CreateSourceMethods;
        private readonly IDictionary<EExpressionType, CreateExpressionDelegate> _CreateExpressionMethods;
        private readonly IDictionary<EExpressionType, RetrievalMemberDelegate> _RetrievalMemberMethods;
        private readonly IDictionary<EExpressionType, RetrievalMembersDelegate> _RetrievalMembersMethods;
        private readonly IDictionary<EExpressionType, InitialMembersDelegate> _InitialMembersMethods;
        private readonly IDictionary<Type, CreateMemberDelegate> _CreateMemberMethods;
        private readonly IDictionary<Type, GenerateFragmentDelegate> _GenerateFragmentMethods;
        private readonly IDictionary<MemberInfo, InitialRetrievalDelegate> _InitialRetrievalMethods;
        private readonly IDictionary<EOperateType, MaintenanceOperateDelegate> _MaintenanceMethods;
        /// <summary>
        /// 初始化语句生成器对象。
        /// </summary>
        public SqlGeneratorBase()
        {
            _CreateSourceMethods = InitialMethodsForCreateSource();
            _CreateExpressionMethods = InitialMethodsForCreateExpression();
            _RetrievalMemberMethods = InitialMethodsForRetrievalMember();
            _RetrievalMembersMethods = InitialMethodsForRetrievalMembers();
            _InitialMembersMethods = InitialMethodsForInitialMembers();

            _CreateMemberMethods = InitialMethodsForCreateMember();
            _InitialRetrievalMethods = InitialMethodsForInitialRetrieval();
            _MaintenanceMethods = InitialMethodsForMaintenance();
            _GenerateFragmentMethods = InitialMethodsForGenerateFragment();
        }
        private IDictionary<Type, GenerateFragmentDelegate> InitialMethodsForGenerateFragment()
        {
            return new Dictionary<Type, GenerateFragmentDelegate>()
            {
                { typeof(InheritDeleteContent) , GenerateForDeleteInherit },
                { typeof(DeleteContent)        , GenerateForDeleteContent },
                { typeof(UpdateContent)        , GenerateForUpdateContent },
                { typeof(InheritUpdateContent) , GenerateForInheritUpdate },
                { typeof(InsertContent)        , GenerateForInsertContent },
                { typeof(InheritInsertContent) , GenerateForInheritInsert },
                { typeof(QueryContent)         , GenerateForQuery         },
                { typeof(RelationContent)      , GenerateForRelation      },
                { typeof(MaintenanceContent)   , GenerateForMaintenance   },
                { typeof(StatementContent)     , GenerateForStatement     },
            };
        }
        /// <summary>
        /// 初始化<see cref="CreateSourceDelegate"/>的方法映射。
        /// </summary>
        /// <returns>映射字典。</returns>
        protected virtual IDictionary<EExpressionType, CreateSourceDelegate> InitialMethodsForCreateSource()
        {
            var result = new Dictionary<EExpressionType, CreateSourceDelegate>()
            {
                { EExpressionType.DataSet, CreateSourceForDataSet },
                { EExpressionType.SetConnect, CreateSourceForSetConnect },
                { EExpressionType.CrossJoin, CreateSourceForCrossJoin },
                { EExpressionType.InnerJoin, CreateSourceForInnerJoin },
                { EExpressionType.Select, CreateSourceForSelect },
                { EExpressionType.ObjectMember,CreateSourceForObjectMember },

                { EExpressionType.GroupSet,CreateSourceForGroupSet },
                { EExpressionType.GroupBy, CreateSourceForGroupBy },
                { EExpressionType.GroupJoin, CreateSourceForGroupJoin },
                { EExpressionType.CollectionMember, CreateSourceForCollectionMember },
            };
            return result;
        }
        /// <summary>
        /// 初始化<see cref="CreateExpressionDelegate"/>的方法映射。
        /// </summary>
        /// <returns>映射字典。</returns>
        protected virtual IDictionary<EExpressionType, CreateExpressionDelegate> InitialMethodsForCreateExpression()
        {
            var result = new Dictionary<EExpressionType, CreateExpressionDelegate>()
            {
                { EExpressionType.Constant, CreateExpressionForConstant },
                { EExpressionType.Binary, CreateExpressionForBinary },
                { EExpressionType.ScalarFunction, CreateExpressionForScalarFunction },
                { EExpressionType.JoinKeyPair, CreateExpressionForJoinKeyPair },
                { EExpressionType.Order, CreateExpressionForOrder },
                { EExpressionType.Unary, CreateExpressionForUnary },
                // EExpressionType.Default                
            };
            return result;
        }
        /// <summary>
        /// 初始化<see cref="RetrievalMemberDelegate"/>的方法映射。
        /// </summary>
        /// <returns>映射字典。</returns>
        protected virtual IDictionary<EExpressionType, RetrievalMemberDelegate> InitialMethodsForRetrievalMember()
        {
            return new Dictionary<EExpressionType, RetrievalMemberDelegate>()
            {
                { EExpressionType.MemberAccess, RetrievalMemberForMemberAccess },
                { EExpressionType.AggregateFunction, RetrievalMemberForAggregateFunction },
                { EExpressionType.UnitValueContent, RetrievalMemberForUnitValueContent },
            };
        }
        /// <summary>
        /// 初始化<see cref="RetrievalMembersDelegate"/>的方法映射。
        /// </summary>
        /// <returns>映射字典。</returns>
        protected virtual IDictionary<EExpressionType, RetrievalMembersDelegate> InitialMethodsForRetrievalMembers()
        {
            return new Dictionary<EExpressionType, RetrievalMembersDelegate>()
            {
                { EExpressionType.UnitItemContent, RetrievalMembersForUnitItemContent },
                { EExpressionType.DataItem, RetrievalMembersForDataItem },
                { EExpressionType.ObjectMember, RetrievalMembersForObjectMember },
                { EExpressionType.New, RetrievalMembersForNew },
                { EExpressionType.CollectionMember, RetrievalMembersForVirtualList },
                { EExpressionType.GroupSet, RetrievalMembersForVirtualList },
                { EExpressionType.GroupItem, RetrievalMembersForVirtualList },
            };
        }
        /// <summary>
        /// 初始化<see cref="InitialMembersDelegate"/>的方法映射。
        /// </summary>
        /// <returns>映射字典。</returns>
        protected virtual IDictionary<EExpressionType, InitialMembersDelegate> InitialMethodsForInitialMembers()
        {
            return new Dictionary<EExpressionType, InitialMembersDelegate>()
            {
                { EExpressionType.DataItem, InitialMembersForCommon },
                { EExpressionType.UnitItemContent, InitialMembersForCommon },
                { EExpressionType.UnitValueContent, InitialMembersForUnitValueContent },
                { EExpressionType.ObjectMember, InitialMembersForObjectMember },
                { EExpressionType.UnitObjectContent, InitialMembersForUnitObjectContent },
                { EExpressionType.New, InitialMembersForNew },

                { EExpressionType.AggregateFunction, InitialMembersForAggregateFunction },
                { EExpressionType.RetrievalFunction, InitialMembersForRetrievalFunction },

                { EExpressionType.CollectionMember, InitialMembersForCommon },
                { EExpressionType.GroupSet, InitialMembersForCommon },
                { EExpressionType.GroupItem, InitialMembersForCommon },
            };
        }
        /// <summary>
        /// 初始化<see cref="CreateMemberDelegate"/>的方法映射。
        /// </summary>
        /// <returns>映射字典。</returns>
        protected virtual IDictionary<Type, CreateMemberDelegate> InitialMethodsForCreateMember()
        {
            var result = new Dictionary<Type, CreateMemberDelegate>()
            {
                { typeof(MemberFragment), CreateMemberForReference },
                { typeof(ColumnMetadata), CreateMemberForColumn },
                { typeof(DbAggregateFunctionExpression), CreateMemberForAggregate },
            };
            return result;
        }
        /// <summary>
        /// 初始化<see cref="InitialRetrievalDelegate"/>的方法映射。
        /// </summary>
        /// <returns>映射字典。</returns>
        protected virtual IDictionary<MemberInfo, InitialRetrievalDelegate> InitialMethodsForInitialRetrieval()
        {
            return new Dictionary<MemberInfo, InitialRetrievalDelegate>()
            {
                { SupportMembers.Queryable.First1, InitialRetrievalForFirst },
                { SupportMembers.Queryable.First2, InitialRetrievalForFirst },
                { SupportMembers.Queryable.FirstOrDefault1, InitialRetrievalForFirstDefault },
                { SupportMembers.Queryable.FirstOrDefault2, InitialRetrievalForFirstDefault },
                { SupportMembers.Queryable.Single1, InitialRetrievalForSingle },
                { SupportMembers.Queryable.Single2, InitialRetrievalForSingle },
                { SupportMembers.Queryable.SingleOrDefault1, InitialRetrievalForSingleDefault },
                { SupportMembers.Queryable.SingleOrDefault2, InitialRetrievalForSingleDefault },
                { SupportMembers.Queryable.ElementAt, InitialRetrievalForElementAt },
                { SupportMembers.Queryable.ElementAtOrDefault, InitialRetrievalForElementAtDefault },
            };
        }
    }
}