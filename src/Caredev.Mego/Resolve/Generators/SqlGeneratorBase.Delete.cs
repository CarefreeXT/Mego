// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Linq;
    using Res = Properties.Resources;
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成删除操作语句，对象删除的总入口。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDelete(GenerateContext context)
        {
            var data = (GenerateDataForDelete)context.Data;
            if (data.Items.Count == 1)
            {
                return GenerateForDeleteSingle(context);
            }
            else
            {
                if (data.Table.Keys.Length == 1 && !data.NeedConcurrencyCheck)
                {
                    return GenerateForDeleteKey(context);
                }
                else
                {
                    if (context.Feature.HasCapable(EDbCapable.TemporaryTable | EDbCapable.ModifyJoin))
                    {
                        return GenerateForDeleteKeysTempTable(context);
                    }
                    else
                    {
                        return GenerateForDeleteSingleRepeat(context);
                    }
                }
            }
        }
        /// <summary>
        /// 生成删除单个数据对象语句
        /// </summary>
        /// <param name="context">生成上下文</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteSingle(GenerateContext context)
        {
            var data = (GenerateDataForDelete)context.Data;
            var block = new BlockFragment(context);
            var name = data.TargetName;
            foreach (var table in data.GetTables())
            {
                var delete = new DeleteFragment(context, table, name);
                delete.Where = delete.Target.JoinCondition(data.CommitObject, table.Keys.Union(table.Concurrencys));
                block.Add(delete);
            }
            data.SetConcurrencyExpectCount(data.TableCount);
            return block;
        }
        /// <summary>
        /// 生成删除若干数据对象（单个主键）语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteKey(GenerateContext context)
        {
            var data = (GenerateDataForDelete)context.Data;
            var block = new BlockFragment(context);
            var key = data.Table.Keys[0];
            var member = (CommitMemberFragment)data.CommitObject.GetMember(key);
            var values = new ValueListFragment(context, member, data.Items);
            var name = data.TargetName;
            foreach (var table in data.GetTables())
            {
                var delete = new DeleteFragment(context, table, name);
                var keyMember = delete.Target.GetMember(key);
                delete.Where = new ScalarFragment(context, values, keyMember)
                {
                    Function = SupportMembers.Enumerable.Contains
                };
                block.Add(delete);
            }
            return block;
        }
        /// <summary>
        /// 使用临时表形式，生成删除若干数据对象（多个主键）语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteKeysTempTable(GenerateContext context)
        {
            var data = (GenerateDataForDelete)context.Data;
            var block = new BlockFragment(context)
            {
                GenerateCreateTemplateTable(context, data.CommitObject, data.Items,data.Table.Keys.Concat(data.ConcurrencyMembers))
            };
            var temptable = ((CreateTempTableFragment)block.First()).Table;
            var name = data.TargetName;
            foreach (var metadata in data.GetTables())
            {
                var delete = new DeleteFragment(context, metadata, name);
                var current = delete.Target;
                var currenttemptable = new TemporaryTableFragment(context, temptable.Name, metadata.Keys.Union(metadata.Concurrencys));
                delete.AddSource(currenttemptable);
                current.Join(currenttemptable, metadata.Keys.Union(metadata.Concurrencys));
                block.Add(delete);
            }
            data.SetConcurrencyExpectCount(data.TableCount + 1);
            return block;
        }
        /// <summary>
        /// 使用重复生成的方式，生成删除多个对象的语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteSingleRepeat(GenerateContext context)
        {
            var data = (GenerateDataForDelete)context.Data;
            var block = (BlockFragment)GenerateForDeleteSingle(context);
            return new RepeatBlockFragment(context, data.Items, data.CommitObject.Loader, block);
        }
        /// <summary>
        /// 表达式生成删除语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">生成表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteStatement(GenerateContext context, DbExpression content)
        {
            var data = (GenerateDataForStatement)context.Data;
            var item = data.ItemEpxression;
            var source = CreateSource(context, content) as QueryBaseFragment;

            var table = data.Table;
            if (table.InheritSets.Length == 0)
            {
                var target = (TableFragment)GetSource(context, item);
                target.Name = context.ConvertName(data.TargetName);
                var delete = new DeleteFragment(context, target)
                {
                    Where = source.Where,
                    Take = source.Take
                };
                delete.AddSource(source.Sources.Where(a => a != target));
                return delete;
            }
            else
            {
                var createtable = new CreateTempTableFragment(context, table.Keys);
                var insert = new InsertFragment(context, createtable.Table, source);
                insert.Members.AddRange(createtable.Table.Members);
                foreach (var key in table.Keys)
                {
                    RetrievalMember(context, source, new DbMemberExpression(key.Member, item), null, false);
                }
                var block = new BlockFragment(context, createtable, insert);
                foreach (var subtable in table.InheritSets.Concat(new TableMetadata[] { table }).Reverse())
                {
                    var current = new TableFragment(context, subtable);
                    var delete = new DeleteFragment(context, current);
                    var temptable = new TemporaryTableFragment(context, createtable.Table.Name, table.Keys);
                    current.Join(temptable, table.Keys);
                    delete.AddSource(temptable);
                    block.Add(delete);
                }
                return block;
            }
        }
    }
}