// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using Caredev.Mego.Resolve.Outputs;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq.Expressions;
    /// <summary>
    /// 提交多个数据对象的操作。
    /// </summary>
    public abstract class DbObjectsOperateBase : DbSplitObjectsOperate<object>
    {
        private bool _HasResult = true;
        /// <summary>
        /// 创建多个数据对象的提交操作。
        /// </summary>
        /// <param name="target">操作目标。</param>
        /// <param name="items">提交的数据集合。</param>
        /// <param name="type">数据项CLR类型。</param>
        /// <param name="expression">操作目标表达式。</param>
        internal DbObjectsOperateBase(IDbSet target, IEnumerable items, Type type, Expression expression)
            : base(target.Context, type, (IEnumerable<object>)items)
        {
            _ItemParameterCount = target.Context.Configuration.Metadata.Type(type).PrimaryMembers.Count;
            Expression = expression;
            DbSet = target;
        }
        /// <summary>
        /// 目标数据集。
        /// </summary>
        public IDbSet DbSet { get; }
        /// <summary>
        /// 操作表达式。
        /// </summary>
        public Expression Expression { get; }
        /// <inheritdoc/>
        internal override bool Read(DbDataReader reader)
        {
            var output = (ComplexOutputInfo)Output;
            var metadata = output.Metadata;
            var fields = output.ItemFields;
            foreach (var obj in this)
            {
                reader.Read();
                metadata.ModifyProperty(reader, fields, obj);
            }
            return true;
        }
        /// <inheritdoc/>
        public override int ItemParameterCount => _ItemParameterCount;
        private readonly int _ItemParameterCount;
        /// <inheritdoc/>
        public override bool HasResult => _HasResult;
        /// <summary>
        /// 更改是否返回结果的值。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbObjectsOperateBase ChangeHasResult(bool value)
        {
            _HasResult = value;
            return this;
        }
    }
}