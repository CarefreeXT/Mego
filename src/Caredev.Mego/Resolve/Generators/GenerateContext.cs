// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Metadatas;
    using Caredev.Mego.Resolve.Operates;
    using Caredev.Mego.Resolve.Outputs;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    /// <summary>
    /// 语句生成上下文对象。
    /// </summary>
    public class GenerateContext
    {
        private string dataSourceAlias = "a";
        private readonly IDictionary<DbExpression, ISourceFragment> _RegistedSource = new Dictionary<DbExpression, ISourceFragment>();
        private readonly IDictionary<DbExpression, ISourceFragment> _RegistedTempSource = new Dictionary<DbExpression, ISourceFragment>();
        private readonly FragmentWriterBase _FragmentWriter;
        private readonly Translators.TranslateEngine _Translator;
        /// <summary>
        /// 创建语句生成上下文。
        /// </summary>
        /// <param name="operate">操作对象。</param>
        /// <param name="generator">值生成对象。</param>
        public GenerateContext(DbOperateBase operate, SqlGeneratorBase generator)
        {
            var operateContext = operate.Executor;
            var dataContext = operateContext.Context;
            Generator = generator;
            _FragmentWriter = generator.FragmentWriter;
            var configure = dataContext.Configuration;
            Metadata = configure.Metadata;
            _Translator = configure.Translator;
            Feature = dataContext.Database.Feature;
            switch (operate.Type)
            {
                case EOperateType.InsertObjects:
                case EOperateType.InsertPropertys:
                    Data = new GenerateDataForInsert(this, operate as DbObjectsOperateBase);
                    break;
                case EOperateType.UpdateObjects:
                case EOperateType.UpdatePropertys:
                    Data = new GenerateDataForUpdate(this, operate as DbObjectsOperateBase);
                    break;
                case EOperateType.DeleteObjects:
                    Data = new GenerateDataForDelete(this, operate as DbObjectsOperateBase);
                    break;
                case EOperateType.AddRelation:
                case EOperateType.RemoveRelation:
                    Data = new GenerateDataForRelation(this, operate as DbRelationOperateBase);
                    break;
                case EOperateType.InsertStatement:
                case EOperateType.UpdateStatement:
                case EOperateType.DeleteStatement:
                    Data = new GenerateDataForStatement(this, operate as DbStatementOperateBase);
                    break;
                default:
                    Data = new GenerateData(this, operate);
                    break;
            }
        }
        /// <summary>
        /// 当前生成数据对象。
        /// </summary>
        public GenerateData Data { get; }
        /// <summary>
        /// 元数据引擎对象。
        /// </summary>
        public MetadataEngine Metadata { get; }
        /// <summary>
        /// SQL 语句生成器。
        /// </summary>
        public SqlGeneratorBase Generator { get; }
        /// <summary>
        /// 当前数据库运行时特性。
        /// </summary>
        public DbFeature Feature { get; }
        /// <summary>
        /// 获取参数名。
        /// </summary>
        /// <param name="value">参数值。</param>
        /// <returns>参数名。</returns>
        public string GetParameter(object value)
        {
            return Data.OperateCommand.AddParameter(value, "@p");
        }
        /// <summary>
        /// 获取变量名。
        /// </summary>
        /// <param name="key">键值。</param>
        /// <returns>变量索引。</returns>
        public int GetVariable(object key)
        {
            return Data.OperateCommand.GetVariableName(key);
        }
        /// <summary>
        /// 生成数据源别名。
        /// </summary>
        /// <returns>数据源别名。</returns>
        public string GetDataSourceAlias()
        {
            return GenerateAlias(ref dataSourceAlias);
        }
        /// <summary>
        /// 通过指定的表达式获取关联的存储语句片段对象。
        /// </summary>
        /// <param name="expression">获取指定表达式所关联的语句片段。</param>
        /// <returns>关联的源语句片段。</returns>
        public ISourceFragment GetSourceFragment(DbExpression expression)
        {
            if (_RegistedTempSource.TryGetValue(expression, out ISourceFragment source))
                return source;
            if (_RegistedSource.TryGetValue(expression, out source))
                return source;
            return null;
        }
        /// <summary>
        /// 获取当前输出根对象。
        /// </summary>
        /// <returns>输出对象。</returns>
        internal OutputInfoBase GetOutputRoot()
        {
            var operate = Data.Operate;
            if (operate.Output == null && operate.HasResult)
            {
                var type = operate.ClrType;
                if (type.IsPrimary())
                    operate.Output = new SingleValueOutputInfo();
                else if (type.IsObject())
                    operate.Output = CreateOutput(type);
                else if (type.IsCollection())
                {
                    var itemtype = type.ElementType();
                    if (itemtype.IsPrimary())
                        operate.Output = new MultiValueOutputInfo();
                    else
                        operate.Output = CreateOutput(type);
                }
            }
            return operate.Output;
        }
        /// <summary>
        /// 注册指定的表达式关联存储语句片段对象。
        /// </summary>
        /// <param name="expression">关联的表达式。</param>
        /// <param name="source">关联的源对象。</param>
        /// <param name="isforce">是否强制更新关联数据源。</param>
        public void RegisterSource(DbExpression expression, ISourceFragment source, bool isforce = false)
        {
            if (!_RegistedSource.ContainsKey(expression))
            {
                _RegistedSource.Add(expression, source);
            }
            else if (isforce)
            {
                _RegistedSource[expression] = source;
            }
        }
        /// <summary>
        /// 注册局部表达式关联存储语句片段对象。
        /// </summary>
        /// <param name="expression">关联的表达式。</param>
        /// <param name="source">关联的源对象。</param>
        /// <param name="action">执行方法。</param>
        public void RegisterTempSource(DbExpression expression, ISourceFragment source, Action action)
        {
            var contains = _RegistedTempSource.ContainsKey(expression);
            if (!contains)
                _RegistedTempSource.Add(expression, source);
            action();
            if (!contains)
                _RegistedTempSource.Remove(expression);
        }
        /// <summary>
        /// 翻译<see cref="System.Linq.Expressions.Expression"/>对象为<see cref="DbExpression"/>。
        /// </summary>
        /// <param name="expression">LINQ表达式。</param>
        /// <returns>数据表达式。</returns>
        internal DbExpression Translate(System.Linq.Expressions.Expression expression)
        {
            return _Translator.Translate(expression, Data.DataContext);
        }
        /// <summary>
        /// 写入语句片段。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">片段对象。</param>
        internal void WriteFragment(SqlWriter writer, ISqlFragment fragment)
        {
            _FragmentWriter.WriteFragment(writer, fragment);
        }
        /// <summary>
        /// 写入语句结束符。
        /// </summary>
        /// <param name="writer">语句写入器。</param>
        /// <param name="fragment">片段对象。</param>
        internal void WriteTerminator(SqlWriter writer, ISqlFragment fragment)
        {
            _FragmentWriter.WriteTerminator(writer, fragment);
        }
        /// <summary>
        /// 创建输出信息对象。
        /// </summary>
        /// <param name="type">输出对象类型。</param>
        /// <returns>复杂输出对象。</returns>
        internal ComplexOutputInfo CreateOutput(Type type)
        {
            bool isCollection = type.IsComplexCollection();
            Type itemType = isCollection ? type.ElementType() : type;
            var metadata = Metadata.Type(itemType);
            var fields = Utility.Array<int>(metadata.PrimaryMembers.Count, -1);
            if (isCollection)
                return new CollectionOutputInfo(metadata, fields);
            else
                return new ObjectOutputInfo(metadata, fields);
        }
        /// <summary>
        /// 生成别名。
        /// </summary>
        /// <param name="current">当前字符串。</param>
        /// <returns>返回不重复的别名。</returns>
        private string GenerateAlias(ref string current)
        {
            var result = current;
            char[] charArray = current.ToCharArray();
            charArray[0] += (char)1;
            if (charArray[0] > 'z')
            {
                if (charArray.Length == 1)
                    current = "aa";
                else
                {
                    charArray[0] = 'a';
                    charArray[1] += (char)1;
                    current = new string(charArray);
                }
            }
            else
                current = new string(charArray);
            return result;
        }
        //内部方法
        [System.Diagnostics.Conditional("DEBUG")]
        internal void VerfityOutput(ComplexOutputInfo output)
        {
            if (output != null)
            {
                Debug.Assert(output.ItemFields.Length == 0 || output.ItemFields.Any(a => a > -1));
                foreach (var item in output.Children.OfType<ComplexOutputInfo>())
                {
                    VerfityOutput(item);
                }
            }
        }
    }
}