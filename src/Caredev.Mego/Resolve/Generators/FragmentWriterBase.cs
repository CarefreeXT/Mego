// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Collections.Generic;
    using Res = Properties.Resources;
    /// <summary>
    /// 写入语句片段委托定义。
    /// </summary>
    /// <param name="writer">语句写入器。</param>
    /// <param name="fragment">写入的语句。</param>
    public delegate void WriteFragmentDelegate(SqlWriter writer, ISqlFragment fragment);
    /// <summary>
    /// 语句写入基类。
    /// </summary>
    public abstract partial class FragmentWriterBase
    {
        /// <summary>
        /// 创建语句片段写入器。
        /// </summary>
        /// <param name="generator"></param>
        public FragmentWriterBase(SqlGeneratorBase generator)
        {
            this.Generator = generator;

            _WriteFragmentMethods = InitialMethodsForWriteFragment();

            _WriteBinaryMethods = InitialMethodsForWriteBinary();
            _WriteScalarMethods = InitialMethodsForWriteScalar();
            _WriteAggregateMethods = InitialMethodsForWriteAggregate();

            _ClrTypeDefaultMapping = InitialClrTypeDefaultMapping();
            _ClrTypeSimpleMapping = InitialClrTypeSimpleMapping();
            _ClrTypeCustomWriteMethods = InitialCustomWriteDbDataTypeMethods();
        }
        /// <summary>
        /// 生成器引用。
        /// </summary>
        public SqlGeneratorBase Generator { get; }
        /// <summary>
        /// 写入数据库安全名称。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="name">名称。</param>
        public abstract void WriteDbName(SqlWriter writer, string name);
        /// <summary>
        /// 写入别名。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="name">名称。</param>
        public virtual void WriteAliasName(SqlWriter writer, string name)
        {
            writer.Write(" AS ");
            writer.Write(name);
        }
        /// <summary>
        /// 写入参数名称。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="name">名称。</param>
        public virtual void WriteParameterName(SqlWriter writer, string name)
        {
            writer.Write('@');
            writer.Write(name);
        }
        /// <summary>
        /// 写入数据库对象名称。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构名。</param>
        public virtual void WriteDbObject(SqlWriter writer, string name, string schema)
        {
            if (string.IsNullOrEmpty(schema))
            {
                schema = writer.Context.Feature.DefaultSchema;
            }
            if (!string.IsNullOrEmpty(schema))
            {
                WriteDbName(writer, schema);
                writer.Write('.');
            }
            WriteDbName(writer, name);
        }
        /// <summary>
        /// 写入语句终止符。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">当前语句。</param>
        public virtual void WriteTerminator(SqlWriter writer, ISqlFragment fragment) => writer.Write(';');
    }
    public partial class FragmentWriterBase
    {
        private readonly IDictionary<Type, string> _ClrTypeDefaultMapping;
        private readonly IDictionary<Type, string> _ClrTypeSimpleMapping;
        private readonly IDictionary<Type, Action<SqlWriter, ColumnMetadata>> _ClrTypeCustomWriteMethods;
        /// <summary>
        /// 初始化CLR类型到数据库类型默认映射。
        /// </summary>
        /// <returns>返回映射字典。</returns>
        protected abstract IDictionary<Type, string> InitialClrTypeDefaultMapping();
        /// <summary>
        /// 初始化CLR类型到数据库类型映射，该类型没有附加属性。
        /// </summary>
        /// <returns>返回映射字典。</returns>
        protected abstract IDictionary<Type, string> InitialClrTypeSimpleMapping();
        /// <summary>
        /// 初始化自定义写入数据类型方法集。
        /// </summary>
        /// <returns>类型与写入方法的映射字典。</returns>
        protected virtual IDictionary<Type, Action<SqlWriter, ColumnMetadata>> InitialCustomWriteDbDataTypeMethods()
        {
            return new Dictionary<Type, Action<SqlWriter, ColumnMetadata>>()
            {
                { typeof(byte[]), WriteDbDataTypeForByteArray },
                { typeof(string), WriteDbDataTypeForString },
                { typeof(decimal), WriteDbDataTypeForDeciaml },
            };
        }
        /// <summary>
        /// 写入<see cref="string"/>的数据类型。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="column">数据列元数据。</param>
        protected virtual void WriteDbDataTypeForString(SqlWriter writer, ColumnMetadata column)
        {
            var strattr = column.GetProperty<StringAttribute>();
            if (strattr == null)
            {
                WriteDbDataType(writer, typeof(string));
            }
            else
            {
                if (strattr.IsUnicode) writer.Write('N');
                if (!strattr.IsFixed) writer.Write("VAR");
                writer.Write("CHAR(");
                if (strattr.Length > 0)
                    writer.Write(strattr.Length);
                else
                    writer.Write("MAX");
                writer.Write(')');
            }
        }
        /// <summary>
        /// 写入<see cref="byte"/>数组的数据类型。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="column">数据列元数据。</param>
        protected virtual void WriteDbDataTypeForByteArray(SqlWriter writer, ColumnMetadata column)
        {
            var lengthattr = column.GetProperty<LengthAttribute>();
            if (lengthattr == null || lengthattr.Length <= 0)
            {
                WriteDbDataType(writer, typeof(byte[]));
            }
            else
            {
                if (!lengthattr.IsFixed) writer.Write("VAR");
                writer.Write("BINARY(");
                writer.Write(lengthattr.Length);
                writer.Write(')');
            }
        }
        /// <summary>
        /// 写入<see cref="decimal"/>的数据类型。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="column">数据列元数据。</param>
        protected virtual void WriteDbDataTypeForDeciaml(SqlWriter writer, ColumnMetadata column)
        {
            var precision = column.GetProperty<PrecisionAttribute>();
            if (precision == null)
            {
                WriteDbDataType(writer, typeof(decimal));
            }
            else
            {
                writer.Write("DECIMAL");
                writer.Write('(');
                writer.Write(precision.Precision);
                writer.Write(',');
                writer.Write(precision.Scale);
                writer.Write(')');
            }
        }
        /// <summary>
        /// 根据数据列元数据写入相应数据类型。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="column">数据列元数据</param>
        public virtual void WriteDbDataType(SqlWriter writer, ColumnMetadata column)
        {
            if (!string.IsNullOrEmpty(column.TypeName))
            {
                writer.Write(' ');
                writer.Write(column.TypeName);
                return;
            }
            var type = column.Member.PropertyType.GetRealType();
            if (_ClrTypeSimpleMapping.TryGetValue(type, out string value))
            {
                writer.Write(value);
                return;
            }
            else if (_ClrTypeCustomWriteMethods.TryGetValue(type, out Action<SqlWriter, ColumnMetadata> method))
            {
                method(writer, column);
            }
        }
        /// <summary>
        /// 根据CLR类型写入相应数据类型。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="type">CLR类型。</param>
        public virtual void WriteDbDataType(SqlWriter writer, Type type)
        {
            if (!_ClrTypeDefaultMapping.TryGetValue(type.GetRealType(), out string value))
            {
                throw new NotSupportedException(string.Format(Res.NotSupportedWriteDbDataType, type));
            }
            writer.Write(value);
        }
    }
}