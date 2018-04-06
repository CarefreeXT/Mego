// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Outputs
{
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using Res = Properties.Resources;
    /// <summary>
    /// 对象输出信息对象。
    /// </summary>
    public class ObjectOutputInfo : ComplexOutputInfo, ISingleOutput
    {
        /// <summary>
        /// 创建对象输出信息对象。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <param name="metadata">类型元数据。</param>
        public ObjectOutputInfo(DbDataReader reader, TypeMetadata metadata) : base(reader, metadata)
        {
        }
        /// <summary>
        /// 创建对象输出信息对象。
        /// </summary>
        /// <param name="metadata">类型元数据。</param>
        /// <param name="fields">字段索引集合。</param>
        public ObjectOutputInfo(TypeMetadata metadata, int[] fields)
            : base(metadata, fields)
        {
        }
        /// <inheritdoc/>
        public override EOutputType Type => EOutputType.Object;
        /// <summary>
        /// 对象输出选项。
        /// </summary>
        public EObjectOutputOption Option { get; set; } = EObjectOutputOption.ZeroOrOne;
        /// <summary>
        /// 获取执行结果。
        /// </summary>
        /// <param name="reader">数据读取器。</param>
        /// <returns>执行结果。</returns>
        public object GetResult(DbDataReader reader)
        {
            Initialize();
            if (!reader.HasRows)
            {
                if (Option == EObjectOutputOption.One ||
                    Option == EObjectOutputOption.OnlyOne)
                {
                    throw new OutputException(this, Res.ExceptionOutputMustHasRows);
                }
                return null;
            }
            reader.Read();
            var result = CreateItem(reader);
            if (Option == EObjectOutputOption.OnlyOne ||
                Option == EObjectOutputOption.ZeroOrOnlyOne)
            {
                if (reader.Read())
                {
                    throw new OutputException(this, Res.ExceptionOutputMustOnlyOne);
                }
            }
            return result;
        }
    }
}