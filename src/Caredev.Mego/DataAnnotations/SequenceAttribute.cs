// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using Exp = System.Linq.Expressions.Expression;
    using System.Linq.Expressions;
    using Caredev.Mego.Resolve.ValueGenerates;
    using Caredev.Mego.Common;

    /// <summary>
    /// 使用序列生成值。
    /// </summary>
    public sealed class SequenceAttribute : GeneratedExpressionAttribute
    {
        /// <summary>
        /// 创建序列生成特性。
        /// </summary>
        /// <param name="name">序列名。</param>
        /// <param name="purpose">生成目的。</param>
        public SequenceAttribute(string name, EGeneratedPurpose purpose = EGeneratedPurpose.Insert)
            : base(purpose)
        {
            Name = name;
        }
        /// <inheritdoc/>
        public override Expression Expression
        {
            get
            {
                if (_Expression == null)
                {
                    _Expression = Exp.Call(null, SupportMembers.DbFunctions.SequenceNext
                        , Exp.Constant(Name, typeof(string))
                        , Exp.Constant(Schema, typeof(string)));
                }
                return _Expression;
            }
            protected set { }
        }
        private Expression _Expression;
        /// <summary>
        /// 序列名。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 序列架构名。
        /// </summary>
        public string Schema { get; set; } = "";
    }
}
