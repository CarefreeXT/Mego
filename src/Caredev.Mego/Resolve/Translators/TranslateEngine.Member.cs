// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
using Caredev.Mego.Common;
using Caredev.Mego.Resolve.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
namespace Caredev.Mego.Resolve.Translators
{
    using UnitTypeTranslateMethodType = System.Func<TranslationContext, MethodCallExpression, DbUnitTypeExpression>;
    using UnitItemTypeTranslateMethodType = System.Func<TranslationContext, MethodCallExpression, MethodInfo, DbExpression>;
    using ValueTypeTranslateMethodType = System.Func<TranslationContext, MethodCallExpression, MethodInfo, DbExpression>;
    using ValueTypeTranslatePropertyType = System.Func<TranslationContext, MemberExpression, DbExpression>;
    public partial class TranslateEngine
    {
        private Dictionary<MethodInfo, UnitTypeTranslateMethodType> UnitTypeMethods;
        private Dictionary<MethodInfo, UnitItemTypeTranslateMethodType> UnitItemTypeMethods;
        private Dictionary<MemberInfo, ValueTypeTranslateMethodType> ValueTypeMethods;
        private Dictionary<MemberInfo, ValueTypeTranslatePropertyType> ValueTypePropertys;
        private Dictionary<ExpressionType, EBinaryKind> BinaryKindMap;
        private Dictionary<ExpressionType, EUnaryKind> UnaryKindMap;
        private void InitialMethods()
        {
            UnitTypeMethods = new Dictionary<MethodInfo, UnitTypeTranslateMethodType>()
            {
                { SupportMembers.Enumerable.Include1, Include1Translate },
                { SupportMembers.Enumerable.Include2, Include2Translate },
                { SupportMembers.Enumerable.Where, WhereTranslate },

                { SupportMembers.Queryable.Include1, Include1Translate },
                { SupportMembers.Queryable.Include2, Include2Translate },
                { SupportMembers.Queryable.Where, WhereTranslate },
                { SupportMembers.Queryable.Select, SelectTranslate },
                { SupportMembers.Queryable.SelectMany, SelectManyTranslate },
                { SupportMembers.Queryable.Join, JoinTranslate },
                { SupportMembers.Queryable.GroupJoin, GroupJoinTranslate },
                { SupportMembers.Queryable.GroupBy2, GroupByTranslate },
                { SupportMembers.Queryable.GroupBy3, GroupByTranslate },
                { SupportMembers.Queryable.OrderBy, OrderByTranslate },
                { SupportMembers.Queryable.OrderByDescending, OrderByDescendingTranslate },
                { SupportMembers.Queryable.ThenBy, ThenByTranslate },
                { SupportMembers.Queryable.ThenByDescending, ThenByDescendingTranslate },
                { SupportMembers.Queryable.Skip, SkipTranslate },
                { SupportMembers.Queryable.Take, TakeTranslate },
                { SupportMembers.Queryable.Distinct, DistinctTranslate },

                { SupportMembers.Queryable.Union, UnionTranslate },
                { SupportMembers.Queryable.Intersect, IntersectTranslate },
                { SupportMembers.Queryable.Except, ExceptTranslate },
                { SupportMembers.Queryable.Concat, ConcatTranslate },

                { SupportMembers.Enumerable.DefaultIfEmpty1, DefaultIfEmptyTranslate },
                { SupportMembers.Enumerable.DefaultIfEmpty2, DefaultIfEmptyTranslate },
            };
            UnitItemTypeMethods = new Dictionary<MethodInfo, UnitItemTypeTranslateMethodType>()
            {
                { SupportMembers.Queryable.First1, RetrievalFunctionTranslate },
                { SupportMembers.Queryable.FirstOrDefault1, RetrievalFunctionTranslate },
                { SupportMembers.Queryable.Last1, RetrievalFunctionTranslate },
                { SupportMembers.Queryable.LastOrDefault1, RetrievalFunctionTranslate },
                { SupportMembers.Queryable.Single1, RetrievalFunctionTranslate },
                { SupportMembers.Queryable.SingleOrDefault1, RetrievalFunctionTranslate },

                { SupportMembers.Queryable.First2, RetrievalFunctionTranslate },
                { SupportMembers.Queryable.FirstOrDefault2, RetrievalFunctionTranslate },
                { SupportMembers.Queryable.Last2, RetrievalFunctionTranslate },
                { SupportMembers.Queryable.LastOrDefault2, RetrievalFunctionTranslate },
                { SupportMembers.Queryable.Single2, RetrievalFunctionTranslate },
                { SupportMembers.Queryable.SingleOrDefault2, RetrievalFunctionTranslate },

                { SupportMembers.Queryable.ElementAt, RetrievalFunctionElementAtTranslate },
                { SupportMembers.Queryable.ElementAtOrDefault, RetrievalFunctionElementAtTranslate },

                { SupportMembers.Queryable.Any1, JudgeFunctionTranslate },
                { SupportMembers.Queryable.Any2, JudgeFunctionTranslate },
                { SupportMembers.Queryable.All, JudgeFunctionTranslate },

                { SupportMembers.Queryable.Count1, AggregateFunctionTranslate },
                { SupportMembers.Queryable.Count2, AggregateFunctionTranslate },
                { SupportMembers.Queryable.LongCount1, AggregateFunctionTranslate },
                { SupportMembers.Queryable.LongCount2, AggregateFunctionTranslate },

                { SupportMembers.Queryable.Max1, AggregateFunctionTranslate },
                { SupportMembers.Queryable.Max2, AggregateFunctionTranslate },
                { SupportMembers.Queryable.Min1, AggregateFunctionTranslate },
                { SupportMembers.Queryable.Min2, AggregateFunctionTranslate },

                { SupportMembers.Queryable.Sum1[0], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum1[1], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum1[2], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum1[3], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum1[4], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum1[5], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum1[6], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum1[7], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum1[8], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum1[9], AggregateFunctionTranslate },

                { SupportMembers.Queryable.Sum2[0], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum2[1], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum2[2], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum2[3], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum2[4], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum2[5], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum2[6], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum2[7], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum2[8], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Sum2[9], AggregateFunctionTranslate },

                { SupportMembers.Queryable.Average1[0], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average1[1], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average1[2], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average1[3], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average1[4], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average1[5], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average1[6], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average1[7], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average1[8], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average1[9], AggregateFunctionTranslate },

                { SupportMembers.Queryable.Average2[0], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average2[1], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average2[2], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average2[3], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average2[4], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average2[5], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average2[6], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average2[7], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average2[8], AggregateFunctionTranslate },
                { SupportMembers.Queryable.Average2[9], AggregateFunctionTranslate },
            };
            ValueTypeMethods = new Dictionary<MemberInfo, ValueTypeTranslateMethodType>()
            {
                { SupportMembers.Enumerable.First1          , RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.FirstOrDefault1 , RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.Last1           , RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.LastOrDefault1  , RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.Single1         , RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.SingleOrDefault1, RetrievalFunctionInlineTranslate },

                { SupportMembers.Enumerable.First2, RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.FirstOrDefault2, RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.Last2, RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.LastOrDefault2, RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.Single2, RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.SingleOrDefault2, RetrievalFunctionInlineTranslate },
                { SupportMembers.Enumerable.ElementAt, RetrievalFunctionElementAtInlineTranslate },
                { SupportMembers.Enumerable.ElementAtOrDefault, RetrievalFunctionElementAtInlineTranslate },

                { SupportMembers.Enumerable.Any1, JudgeFunctionInlineTranslate },
                { SupportMembers.Enumerable.Any2, JudgeFunctionInlineTranslate },
                { SupportMembers.Enumerable.All, JudgeFunctionInlineTranslate },

                { SupportMembers.Enumerable.Count1, AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Count2, AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.LongCount1, AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.LongCount2, AggregateFunctionInlineTranslate },

                { SupportMembers.Enumerable.Max1[0], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max1[1], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max1[2], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max1[3], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max1[4], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max1[5], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max1[6], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max1[7], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max1[8], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max1[9], AggregateFunctionInlineTranslate },

                { SupportMembers.Enumerable.Max2[0], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[1], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[2], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[3], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[4], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[5], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[6], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[7], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[8], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[9], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[10], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Max2[11], AggregateFunctionInlineTranslate },

                { SupportMembers.Enumerable.Min1[0], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min1[1], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min1[2], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min1[3], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min1[4], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min1[5], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min1[6], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min1[7], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min1[8], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min1[9], AggregateFunctionInlineTranslate },

                { SupportMembers.Enumerable.Min2[0], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[1], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[2], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[3], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[4], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[5], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[6], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[7], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[8], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[9], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[10], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Min2[11], AggregateFunctionInlineTranslate },

                { SupportMembers.Enumerable.Sum1[0], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum1[1], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum1[2], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum1[3], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum1[4], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum1[5], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum1[6], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum1[7], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum1[8], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum1[9], AggregateFunctionInlineTranslate },

                { SupportMembers.Enumerable.Sum2[0], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum2[1], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum2[2], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum2[3], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum2[4], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum2[5], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum2[6], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum2[7], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum2[8], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Sum2[9], AggregateFunctionInlineTranslate },

                { SupportMembers.Enumerable.Average1[0], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average1[1], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average1[2], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average1[3], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average1[4], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average1[5], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average1[6], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average1[7], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average1[8], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average1[9], AggregateFunctionInlineTranslate },

                { SupportMembers.Enumerable.Average2[0], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average2[1], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average2[2], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average2[3], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average2[4], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average2[5], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average2[6], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average2[7], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average2[8], AggregateFunctionInlineTranslate },
                { SupportMembers.Enumerable.Average2[9], AggregateFunctionInlineTranslate },
                { SupportMembers.Math.Abs[0],     ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Abs[1],     ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Abs[2],     ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Abs[3],     ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Abs[4],     ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Abs[5],     ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Abs[6],     ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Ceiling[0], ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Ceiling[1], ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Floor[0],   ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Floor[1],   ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Pow,        ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Sign[0],    ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Sign[1],    ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Sign[2],    ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Sign[3],    ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Sign[4],    ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Sign[5],    ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Sign[6],    ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Exp,        ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Log[0],     ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Log[1],     ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Log10,      ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Sin,        ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Cos,        ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Tan,        ScalarFunctionInlineTranslate },
                { SupportMembers.Math.Sqrt,       ScalarFunctionInlineTranslate },

                { SupportMembers.Enumerable.Contains       , ScalarFunctionInlineTranslate },

                { SupportMembers.String.IsNullOrEmpty      , ScalarFunctionInlineTranslate },
#if !NET35
                { SupportMembers.String.IsNullOrWhiteSpace , ScalarFunctionInlineTranslate },  
#endif
                { SupportMembers.String.Substring1         , ScalarFunctionInlineTranslate },
                { SupportMembers.String.Substring2         , ScalarFunctionInlineTranslate },
                { SupportMembers.String.Contains           , ScalarFunctionInlineTranslate },
                { SupportMembers.String.Replace            , ScalarFunctionInlineTranslate },
                { SupportMembers.String.Concat             , ScalarFunctionInlineTranslate },
                { SupportMembers.String.Compare            , ScalarFunctionInlineTranslate },
                { SupportMembers.String.StartsWith         , ScalarFunctionInlineTranslate },
                { SupportMembers.String.EndsWith           , ScalarFunctionInlineTranslate },
                { SupportMembers.String.Trim               , ScalarFunctionInlineTranslate },
                { SupportMembers.String.TrimStart          , ScalarFunctionInlineTranslate },
                { SupportMembers.String.TrimEnd            , ScalarFunctionInlineTranslate },
                { SupportMembers.String.ToUpper            , ScalarFunctionInlineTranslate },
                { SupportMembers.String.ToLower            , ScalarFunctionInlineTranslate },

                { SupportMembers.DateTime.AddDays         , ScalarFunctionInlineTranslate },
                { SupportMembers.DateTime.AddHours        , ScalarFunctionInlineTranslate },
                { SupportMembers.DateTime.AddMilliseconds , ScalarFunctionInlineTranslate },
                { SupportMembers.DateTime.AddMinutes      , ScalarFunctionInlineTranslate },
                { SupportMembers.DateTime.AddMonths       , ScalarFunctionInlineTranslate },
                { SupportMembers.DateTime.AddSeconds      , ScalarFunctionInlineTranslate },
                { SupportMembers.DateTime.AddYears        , ScalarFunctionInlineTranslate },

                { SupportMembers.DbFunctions.GetIdentity  , ScalarFunctionInlineTranslate },

                { SupportMembers.Guid.NewGuid        , ScalarFunctionInlineTranslate },
            };
            ValueTypePropertys = new Dictionary<MemberInfo, ValueTypeTranslatePropertyType>()
            {
                { SupportMembers.String.Length,      ScalarPropertyTranslate },
                { SupportMembers.DateTime.Now,       ScalarPropertyTranslate },
                { SupportMembers.DateTime.UtcNow,    ScalarPropertyTranslate },
                { SupportMembers.DateTime.Year,      ScalarPropertyTranslate },
                { SupportMembers.DateTime.Month,     ScalarPropertyTranslate },
                { SupportMembers.DateTime.Day,       ScalarPropertyTranslate },
                { SupportMembers.DateTime.Hour,      ScalarPropertyTranslate },
                { SupportMembers.DateTime.Minute,    ScalarPropertyTranslate },
                { SupportMembers.DateTime.Second,    ScalarPropertyTranslate },
                { SupportMembers.DateTime.DayOfYear, ScalarPropertyTranslate },
            };
            BinaryKindMap = new Dictionary<ExpressionType, EBinaryKind>()
            {
                { ExpressionType.Add               ,EBinaryKind.Add               },
                { ExpressionType.Divide            ,EBinaryKind.Divide            },
                { ExpressionType.Modulo            ,EBinaryKind.Modulo            },
                { ExpressionType.Multiply          ,EBinaryKind.Multiply          },
                { ExpressionType.Power             ,EBinaryKind.Power             },
                { ExpressionType.Subtract          ,EBinaryKind.Subtract          },
                { ExpressionType.And               ,EBinaryKind.And               },
                { ExpressionType.Or                ,EBinaryKind.Or                },
                { ExpressionType.ExclusiveOr       ,EBinaryKind.ExclusiveOr       },
                { ExpressionType.LeftShift         ,EBinaryKind.LeftShift         },
                { ExpressionType.RightShift        ,EBinaryKind.RightShift        },
                { ExpressionType.AndAlso           ,EBinaryKind.AndAlso           },
                { ExpressionType.OrElse            ,EBinaryKind.OrElse            },
                { ExpressionType.Equal             ,EBinaryKind.Equal             },
                { ExpressionType.NotEqual          ,EBinaryKind.NotEqual          },
                { ExpressionType.GreaterThanOrEqual,EBinaryKind.GreaterThanOrEqual},
                { ExpressionType.GreaterThan       ,EBinaryKind.GreaterThan       },
                { ExpressionType.LessThan          ,EBinaryKind.LessThan          },
                { ExpressionType.LessThanOrEqual   ,EBinaryKind.LessThanOrEqual   }
            };
            UnaryKindMap = new Dictionary<ExpressionType, EUnaryKind>()
            {
                { ExpressionType.Not, EUnaryKind.Not },
                { ExpressionType.UnaryPlus, EUnaryKind.UnaryPlus },
                { ExpressionType.Negate, EUnaryKind.Negate },
                { ExpressionType.NegateChecked, EUnaryKind.Negate },
                { ExpressionType.Convert, EUnaryKind.Convert },
                { ExpressionType.ConvertChecked, EUnaryKind.Convert },
                { ExpressionType.TypeAs, EUnaryKind.Convert },
            };
        }
    }
}