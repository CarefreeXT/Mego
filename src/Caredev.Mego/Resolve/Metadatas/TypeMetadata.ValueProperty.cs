// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using Caredev.Mego.Common;
    using Caredev.Mego.Resolve.ValueConversion;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    public partial class TypeMetadata
    {
        private Action<object, int[], object[]> GetPropertyMethod;
        private Action<object, int, object> SetPropertyMethod;
        /// <summary>
        /// 根据<see cref="PrimaryMembers"/>中声明的成员，获取指定索引对应的多个属性值。
        /// </summary>
        /// <param name="data">数据对象。</param>
        /// <param name="indexs">指定属性的索引数组。</param>
        /// <param name="values">输出属性值的目标数数组。</param>
        internal void GetProperty(object data, int[] indexs, object[] values)
        {
            if (GetPropertyMethod == null)
            {
                GetPropertyMethod = (Action<object, int[], object[]>)new GetPropertyGenerator(this).Compile();
            }
            GetPropertyMethod(data, indexs, values);
        }
        /// <summary>
        /// 根据<see cref="PrimaryMembers"/>中声明的成员，设置指定索引对应的多个属性值。
        /// </summary>
        /// <param name="data">数据对象。</param>
        /// <param name="index">指定属性的索引。</param>
        /// <param name="value">设置的属性值。</param>
        internal void SetProperty(object data, int index, object value)
        {
            if (SetPropertyMethod == null)
            {
                SetPropertyMethod = (Action<object, int, object>)new SetPropertyGenerator(this).Compile();
            }
            SetPropertyMethod(data, index, value);
        }
        /// <summary>
        /// 设置属性代码生成器。
        /// </summary>
        private sealed class SetPropertyGenerator : MethodILGenerateor<Action<object, int, object>>
        {
            public SetPropertyGenerator(TypeMetadata metadata)
                : base(metadata)
            {
            }

            private LocalBuilder item;
            private Label[] switchItems;
            private Label switchEndLabel;
            public override Delegate Compile()
            {
                var itemType = Metadata.ClrType;
                item = il.DeclareLocal(itemType);
                var members = Metadata.PrimaryMembers;

                switchEndLabel = il.DefineLabel();
                //var item = (Type)data;
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, itemType);
                il.Emit(OpCodes.Stloc, item);
                //switch(index)
                il.Emit(OpCodes.Ldarg_1);
                switchItems = new Label[members.Count];
                for (int i = 0; i < switchItems.Length; i++)
                {
                    switchItems[i] = il.DefineLabel();
                }

                il.Emit(OpCodes.Switch, switchItems);
                il.Emit(OpCodes.Ret);

                int count = members.Count - 1;
                for (int i = 0; i <= count; i++)
                {
                    Property(members[i].Member, i);
                }
                //il.MarkLabel(switchEndLabel);

                //il.Emit(OpCodes.Ret);
                return base.Compile();
            }

            private void Property(PropertyInfo property, int currentIndex)
            {
                il.MarkLabel(switchItems[currentIndex]);
                var type = property.PropertyType;

                il.Emit(OpCodes.Ldloc, item);
                il.Emit(OpCodes.Ldarg_2);

                if (type.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, type);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, property.PropertyType);
                }
                il.Emit(OpCodes.Callvirt, property.GetSetMethod(true));
                il.Emit(OpCodes.Ret);
            }
        }
        /// <summary>
        /// 获取属性代码生成器。
        /// </summary>
        private sealed class GetPropertyGenerator : MethodILGenerateor<Action<object, int[], object[]>>
        {
            public GetPropertyGenerator(TypeMetadata metadata)
                : base(metadata)
            {
            }

            public override Delegate Compile()
            {
                var itemType = Metadata.ClrType;

                var item = il.DeclareLocal(itemType);
                var index = il.DeclareLocal(typeof(int));
                var count = il.DeclareLocal(typeof(int));
                var forAdd = il.DefineLabel();
                var forHead = il.DefineLabel();
                var forBody = il.DefineLabel();
                var members = Metadata.PrimaryMembers;
                var judgeLabels = new Label[members.Count];
                for (int i = 0; i < judgeLabels.Length; i++)
                    judgeLabels[i] = il.DefineLabel();

                //var count = indexs.Length;
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldlen);
                il.Emit(OpCodes.Conv_I4);
                il.Emit(OpCodes.Stloc, count);
                //var item = (Type)data;
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, itemType);
                il.Emit(OpCodes.Stloc, item);
                //var index = 0;
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Stloc, index);

                il.Emit(OpCodes.Br, forHead);

                il.MarkLabel(forBody);

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Ldelem_I4);
                il.Emit(OpCodes.Switch, judgeLabels);
                il.Emit(OpCodes.Br, forAdd);

                for (int i = 0; i < members.Count; i++)
                {
                    var property = members[i].Member;
                    il.MarkLabel(judgeLabels[i]);
                    //values[index] = item.Id;
                    il.Emit(OpCodes.Nop);
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Ldloc, index);
                    il.Emit(OpCodes.Ldloc, item);
                    il.Emit(OpCodes.Callvirt, property.GetGetMethod(true));
                    var targetType = property.PropertyType;
                    //处理值转换，Object -> Storage
                    if (Metadata.Engine.TryGetConversion(targetType, out ConversionInfo info))
                    {
                        il.Emit(OpCodes.Call, info.ConvertToStorage);
                    }
                    else
                    {
                        if (targetType.IsValueType)
                        {
                            il.Emit(OpCodes.Box, targetType);
                        }
                    }
                    il.Emit(OpCodes.Stelem_Ref);

                    il.Emit(OpCodes.Br, forAdd);
                }

                il.MarkLabel(forAdd);
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc, index);

                il.MarkLabel(forHead);
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Ldloc, count);
                il.Emit(OpCodes.Clt);
                il.Emit(OpCodes.Brtrue, forBody);

                il.Emit(OpCodes.Ret);
                return base.Compile();
            }
        }
    }
}