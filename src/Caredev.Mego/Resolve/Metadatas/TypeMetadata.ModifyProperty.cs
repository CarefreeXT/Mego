// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.Common;
    using System;
    using System.Data.Common;
    using System.Reflection;
    using System.Reflection.Emit;
    using Res = Properties.Resources;
    public partial class TypeMetadata
    {
        // DataReader 修改基础属性值（修改所有属性）
        private Action<DbDataReader, int[], object> ModifyPrimaryPropertyMethod;
        // Array 修改基础属性值（修改所有属性）
        private Action<object[], int[], object> ModifyPrimaryProperty2Method;
        //设置复杂属性值（一次只能修改一个属性）
        private Action<object, int, object> SetComplexPropertyMethod;
        //获取复杂属性值
        private Func<object, int, object> GetComplexPropertyMethod;
        /// <summary>
        /// 根据属性索引列表加载<see cref="DbDataReader"/>的数据到目标对象。
        /// </summary>
        /// <param name="reader">数据读取对象。</param>
        /// <param name="indexs">赋值索引列表，按属性的排列顺序依次获取指定<see cref="DbDataReader"/>
        /// 索引处的值，若索引为 -1 则表示忽略该属性。</param>
        /// <param name="target">目标对象。</param>
        internal void ModifyProperty(DbDataReader reader, int[] indexs, object target)
        {
            if (ClrType.IsAnonymous())
            {
                throw new InvalidOperationException(Res.ExceptionAnonymousObjectPropertyIsReadOnly);
            }
            if (ModifyPrimaryPropertyMethod == null)
            {
                ModifyPrimaryPropertyMethod = (Action<DbDataReader, int[], object>)new ModifyPrimaryPropertyGenerator(this).Compile();
            }
            ModifyPrimaryPropertyMethod(reader, indexs, target);
        }
        /// <summary>
        /// 根据属性索引列表加载指定的值到目标对象。
        /// </summary>
        /// <param name="data">数据内容。</param>
        /// <param name="indexs">赋值索引列表，按属性的排列顺序依次获取指定<see cref="DbDataReader"/>
        /// 索引处的值，若索引为 -1 则表示忽略该属性。</param>
        /// <param name="target">目标对象。</param>
        internal void ModifyProperty(object[] data, int[] indexs, object target)
        {
            if (ClrType.IsAnonymous())
            {
                throw new InvalidOperationException(Res.ExceptionAnonymousObjectPropertyIsReadOnly);
            }
            if (ModifyPrimaryProperty2Method == null)
            {
                ModifyPrimaryProperty2Method = (Action<object[], int[], object>)new ModifyPrimaryProperty2Generator(this).Compile();
            }
            ModifyPrimaryProperty2Method(data, indexs, target);
        }
        /// <summary>
        /// 设置指定索引位置的复杂属性值。
        /// </summary>
        /// <param name="source">设置对象。</param>
        /// <param name="index">属性相应的索引。</param>
        /// <param name="value">设置的值。</param>
        internal void SetComplexProperty(object source, int index, object value)
        {
            if (ClrType.IsAnonymous())
            {
                throw new InvalidOperationException(Res.ExceptionAnonymousObjectPropertyIsReadOnly);
            }
            if (SetComplexPropertyMethod == null)
            {
                SetComplexPropertyMethod = (Action<object, int, object>)new SetComplexPropertyGenerator(this).Compile();
            }
            SetComplexPropertyMethod(source, index, value);
        }
        /// <summary>
        /// 获取复杂属性值
        /// </summary>
        /// <param name="source">获取的对象。</param>
        /// <param name="index">属性相应的索引。</param>
        /// <returns>检索值。</returns>
        internal object GetComplexProperty(object source, int index)
        {
            if (GetComplexPropertyMethod == null)
            {
                GetComplexPropertyMethod = (Func<object, int, object>)new GetComplexPropertyGenerator(this).Compile();
            }
            return GetComplexPropertyMethod(source, index);
        }
        /// <summary>
        /// 使用数组修改普通属性代码生成器。
        /// </summary>
        private sealed class ModifyPrimaryProperty2Generator : MethodILGenerateor<Action<object[], int[], object>>
        {
            public ModifyPrimaryProperty2Generator(TypeMetadata metadata)
             : base(metadata)
            {
            }

            private LocalBuilder item;
            private LocalBuilder field;

            private LocalBuilder value;
            /// <inheritdoc/>
            public override Delegate Compile()
            {
                value = il.DeclareLocal(typeof(object));

                var itemType = Metadata.ClrType;
                item = il.DeclareLocal(itemType);
                field = il.DeclareLocal(typeof(int));

                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Castclass, Metadata.ClrType);
                il.Emit(OpCodes.Stloc, item);

                var members = Metadata.PrimaryMembers;
                int count = members.Count - 1;
                for (int i = 0; i <= count; i++)
                {
                    Property(members[i].Member, i);
                }

                il.Emit(OpCodes.Ret);
                return base.Compile();
            }

            private void Property(PropertyInfo property, int currentIndex)
            {
                Label judgeLabel = il.DefineLabel();
                //var field = fields[index];
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldc_I4, currentIndex);
                il.Emit(OpCodes.Ldelem_I4);
                il.Emit(OpCodes.Stloc, field);
                //if(field >= 0)
                il.Emit(OpCodes.Ldloc, field);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Blt_S, judgeLabel);
                //item.Property = values[field]
                il.PropertySetValue(property, item, () => il.ArrayGetValue(Metadata, property, field));

                il.MarkLabel(judgeLabel);
            }
        }
        /// <summary>
        /// 使用<see cref="DbDataReader"/>修改普通属性代码生成器。
        /// </summary>
        private sealed class ModifyPrimaryPropertyGenerator : MethodILGenerateor<Action<DbDataReader, int[], object>>
        {
            public ModifyPrimaryPropertyGenerator(TypeMetadata metadata)
                : base(metadata)
            {
            }

            private LocalBuilder item;
            private LocalBuilder field;
            /// <inheritdoc/>
            public override Delegate Compile()
            {
                var itemType = Metadata.ClrType;
                item = il.DeclareLocal(itemType);
                field = il.DeclareLocal(typeof(int));

                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Castclass, Metadata.ClrType);
                il.Emit(OpCodes.Stloc, item);

                var members = Metadata.PrimaryMembers;
                int count = members.Count - 1;
                for (int i = 0; i <= count; i++)
                    Property(members[i].Member, i);

                il.Emit(OpCodes.Ret);
                return base.Compile();
            }

            private void Property(PropertyInfo property, int currentIndex)
            {
                Label judgeLabel = il.DefineLabel();
                //var field = fields[index];
                il.GetFieldByIndex(field, currentIndex);
                //if(field >= 0
                il.IfFieldGreaterEqualZero(field, ref judgeLabel);
                var propertyType = property.PropertyType;
                //if(!reader.IsDBNull(field))
                il.IfReaderIsDbNull(propertyType, field, ref judgeLabel);
                //item.Property = reader.GetValue(field)
                il.PropertySetValue(property, item, () => il.ReaderGetValue(Metadata, property, field));

                il.MarkLabel(judgeLabel);
            }
        }
        /// <summary>
        /// 设置复杂属性代码生成器。
        /// </summary>
        private sealed class SetComplexPropertyGenerator : MethodILGenerateor<Action<object, int, object>>
        {
            public SetComplexPropertyGenerator(TypeMetadata metadata)
                : base(metadata)
            {
            }
            /// <inheritdoc/>
            public override Delegate Compile()
            {
                var members = Metadata.ComplexMembers;

                Label[] jumpTable = new Label[members.Count];
                var endLabel = il.DefineLabel();
                for (int i = 0; i < jumpTable.Length; i++)
                    jumpTable[i] = il.DefineLabel();

                il.Emit(OpCodes.Ldarg_1);

                il.Emit(OpCodes.Switch, jumpTable);
                il.Emit(OpCodes.Br, endLabel);

                for (int i = 0; i < jumpTable.Length; i++)
                {
                    var member = members[i];
                    il.MarkLabel(jumpTable[i]);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Castclass, Metadata.ClrType);
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Castclass, member.Member.PropertyType);
                    il.Emit(OpCodes.Callvirt, member.Member.GetSetMethod());
                    if (i == 0)
                        il.Emit(OpCodes.Br, endLabel);
                    else
                        il.Emit(OpCodes.Br_S, endLabel);
                }

                il.MarkLabel(endLabel);
                il.Emit(OpCodes.Ret);

                return base.Compile();
            }
        }
        /// <summary>
        /// 获取复杂属性代码生成器。
        /// </summary>
        private sealed class GetComplexPropertyGenerator : MethodILGenerateor<Func<object, int, object>>
        {
            public GetComplexPropertyGenerator(TypeMetadata metadata)
                : base(metadata)
            {
            }
            /// <inheritdoc/>
            public override Delegate Compile()
            {
                var members = Metadata.ComplexMembers;

                Label[] jumpTable = new Label[members.Count];
                var endLabel = il.DefineLabel();
                for (int i = 0; i < jumpTable.Length; i++)
                    jumpTable[i] = il.DefineLabel();

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Switch, jumpTable);
                il.Emit(OpCodes.Br, endLabel);

                for (int i = 0; i < jumpTable.Length; i++)
                {
                    var member = members[i];
                    il.MarkLabel(jumpTable[i]);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Castclass, Metadata.ClrType);
                    il.Emit(OpCodes.Callvirt, member.Member.GetGetMethod(true));
                    il.Emit(OpCodes.Castclass, typeof(object));
                    il.Emit(OpCodes.Ret);
                }

                il.MarkLabel(endLabel);

                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);

                return base.Compile();
            }
        }
    }
}