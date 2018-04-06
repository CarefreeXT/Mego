// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Expressions
{
    using System;
    using System.Linq;
    /// <summary>
    /// 表达式类型。
    /// </summary>
    public enum EExpressionType
    {
        /// <summary>
        /// 数据单元。
        /// </summary>
        Unit                = 0x0100,
        /// <summary>
        /// 数据集。
        /// </summary>
        DataSet             = 0x0101,
        /// <summary>
        /// 数据集函数用于包装一个<see cref="SetFunction"/>，函数返回值等价于<see cref="DataSet"/>。
        /// </summary>
        UnitFunction        = 0x0102,
        /// <summary>
        /// SELECT数据集。
        /// </summary>
        Select              = 0x0103,
        /// <summary>
        /// 数据源连接数据集。
        /// </summary>
        SetConnect          = 0x0104,
        /// <summary>
        /// 交叉连接数据集。
        /// </summary>
        CrossJoin           = 0x0105,
        /// <summary>
        /// 内连接数据集。
        /// </summary>
        InnerJoin           = 0x0106,
        /// <summary>
        /// 分组数据集。
        /// </summary>
        GroupBy             = 0x0107,
        /// <summary>
        /// 分组连接数据集。
        /// </summary>
        GroupJoin           = 0x0108,
        /// <summary>
        /// 分组连接数据集中的每个分组项的数据集。
        /// </summary>
        GroupSet            = 0x0109,
        /// <summary>
        /// 集合类型成员访问。
        /// </summary>
        CollectionMember    = 0x010A,
        /// <summary>
        /// 数据单元中的数据项。
        /// </summary>
        UnitItem            = 0x0200,
        /// <summary>
        /// 普通数据集中的数据项。
        /// </summary>
        DataItem            = 0x0201,
        /// <summary>
        /// 通过<see cref="Select"/>将若干个数据集生成一个新的数据集。
        /// </summary>
        New                 = 0x0202,
        /// <summary>
        /// 分组数据集中的分组项。
        /// </summary>
        GroupItem           = 0x0303,
        /// <summary>
        /// 元素检索函数，例如<see cref="Queryable.First{TSource}(IQueryable{TSource})"/>。
        /// </summary>
        RetrievalFunction   = 0x0204,
        /// <summary>
        /// 对象类型成员访问。
        /// </summary>
        ObjectMember        = 0x0205,
        /// <summary>
        /// <see cref="Select"/>返回<see cref="DataItem"/>或<see cref="GroupItem"/>数据项时的包装对象。
        /// </summary>
        UnitItemContent     = 0x0206,
        /// <summary>
        /// <see cref="Select"/>仅返回一个基元数据类型的属性值时的包装对象。
        /// </summary>
        UnitValueContent    = 0x0207,
        /// <summary>
        /// <see cref="Select"/>仅返回一个对象属性时的包装对象。
        /// </summary>
        UnitObjectContent   = 0x0208,
        /// <summary>
        /// 常量值。
        /// </summary>
        Constant            = 0x0001,
        /// <summary>
        /// 指定类型默认值。
        /// </summary>
        Default             = 0x0002,
        /// <summary>
        /// 基元类型成员访问。
        /// </summary>
        MemberAccess        = 0x0003,
        /// <summary>
        /// 二元操作。
        /// </summary>
        Binary              = 0x0004,
        /// <summary>
        /// 一元操作。
        /// </summary>
        Unary               = 0x0005,
        /// <summary>
        /// 排序操作。
        /// </summary>
        Order               = 0x0006,
        /// <summary>
        /// JOIN操作中条件的配对成员。
        /// </summary>
        JoinKeyPair         = 0x0007,
        /// <summary>
        /// 标量函数，例如<see cref="DateTime.Now"/>。
        /// </summary>
        ScalarFunction      = 0x0011,
        /// <summary>
        /// 映射到数据库的项函数。
        /// </summary>
        ItemFunction        = 0x0012,
        /// <summary>
        /// 数据集函数，该函数必须要关联<see cref="UnitFunction"/>才能使用。
        /// </summary>
        SetFunction         = 0x0013,
        /// <summary>
        /// 聚合函数，例如<see cref="Queryable.Sum(IQueryable{int})"/>。
        /// </summary>
        AggregateFunction   = 0x0014,
        /// <summary>
        /// 判断函数，例如<see cref="Queryable.Any{TSource}(IQueryable{TSource})"/>。
        /// </summary>
        JudgeFunction       = 0x0015,
        /// <summary>
        /// 更新数据时获取数据库原始值引用对象。
        /// </summary>
        OriginalObject      = 0x0016,
    }
}