// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Operates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Res = Properties.Resources;

    partial class SqlGeneratorBase
    {
        /// <summary>
        /// 初始化数据库维护方法集合。
        /// </summary>
        /// <returns></returns>
        protected IDictionary<EOperateType, MaintenanceOperateDelegate> InitialMethodsForMaintenance()
        {
            return new Dictionary<EOperateType, MaintenanceOperateDelegate>()
            {
                { EOperateType.CreateRelation, GenerateForMaintenanceCreateRelation },
                { EOperateType.DropRelation, GenerateForMaintenanceDropRelation },

                { EOperateType.CreateTable, GenerateForMaintenanceCreateTable },
                { EOperateType.CreateTempTable, GenerateForMaintenanceCreateTable },
                { EOperateType.CreateTableVariable, GenerateForMaintenanceCreateTable },
                { EOperateType.CreateView, GenerateForMaintenanceCreateView },

                { EOperateType.TableIsExsit, GenerateForMaintenanceObjectIsExsit },
                { EOperateType.ViewIsExsit, GenerateForMaintenanceObjectIsExsit },
                { EOperateType.DropTable, GenerateForMaintenanceDropObject },
                { EOperateType.DropView, GenerateForMaintenanceDropObject },
                { EOperateType.RenameTable, GenerateForMaintenanceRenameObject },
                { EOperateType.RenameView, GenerateForMaintenanceRenameObject },
            };
        }
        /// <summary>
        /// 生成数据库维护语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>生成的语句片段。</returns>
        protected virtual SqlFragment GenerateForMaintenance(GenerateContext context)
        {
            var operate = context.Data.Operate;
            if (_MaintenanceMethods.TryGetValue(operate.Type, out MaintenanceOperateDelegate method))
            {
                return method(context);
            }
            throw new NotImplementedException(string.Format(Res.NotSupportedCreateMaintenance, operate.Type));
        }
        /// <summary>
        /// 创建数据表语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForMaintenanceCreateTable(GenerateContext context)
        {
            var operate = (DbCreateTableOperate)context.Data.Operate;
            var metadata = context.Metadata.Table(context.Data.Operate.ClrType);
            if (operate.Type == EOperateType.CreateTable)
            {
                var name = context.ConvertName(operate.Name);
                return new CreateTableFragment(context, metadata, name);
            }
            else
            {
                var members = metadata.InheritSets.SelectMany(a => a.Members).Concat(metadata.Members);
                INameFragment name = null;
                if (operate.Type == EOperateType.CreateTempTable)
                {
                    name = new TempTableNameFragment(context, operate.Name.Name);
                    return new CreateTempTableFragment(context, members, name) { IsVariable = false };
                }
                else
                {
                    name = new VariableFragment(context, operate.Name.Name);
                    return new CreateTempTableFragment(context, members, name) { IsVariable = true };
                }
            }
        }
        /// <summary>
        /// 创建视图语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForMaintenanceCreateView(GenerateContext context)
        {
            var operate = (DbCreateViewOperate)context.Data.Operate;
            var name = context.ConvertName(operate.Name);
            return new CreateViewFragment(context, name, operate.Statement);
        }
        /// <summary>
        /// 重命名对象语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForMaintenanceRenameObject(GenerateContext context)
        {
            var operate = (DbRenameObjectOperate)context.Data.Operate;
            var name = context.ConvertName(operate.Name);
            var kind = EDatabaseObject.Table;
            switch (operate.Type)
            {
                case EOperateType.RenameTable: kind = EDatabaseObject.Table; break;
                case EOperateType.RenameView: kind = EDatabaseObject.View; break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedGenerateMaintenance, operate.Type));
            }
            return new RenameObjectFragment(context, name, kind, operate.NewName);
        }
        /// <summary>
        /// 删除对象语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForMaintenanceDropObject(GenerateContext context)
        {
            var operate = (DbDropObjectOperate)context.Data.Operate;
            var name = context.ConvertName(operate.Name);
            var kind = EDatabaseObject.Table;
            switch (operate.Type)
            {
                case EOperateType.DropTable: kind = EDatabaseObject.Table; break;
                case EOperateType.DropView: kind = EDatabaseObject.View; break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedGenerateMaintenance, operate.Type));
            }
            return new DropObjectFragment(context, name, kind);
        }
        /// <summary>
        /// 生成判断对象存在语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForMaintenanceObjectIsExsit(GenerateContext context)
        {
            var operate = (DbObjectIsExsitOperate)context.Data.Operate;
            var name = context.ConvertName(operate.Name);
            var kind = EDatabaseObject.Table;
            switch (operate.Type)
            {
                case EOperateType.TableIsExsit: kind = EDatabaseObject.Table; break;
                case EOperateType.ViewIsExsit: kind = EDatabaseObject.View; break;
                case EOperateType.IndexIsExsit: kind = EDatabaseObject.Index; break;
                default: throw new NotSupportedException(string.Format(Res.NotSupportedGenerateMaintenance, operate.Type));
            }
            return new ObjectExsitFragment(context, name, kind);
        }
        /// <summary>
        /// 生成创建关系语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForMaintenanceCreateRelation(GenerateContext context)
        {
            var operate = (DbCreateDropRelationOperate)context.Data.Operate;
            var name = context.ConvertName(operate.Name);
            var fragment = new CreateRelationFragment(context, name)
            {
                Foreign = new ObjectNameFragment(context, operate.Foreign.Name, operate.Foreign.Schema),
                Principal = new ObjectNameFragment(context, operate.Principal.Name, operate.Principal.Schema),
                ForeignKeys = operate.Pairs.Select(a => a.ForeignKey.Name).ToArray(),
                PrincipalKeys = operate.Pairs.Select(a => a.PrincipalKey.Name).ToArray()
            };
            if (operate.Action != null)
            {
                var action = operate.Action;
                fragment.Update = action.Update;
                fragment.Delete = action.Delete;
            }
            return fragment;
        }
        /// <summary>
        /// 生成删除关系语句片段。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForMaintenanceDropRelation(GenerateContext context)
        {
            var operate = (DbCreateDropRelationOperate)context.Data.Operate;
            var name = context.ConvertName(operate.Name);
            var fragment = new DropRelationFragment(context, name)
            {
                Foreign = new ObjectNameFragment(context, operate.Foreign.Name, operate.Foreign.Schema),
            };
            return fragment;
        }
    }
}