// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Linq;
    using Res = Properties.Resources;
    partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成数据库维护语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>生成的语句片段。</returns>
        protected virtual SqlFragment GenerateForMaintenance(GenerateContext context)
        {
            var operate = context.Data.Operate;
            switch (operate.Type)
            {
                case EOperateType.CreateTable: return GenerateForMaintenanceCreateTable(context);
                case EOperateType.ObjectExsit: return GenerateForMaintenanceObjectExsit(context);
                case EOperateType.CreateRelation: return GenerateForMaintenanceCreateRelation(context);
            }
            throw new NotImplementedException(string.Format(Res.NotSupportedCreateMaintenance, operate.Type));
        }

        private SqlFragment GenerateForMaintenanceCreateTable(GenerateContext context)
        {
            return new CreateTableFragment(context, context.Metadata.Table(context.Data.Operate.ClrType));
        }

        private SqlFragment GenerateForMaintenanceObjectExsit(GenerateContext context)
        {
            var operate = (DbObjectExsitOperate)context.Data.Operate;
            ObjectNameFragment name = null;
            if (string.IsNullOrEmpty(operate.Name))
            {
                var table = context.Metadata.Table(operate.ClrType);
                name = new ObjectNameFragment(context, table.Name, table.Schema);
            }
            else
            {
                name = new ObjectNameFragment(context, operate.Name, operate.Schema);
            }
            return new ObjectExsitFragment(context, name, operate.Kind);
        }

        private SqlFragment GenerateForMaintenanceCreateRelation(GenerateContext context)
        {
            var operate = (DbCreateDropRelationOperate)context.Data.Operate;
            var fragment = new CreateRelationFragment(context, operate.ForeignName)
            {
                Foreign = new ObjectNameFragment(context, operate.Foreign.Name, operate.Foreign.Schema),
                Principal = new ObjectNameFragment(context, operate.Principal.Name, operate.Principal.Schema),
                ForeignKeys = operate.Pairs.Select(a => a.ForeignKey.Name).ToArray(),
                PrincipalKeys = operate.Pairs.Select(a => a.PrincipalKey.Name).ToArray()
            };

            return fragment;
        }
    }
}
