// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.Expressions;
    using Caredev.Mego.Resolve.Generators.Contents;
    using Caredev.Mego.Resolve.Generators.Fragments;
    using Caredev.Mego.Resolve.Metadatas;
    using System;
    using System.Linq;
    public partial class SqlGeneratorBase
    {
        /// <summary>
        /// 表达式生成删除语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">生成表达式。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteStatement(GenerateContext context, DbExpression content)
        {
            var data = (StatementContent)context.Data;
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
                    var temptable = new TemporaryTableFragment(context, table.Keys, createtable.Table.Name);
                    current.Join(temptable, table.Keys);
                    delete.AddSource(temptable);
                    block.Add(delete);
                }
                return block;
            }
        }
    }
    partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成实体删除语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">表达式内容（为空）。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteContent(GenerateContext context, DbExpression content)
        {
            var data = (DeleteContent)context.Data;
            data.SetConcurrencyExpectCount(1);
            if (data.Items.Count == 1)
            {
                return GenerateForDeleteSingle(context, data);
            }
            else if (data.Table.Keys.Length == 1 && !data.NeedConcurrencyCheck)
            {
                return GenerateForDeleteKey(context, data);
            }
            else if (context.Feature.HasCapable(EDbCapable.TemporaryTable | EDbCapable.ModifyJoin))
            {
                data.SetConcurrencyExpectCount(2);
                return GenerateForDeleteKeysTempTable(context, data);
            }
            else
            {
                return GenerateForDeleteSingleRepeat(context, data);
            }
        }
        /// <summary>
        /// 生成删除单个数据对象语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteSingle(GenerateContext context, DeleteContent data)
        {
            return data.DeleteByKeys(data.Table, data.TargetName);
        }
        /// <summary>
        /// 生成删除若干数据对象（单个主键）语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteKey(GenerateContext context, DeleteContent data)
        {
            var key = data.Table.Keys[0];
            var member = (CommitMemberFragment)data.CommitObject.GetMember(key);
            var values = new ValueListFragment(context, member, data.Items);
            var table = data.Table;
            return data.DeleteInKey(data.Table, key, values, data.TargetName);
        }
        /// <summary>
        /// 使用临时表形式，生成删除若干数据对象（多个主键）语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteKeysTempTable(GenerateContext context, DeleteContent data)
        {
            var block = new BlockFragment(context)
            {
                GenerateCreateTemplateTable(context, data.CommitObject, data.Items
                    , data.UnionConcurrencyMembers(data.Table, data.Table.Keys))
            };
            var temptable = ((CreateTempTableFragment)block.First()).Table;
            block.Add(data.DeleteByTemptable(data.Table, temptable, data.TargetName));
            return block;
        }
        /// <summary>
        /// 使用重复生成的方式，生成删除多个对象的语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteSingleRepeat(GenerateContext context, DeleteContent data)
        {
            var delete = GenerateForDeleteSingle(context, data);
            return new RepeatBlockFragment(context, data.Items, data.CommitObject.Loader, new BlockFragment(context, delete));
        }
    }
    partial class SqlGeneratorBase
    {
        /// <summary>
        /// 生成继承实体删除语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="content">表达式内容（为空）。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteInherit(GenerateContext context, DbExpression content)
        {
            var data = (InheritDeleteContent)context.Data;
            data.SetConcurrencyExpectCount(data.Tables.Length);
            if (data.Items.Count == 1)
            {
                return GenerateForDeleteSingle(context, data);
            }
            else if (data.Table.Keys.Length == 1 && !data.NeedConcurrencyCheck)
            {
                return GenerateForDeleteKey(context, data);
            }
            else if (context.Feature.HasCapable(EDbCapable.TemporaryTable | EDbCapable.ModifyJoin))
            {
                data.SetConcurrencyExpectCount(data.Tables.Length + 1);
                return GenerateForDeleteKeysTempTable(context, data);
            }
            else
            {
                return GenerateForDeleteSingleRepeat(context, data);
            }
        }
        /// <summary>
        /// 生成删除单个数据对象语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteSingle(GenerateContext context, InheritDeleteContent data)
        {
            return new BlockFragment(context, data.Tables.Select(a => data.DeleteByKeys(a)).ToArray());
        }
        /// <summary>
        /// 生成删除若干数据对象（单个主键）语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteKey(GenerateContext context, InheritDeleteContent data)
        {
            var key = data.Table.Keys[0];
            var member = (CommitMemberFragment)data.CommitObject.GetMember(key);
            var values = new ValueListFragment(context, member, data.Items);
            return new BlockFragment(context, data.Tables.Select(
                a => data.DeleteInKey(a, key, values)).ToArray());
        }
        /// <summary>
        /// 使用临时表形式，生成删除若干数据对象（多个主键）语句，
        /// 要求数据库拥有<see cref="EDbCapable.TemporaryTable"/>
        /// 和<see cref="EDbCapable.ModifyJoin"/>两个特性。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteKeysTempTable(GenerateContext context, InheritDeleteContent data)
        {
            var block = new BlockFragment(context)
            {
                GenerateCreateTemplateTable(context, data.CommitObject, data.Items
                    , data.UnionConcurrencyMembers(data.Table, data.Table.Keys))
            };
            var temptable = ((CreateTempTableFragment)block.First()).Table;
            foreach (var metadata in data.Tables)
            {
                block.Add(data.DeleteByTemptable(metadata, temptable));
            }
            return block;
        }
        /// <summary>
        /// 使用重复生成的方式，生成删除多个对象的语句。
        /// </summary>
        /// <param name="context">生成上下文。</param>
        /// <param name="data">数据对象。</param>
        /// <returns>语句片段。</returns>
        protected virtual SqlFragment GenerateForDeleteSingleRepeat(GenerateContext context, InheritDeleteContent data)
        {
            var block = (BlockFragment)GenerateForDeleteSingle(context, data);
            return new RepeatBlockFragment(context, data.Items, data.CommitObject.Loader, block);
        }
    }
}