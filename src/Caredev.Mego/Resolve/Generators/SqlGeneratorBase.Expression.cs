// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Collections;
    using Res = Properties.Resources;
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 创建表达式语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">所在数据源。</param>
        /// <param name="source">检索表达式。</param>
        /// <returns>创建结果。</returns>
        public IExpressionFragment CreateExpression(GenerateContext context, DbExpression expression, ISourceFragment source)
        {
            if (_CreateExpressionMethods.TryGetValue(expression.ExpressionType, out CreateExpressionDelegate method))
                return method(context, expression, source);
            if (_RetrievalMemberMethods.TryGetValue(expression.ExpressionType, out RetrievalMemberDelegate method3))
                return method3(context, source, expression, null, true);
            throw new NotSupportedException(Res.NotSupportedGenerateExpression);
        }
        /// <summary>
        /// 根据类型<see cref="DbUnaryExpression"/>表达式创建表达式语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">所在数据源。</param>
        /// <param name="source">检索表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual IExpressionFragment CreateExpressionForUnary(GenerateContext context, DbExpression expression, ISourceFragment source)
        {
            var unary = (DbUnaryExpression)expression;
            return new UnaryFragment(context, unary.Kind, unary.Type)
            {
                Expresssion = source.CreateExpression(unary.Expression)
            };
        }
        /// <summary>
        /// 根据类型<see cref="DbBinaryExpression"/>表达式创建表达式语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">所在数据源。</param>
        /// <param name="source">检索表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual IExpressionFragment CreateExpressionForBinary(GenerateContext context, DbExpression expression, ISourceFragment source)
        {
            var binary = (DbBinaryExpression)expression;
            if (binary.Kind == EBinaryKind.AndAlso || binary.Kind == EBinaryKind.OrElse)
            {
                return source.CreateExpression(binary.Left).Merge(binary.Kind, source.CreateExpression(binary.Right));
            }
            else
            {
                return new BinaryFragment(context, binary.Kind)
                {
                    Left = source.CreateExpression(binary.Left),
                    Right = source.CreateExpression(binary.Right)
                };
            }
        }
        /// <summary>
        /// 根据类型<see cref="DbScalarFunctionExpression"/>表达式创建表达式语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">所在数据源。</param>
        /// <param name="source">检索表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual IExpressionFragment CreateExpressionForScalarFunction(GenerateContext context, DbExpression expression, ISourceFragment source)
        {
            var scalar = (DbScalarFunctionExpression)expression;
            var result = new ScalarFragment(context)
            {
                Function = scalar.Function
            };
            foreach (var arg in scalar.Arguments)
                result.Arguments.Add(source.CreateExpression(arg));
            return result;
        }
        /// <summary>
        /// 根据类型<see cref="DbConstantExpression"/>表达式创建表达式语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">所在数据源。</param>
        /// <param name="source">检索表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual IExpressionFragment CreateExpressionForConstant(GenerateContext context, DbExpression expression, ISourceFragment source)
        {
            var constant = (DbConstantExpression)expression;
            if (constant.Value != null && constant.Value.GetType().IsCollection())
            {
                return new ConstantListFragment(context, constant.Value as IEnumerable);
            }
            return new ConstantFragment(context, constant.Value);
        }
        /// <summary>
        /// 根据类型<see cref="DbJoinKeyPairExpression"/>表达式创建表达式语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">所在数据源。</param>
        /// <param name="source">检索表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual IExpressionFragment CreateExpressionForJoinKeyPair(GenerateContext context, DbExpression expression, ISourceFragment source)
        {
            var content = (DbJoinKeyPairExpression)expression;
            return new BinaryFragment(context, EBinaryKind.Equal)
            {
                Left = source.CreateExpression(content.Left),
                Right = source.CreateExpression(content.Right),
            };
        }
        /// <summary>
        /// 根据类型<see cref="DbOrderExpression"/>表达式创建表达式语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="expression">所在数据源。</param>
        /// <param name="source">检索表达式。</param>
        /// <returns>创建结果。</returns>
        protected virtual IExpressionFragment CreateExpressionForOrder(GenerateContext context, DbExpression expression, ISourceFragment source)
        {
            var sort = (DbOrderExpression)expression;
            var member = source.RetrievalMember(sort.Member);
            return new SortFragment(context, member, sort.Kind);
        }
    }
}
