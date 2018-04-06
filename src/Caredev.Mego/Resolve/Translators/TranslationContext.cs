// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Translators
{
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Res = Properties.Resources;
    /// <summary>
    /// 翻译上下文对象。
    /// </summary>
    public class TranslationContext
    {
        /// <summary>
        /// 创建翻译上下文对象。
        /// </summary>
        /// <param name="context">数据上下文对象。</param>
        public TranslationContext(DbContext context)
        {
            Context = context;
            Parameters = new Dictionary<ParameterExpression, DbExpression>();
        }
        /// <summary>
        /// 当前的数据上下文对象。
        /// </summary>
        public DbContext Context { get; private set; }
        /// <summary>
        /// 当前上下文中所有的参数，只有当在解析Lambda表达式时才会存在参数。
        /// </summary>
        public Dictionary<ParameterExpression, DbExpression> Parameters { get; private set; }
        /// <summary>
        /// 解析Lambda表达式，收集所有参数并进入表达式主体部分。
        /// </summary>
        /// <typeparam name="TResult">返回类型。</typeparam>
        /// <param name="action">执行内容。</param>
        /// <param name="exp">当前表达式。</param>
        /// <param name="dbexps">当前执行参数。</param>
        /// <returns>返回表达式结果</returns>
        public TResult Lambda<TResult>(Func<Expression, TResult> action, Expression exp, params DbExpression[] dbexps)
        {
            LambdaExpression lambda;
            switch (exp.NodeType)
            {
                case ExpressionType.Quote:
                    var unary = exp as UnaryExpression;
                    if (unary == null)
                    {
                        throw new TranslateException(exp, Res.ExceptionIsNotUnaryExpression);
                    }
                    if (unary.Operand.NodeType != ExpressionType.Lambda)
                    {
                        throw new TranslateException(exp, Res.ExceptionNotFoundLambdaExpression);
                    }
                    lambda = (LambdaExpression)unary.Operand;
                    break;
                case ExpressionType.Lambda:
                    lambda = (LambdaExpression)exp;
                    break;
                default:
                    throw new TranslateException(exp, string.Format(Res.NotSupportedParseLambdaNodeType, exp.NodeType));
            }
            for (int i = 0; i < lambda.Parameters.Count; i++)
            {
                Parameters.Add(lambda.Parameters[i], dbexps[i]);
            }
            var result = action(lambda.Body);
            for (int i = 0; i < lambda.Parameters.Count; i++)
            {
                Parameters.Remove(lambda.Parameters[i]);
            }
            return result;
        }
    }
}