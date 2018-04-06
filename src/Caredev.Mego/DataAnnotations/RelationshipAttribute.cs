// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.DataAnnotations
{
    using System;
    using System.Linq;
    using Res = Properties.Resources;
    /// <summary>
    /// 复合导航关系的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RelationshipAttribute : Attribute
    {
        /// <summary>
        /// 创建复合导航关系特性，所有复合关系都会有第三个关系对象用于存储关系数据。
        /// </summary>
        /// <param name="type">关系实体的CLR类型。</param>
        /// <param name="lNames">源对象的外键属性名称集合，多个名称以逗号分割。</param>
        /// <param name="lPrincipals">源对象的主键属性名称集合，多个名称以逗号分割。</param>
        /// <param name="rNames">目标对象的外键属性名称集合，多个名称以逗号分割。</param>
        /// <param name="rPrincipals">目标对象的主键属性名称集合，多个名称以逗号分割</param>
        /// <param name="mode">当前关系模式。</param>
        public RelationshipAttribute(Type type,
            string lNames, string lPrincipals,
            string rNames, string rPrincipals, 
            EObjectRelationMode mode = EObjectRelationMode.None)
        {
            LeftNames = lNames.Split(',').Select(a => a.Trim()).ToArray();
            LeftPrincipals = lPrincipals.Split(',').Select(a => a.Trim()).ToArray();
            RightNames = rNames.Split(',').Select(a => a.Trim()).ToArray();
            RightPrincipals = rPrincipals.Split(',').Select(a => a.Trim()).ToArray();

            if (LeftNames.Length != LeftPrincipals.Length || RightNames.Length != RightPrincipals.Length || LeftNames.Length != RightNames.Length)
            {
                throw new ArgumentException(Res.ExceptionKeyCountIsNotMatch);
            }
            RelationType = type;
            ObjectRelationMode = mode;
        }
        /// <summary>
        /// 关系对象的CLR类型。
        /// </summary>
        public Type RelationType { get; }
        /// <summary>
        /// 源对象的外键属性名称集合。
        /// </summary>
        public string[] LeftNames { get; }
        /// <summary>
        /// 源对象的主键属性名称集合。
        /// </summary>
        public string[] LeftPrincipals { get; }
        /// <summary>
        /// 目标对象的主键属性名称集合。
        /// </summary>
        public string[] RightPrincipals { get; }
        /// <summary>
        /// 目标对象的外键属性名称集合。
        /// </summary>
        public string[] RightNames { get; }
        /// <summary>
        /// 对象有关系模式。
        /// </summary>
        public EObjectRelationMode ObjectRelationMode { get; }
    }
}