// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Operates;
    using System;
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">插入表达式。</param>
        /// <returns>生成结果。</returns>
        protected virtual SqlFragment GenerateForStatement(GenerateContext context, DbExpression content)
        {
            var operate = context.Data.Operate;
            switch (operate.Type)
            {
                case EOperateType.InsertStatement: return GenerateForInsertStatement(context, content);
                case EOperateType.UpdateStatement: return GenerateForUpdateStatement(context, content);
                case EOperateType.DeleteStatement: return GenerateForDeleteStatement(context, content);
            }
            throw new NotImplementedException();
        }
    }
}