// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Translators;
    using System;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// 数据上下文配置对象。
    /// </summary>
    public class DbContextConfiguration
    {
        private readonly static Dictionary<Type, TranslateEngine> _Translators;
        private readonly static Dictionary<Type, MetadataEngine> _Metadatas;
        /// <summary>
        /// 初始化配置相关的基础数据。
        /// </summary>
        static DbContextConfiguration()
        {
            _Translators = new Dictionary<Type, TranslateEngine>();
            _Metadatas = new Dictionary<Type, MetadataEngine>();
        }
        /// <summary>
        /// 创建配置对象。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        internal DbContextConfiguration(DbContext context)
        {
            Context = context;

            var contextType = context.GetType();
            Metadata = _Metadatas.GetOrAdd(contextType, t => new MetadataEngine());
            Translator = _Translators.GetOrAdd(contextType, t => new TranslateEngine());
        }
        /// <summary>
        /// 数据上下文对象。
        /// </summary>
        public DbContext Context { get; }
        /// <summary>
        /// 元数据对象。
        /// </summary>
        public MetadataEngine Metadata { get; }
        /// <summary>
        /// <see cref="Exception"/>转换为<see cref="DbExpression"/>的翻译器。
        /// </summary>
        public TranslateEngine Translator { get; }
        /// <summary>
        /// 当前数据上下文所对应的数据库特性对象。
        /// </summary>
        public DbFeature DatabaseFeature => Context.Database.Generator.Feature;
        /// <summary>
        /// 启用乐观并发检查，默认为启用，如果<see cref="IDbAccessProvider"/>为独占数据库则会强制不可用。
        /// </summary>
        public bool EnableConcurrencyCheck
        {
            get { return _EnableConcurrencyCheck && !Context.Database.Provider.IsExclusive; }
            set { _EnableConcurrencyCheck = value; }
        }
        private bool _EnableConcurrencyCheck = true;
        /// <summary>
        /// 启用自动转换存储类型，默认为不可用。启用该选项后会导致从数据库使用
        /// <see cref="DbDataReader"/>读取数据时强制转换为目标类型。
        /// </summary>
        public bool EnableAutoConversionStorageTypes
        {
            get { return this.Metadata.AutoTypeConversion; }
            set { this.Metadata.AutoTypeConversion = value; }
        }
    }
}