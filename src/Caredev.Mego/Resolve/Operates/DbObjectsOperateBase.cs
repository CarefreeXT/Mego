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
        internal override bool Read(DbDataReader reader) => Read(reader, false);
        /// <summary>
        /// 读取数据，回写对象属性。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <param name="isMoveResult">是否移动<see cref="DbDataReader"/>结果集。</param>
        internal bool Read(DbDataReader reader, bool isMoveResult)
        {
            var output = (ComplexOutputInfo)Output;
            var metadata = output.Metadata;
            var fields = output.ItemFields;
            if (isMoveResult)
            {
                foreach (var obj in this)
                {
                    reader.Read();
                    metadata.ModifyProperty(reader, fields, obj);
                    reader.NextResult();
                }
            }
            else
            {
                foreach (var obj in this)
                {
                    reader.Read();
                    metadata.ModifyProperty(reader, fields, obj);
                }
            }
            return true;
        }
        /// <summary>
        /// 读取数据，回写对象属性。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <param name="obj">指定回写的对象。</param>
        /// <returns>是否成功。</returns>
        internal bool Read(DbDataReader reader, object obj)
        {
            var output = (ComplexOutputInfo)Output;
            var metadata = output.Metadata;
            var fields = output.ItemFields;
            reader.Read();
            metadata.ModifyProperty(reader, fields, obj);
            return true;
        }
        /// <summary>
        /// 从数据库参数中读取值，回写数据对象。
        /// </summary>
        /// <param name="parameters">写入参数。</param>
        /// <returns>是否成功。</returns>
        internal bool Read(DbParameter[] parameters)
        {
            var output = (ComplexOutputInfo)Output;
            var metadata = output.Metadata;
            var fields = output.ItemFields;
            var values = new object[parameters.Length];
            if (this.Count == 1)
            {
                CopyTo(parameters, values);
                foreach (var item in this)
                {
                    metadata.ModifyProperty(values, fields, item);
                    return true;
                }
            }
            else
            {
                var list = CopyTo(parameters, new Array[parameters.Length]);
                var index = 0;
                foreach (var item in this)
                {
                    CopyTo(list, values, index++);
                    metadata.ModifyProperty(values, fields, item);
                }
            }
            return true;
        }
        /// <summary>
        /// 从数据库参数中读取值，回写指定数据对象。
        /// </summary>
        /// <param name="parameters">写入参数。</param>
        /// <param name="values">值数组。</param>
        /// <param name="obj">指定回写的数据对象。</param>
        /// <returns>是否成功。</returns>
        internal bool Read(DbParameter[] parameters, object[] values, object obj)
        {
            var output = (ComplexOutputInfo)Output;
            CopyTo(parameters, values);
            output.Metadata.ModifyProperty(values, output.ItemFields, obj);
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
        private static void CopyTo(DbParameter[] parameters, object[] values)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                values[i] = parameters[i].Value;
            }
        }
        private static void CopyTo(Array[] data, object[] values, int index)
        {
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = data[i].GetValue(index);
            }
        }
        private static Array[] CopyTo(DbParameter[] parameters, Array[] values)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                values[i] = (Array)parameters[i].Value;
            }
            return values;
        }
    }
}