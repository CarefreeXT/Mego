// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Fragments
{
    /// <summary>
    /// 对象名称语句片段。
    /// </summary>
    public class ObjectNameFragment : SqlFragment, INameSchemaFragment
    {
        /// <summary>
        /// 创建对象名称。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="name">对象名称。</param>
        /// <param name="schema">架构名。</param>
        public ObjectNameFragment(GenerateContext context, string name, string schema)
            : base(context)
        {
            Name = name;
            Schema = schema;
        }
        /// <summary>
        /// 对象名称。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 架构名。
        /// </summary>
        public string Schema { get; }
        /// <summary>
        /// 名称种类。
        /// </summary>
        public EDbNameKind NameKind => EDbNameKind.NameSchema;
    }
    /// <summary>
    /// 数据库安全名称语句片段。 
    /// </summary>
    public class DbNameFragment : SqlFragment, INameFragment
    {
        /// <summary>
        /// 创建对象名称。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="name">对象名称。</param>
        public DbNameFragment(GenerateContext context, string name)
            : base(context)
        {
            Name = name;
        }
        /// <summary>
        /// 对象名称。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 名称种类。
        /// </summary>
        public EDbNameKind NameKind => EDbNameKind.NameSchema;
    }
    /// <summary>
    /// 临时表名语句片段。
    /// </summary>
    public class TempTableNameFragment : SqlFragment, INameFragment
    {
        /// <summary>
        /// 创建临时表名。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="name">自定义表名。</param>
        public TempTableNameFragment(GenerateContext context, string name = null)
            : base(context)
        {
            _Name = name;
        }
        /// <summary>
        /// 对象名称。
        /// </summary>
        public string Name
        {
            get
            {
                if (_Name == null)
                {
                    _Name = "t$" + (_IndexSource++).ToString();
                }
                return _Name;
            }
        }
        private string _Name;
        /// <summary>
        /// 名称种类。
        /// </summary>
        public EDbNameKind NameKind => EDbNameKind.Name;
        private static int _IndexSource = 1;
    }
    /// <summary>
    /// 变量语句片段。
    /// </summary>
    public class VariableFragment : SqlFragment, IExpressionFragment, INameFragment
    {
        /// <summary>
        /// 创建变量。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="prefix">变量前缀。</param>
        public VariableFragment(GenerateContext context, char prefix = 'v')
            : base(context)
        {
            _Prefix = prefix;
        }
        /// <summary>
        /// 创建变量。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="name">变量名。</param>
        public VariableFragment(GenerateContext context, string name)
            : base(context)
        {
            _Name = name;
        }
        /// <summary>
        /// 对象名称。
        /// </summary>
        public string Name
        {
            get
            {
                if (_Name == null)
                {
                    _Name = _Prefix.ToString() + this.Context.GetVariable(this);
                }
                return _Name;
            }
        }
        private string _Name = null;
        /// <summary>
        /// 名称种类。
        /// </summary>
        public EDbNameKind NameKind => EDbNameKind.Contact;
        private readonly char _Prefix;
    }
    /// <summary>
    /// 语句字符串语句片段。
    /// </summary>
    public class SimpleFragment : SqlFragment, IExpressionFragment, INameFragment
    {
        /// <summary>
        /// 初始化语句字符串。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">字符串内容。</param>
        public SimpleFragment(GenerateContext context, string content)
            : base(context)
        {
            Content = content;
        }
        /// <summary>
        /// 字符串内容。
        /// </summary>
        public string Content { get; }
        /// <summary>
        /// 对象名称。
        /// </summary>
        public string Name => Content;
        /// <summary>
        /// 名称种类。
        /// </summary>
        public EDbNameKind NameKind => EDbNameKind.Contact;
    }
}