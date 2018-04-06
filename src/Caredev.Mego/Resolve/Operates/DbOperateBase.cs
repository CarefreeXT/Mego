// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Operates
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using Caredev.Mego.Resolve.Outputs;
    /// <summary>
    /// 数据操作基类。
    /// </summary>
    public abstract class DbOperateBase
    {
        /// <summary>
        /// 创建数据操作。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="type">操作数据CLR类型。</param>
        public DbOperateBase(DbContext context, Type type)
        {
            Executor = context.Executor;
            ClrType = type;
            Dependents = new HashSet<DbOperateBase>();
        }
        /// <summary>
        /// 数据操作上下文。
        /// </summary>
        public DbOperateContext Executor { get; }
        /// <summary>
        /// 操作数据类型。
        /// </summary>
        public Type ClrType { get; }
        /// <summary>
        /// 输出内容。
        /// </summary>
        internal OutputInfoBase Output { get; set; }
        /// <summary>
        /// 依赖的操作。
        /// </summary>
        public ICollection<DbOperateBase> Dependents { get; }
        /// <summary>
        /// 操作类型。
        /// </summary>
        public abstract EOperateType Type { get; }
        /// <summary>
        /// 当前操作是否会有结果。
        /// </summary>
        public virtual bool HasResult { get; }
        /// <summary>
        /// 读取数据，回写对象属性。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        internal abstract bool Read(DbDataReader reader);
        /// <summary>
        /// 生成语句。
        /// </summary>
        /// <returns>生成结果。</returns>
        internal virtual string GenerateSql()
        {
            var context = Executor.Context;
            var expression = context.Configuration.Translator.Translate(this);
            return context.Database.Generator.Generate(this, expression);
        }
    }
}