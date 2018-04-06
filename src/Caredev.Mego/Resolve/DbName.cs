// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    /// <summary>
    /// 该类用于表示一个数据库名称。
    /// </summary>
    public class DbName
    {
        /// <summary>
        /// 创建常量名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>名称对象。</returns>
        public static DbName Contact(string name)
        {
            return new DbName() { Kind = EDbNameKind.Contact, Name = name };
        }
        /// <summary>
        /// 创建不包含架构的名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>名称对象。</returns>
        public static DbName NameOnly(string name)
        {
            return new DbName() { Kind = EDbNameKind.Name, Name = name };
        }
        /// <summary>
        /// 创建包含架构的名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构名。</param>
        /// <returns>名称对象。</returns>
        public static DbName Create(string name, string schema = "")
        {
            return new DbName() { Kind = EDbNameKind.NameSchema, Name = name, Schema = schema };
        }
        /// <summary>
        /// 名称种类。
        /// </summary>
        public EDbNameKind Kind { get; private set; }
        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 架构名。
        /// </summary>
        public string Schema { get; private set; }
        private DbName() { }
    }
}
