// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    //Binary 二元操作写入方法
    public partial class FragmentWriterBase
    {
        private readonly IDictionary<EBinaryKind, WriteFragmentDelegate> _WriteBinaryMethods;
        /// <summary>
        /// 初始化写入二元操作的方法映射。
        /// </summary>
        /// <returns>二元操作的方法映射字典。</returns>
        protected virtual IDictionary<EBinaryKind, WriteFragmentDelegate> InitialMethodsForWriteBinary()
        {
            return new Dictionary<EBinaryKind, WriteFragmentDelegate>()
            {
                { EBinaryKind.Equal, WriteBinaryForSimple },
                { EBinaryKind.NotEqual, WriteBinaryForSimple },
                { EBinaryKind.GreaterThanOrEqual, WriteBinaryForSimple },
                { EBinaryKind.GreaterThan, WriteBinaryForSimple },
                { EBinaryKind.LessThan, WriteBinaryForSimple },
                { EBinaryKind.LessThanOrEqual, WriteBinaryForSimple },
                { EBinaryKind.Add, WriteBinaryForSimple },
                { EBinaryKind.Subtract, WriteBinaryForSimple },
                { EBinaryKind.Multiply, WriteBinaryForSimple },
                { EBinaryKind.Divide, WriteBinaryForSimple },
                { EBinaryKind.Modulo, WriteBinaryForSimple },
                { EBinaryKind.And, WriteBinaryForSimple },
                { EBinaryKind.Or, WriteBinaryForSimple },
                { EBinaryKind.ExclusiveOr, WriteBinaryForSimple },
                { EBinaryKind.Power, WriteBinaryForPower },
                { EBinaryKind.Assign, WriteBinaryForAssign },
            };
        }

        private void WriteBinaryForPower(SqlWriter writer, ISqlFragment fragment)
        {
            var binary = (BinaryFragment)fragment;
            writer.Write("POWER(");
            binary.Left.WriteSql(writer);
            writer.Write(", ");
            binary.Right.WriteSql(writer);
            writer.Write(')');
        }
        private void WriteBinaryForAssign(SqlWriter writer, ISqlFragment fragment)
        {
            var binary = (BinaryFragment)fragment;
            writer.Write("SET ");
            binary.Left.WriteSql(writer);
            writer.Write(" = ");
            binary.Right.WriteSql(writer);
        }
        private void WriteBinaryForSimple(SqlWriter writer, ISqlFragment fragment)
        {
            var binary = (BinaryFragment)fragment;
            binary.Left.WriteSql(writer);
            switch (binary.Kind)
            {
                case EBinaryKind.AndAlso: break;
                case EBinaryKind.OrElse: break;

                case EBinaryKind.Add: writer.Write(" + "); break;
                case EBinaryKind.Subtract: writer.Write(" - "); break;
                case EBinaryKind.Multiply: writer.Write(" * "); break;
                case EBinaryKind.Divide: writer.Write(" / "); break;
                case EBinaryKind.Modulo: writer.Write(" % "); break;
                case EBinaryKind.And: writer.Write(" & "); break;
                case EBinaryKind.Or: writer.Write(" | "); break;
                case EBinaryKind.ExclusiveOr: writer.Write(" ^ "); break;

                case EBinaryKind.Equal: writer.Write(" = "); break;
                case EBinaryKind.NotEqual: writer.Write(" != "); break;
                case EBinaryKind.GreaterThanOrEqual: writer.Write(" >= "); break;
                case EBinaryKind.GreaterThan: writer.Write(" > "); break;
                case EBinaryKind.LessThan: writer.Write(" < "); break;
                case EBinaryKind.LessThanOrEqual: writer.Write(" <= "); break;
            }
            binary.Right.WriteSql(writer);
        }
    }
    //Scalar 标量函数写入方法
    public partial class FragmentWriterBase
    {
        private readonly IDictionary<MemberInfo, WriteFragmentDelegate> _WriteScalarMethods;
        /// <summary>
        /// 初始化写入标量函数的方法映射。
        /// </summary>
        /// <returns>标量函数的方法映射字典。</returns>
        protected virtual IDictionary<MemberInfo, WriteFragmentDelegate> InitialMethodsForWriteScalar()
        {
            return new Dictionary<MemberInfo, WriteFragmentDelegate>()
            {
                { SupportMembers.Enumerable.Contains, WriteScalarForContains },
            };
        }
        /// <summary>
        /// 写入<see cref="SupportMembers.Queryable.Contains"/>方法。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        protected virtual void WriteScalarForContains(SqlWriter writer, ISqlFragment fragment)
        {
            var scalar = (ScalarFragment)fragment;
            scalar.Arguments[1].WriteSql(writer);
            writer.Write(" IN (");
            scalar.Arguments[0].WriteSql(writer);
            writer.Write(')');
        }
        /// <summary>
        /// 写入系统标量函数通过方法
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragemnt">当前语句。</param>
        protected void WriteScalarForSystemFunction(SqlWriter writer, ISqlFragment fragemnt)
        {
            var scalar = (ScalarFragment)fragemnt;
            WriteScalarForSystemFunction(writer, fragemnt, scalar.Function.Name.ToUpper());
        }
        /// <summary>
        /// 写入系统标量函数通过方法
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragemnt">当前语句。</param>
        /// <param name="name">函数名称。</param>
        protected void WriteScalarForSystemFunction(SqlWriter writer, ISqlFragment fragemnt, string name)
        {
            var scalar = (ScalarFragment)fragemnt;
            writer.Write(name);
            writer.Write('(');
            scalar.Arguments.ForEach(() => writer.Write(','), a => a.WriteSql(writer));
            writer.Write(')');
        }
    }
    //Aggregate 汇总函数写入方法
    public partial class FragmentWriterBase
    {
        private readonly IDictionary<MemberInfo, WriteFragmentDelegate> _WriteAggregateMethods;
        /// <summary>
        /// 初始化写入聚合函数的方法映射。
        /// </summary>
        /// <returns>聚合函数的方法映射字典。</returns>
        protected virtual IDictionary<MemberInfo, WriteFragmentDelegate> InitialMethodsForWriteAggregate()
        {
            var result = new Dictionary<MemberInfo, WriteFragmentDelegate>()
            {
                { SupportMembers.Queryable.Count1, WriteAggregateForCount },
                { SupportMembers.Enumerable.Count1, WriteAggregateForCount },
                { SupportMembers.Queryable.LongCount1, WriteAggregateForLongCount },
                { SupportMembers.Enumerable.LongCount1, WriteAggregateForLongCount },
                { SupportMembers.Queryable.Max2, WriteAggregateForSimple },
                { SupportMembers.Queryable.Min2, WriteAggregateForSimple },
            };
            foreach (var s in SupportMembers.Queryable.Sum2) result.Add(s, WriteAggregateForSimple);
            foreach (var s in SupportMembers.Enumerable.Sum2) result.Add(s, WriteAggregateForSimple);
            foreach (var a in SupportMembers.Enumerable.Max2) result.Add(a, WriteAggregateForSimple);
            foreach (var a in SupportMembers.Enumerable.Min2) result.Add(a, WriteAggregateForSimple);
            foreach (var a in SupportMembers.Queryable.Average2) result.Add(a, WriteAggregateForAverage);
            foreach (var a in SupportMembers.Enumerable.Average2) result.Add(a, WriteAggregateForAverage);
            return result;
        }

        private void WriteAggregateForCount(SqlWriter writer, ISqlFragment fragment) => writer.Write("COUNT(1)");
        private void WriteAggregateForLongCount(SqlWriter writer, ISqlFragment fragment) => writer.Write("COUNT_BIG(1)");
        private void WriteAggregateForAverage(SqlWriter writer, ISqlFragment fragment) => WriteAggregateForSimple(writer, fragment, "AVG");
        private void WriteAggregateForSimple(SqlWriter writer, ISqlFragment fragment)
        {
            var aggregate = (AggregateFragment)fragment;
            writer.Write(aggregate.Function.Name.ToUpper());
            writer.Write('(');
            aggregate.Arguments.ForEach(() => writer.Write(", "), f => f.WriteSql(writer));
            writer.Write(')');
        }
        private void WriteAggregateForSimple(SqlWriter writer, ISqlFragment fragment, string name)
        {
            var aggregate = (AggregateFragment)fragment;
            writer.Write(name);
            writer.Write('(');
            aggregate.Arguments.ForEach(() => writer.Write(", "), f => f.WriteSql(writer));
            writer.Write(')');
        }
    }
}