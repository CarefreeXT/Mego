// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Metadatas
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    public partial class TypeMetadata
    {
        //向当前类型的集合添加项。
        private Action<object, object> CollectionAddMethod;
        private Action<object, object> CollectionRemoveMethod;
        /// <summary>
        /// 集合添加方法代码生成器。
        /// </summary>
        private sealed class CollectionAddMethodGenerator : MethodILGenerateor<Action<object, object>>
        {
            /// <inheritdoc/>
            public CollectionAddMethodGenerator(TypeMetadata metadata)
                : base(metadata)
            {
            }
            /// <inheritdoc/>
            public override Delegate Compile()
            {
                var collectionType = typeof(ICollection<>).MakeGenericType(Metadata.ClrType);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, collectionType);

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Castclass, Metadata.ClrType);
                il.Emit(OpCodes.Callvirt, collectionType.GetMethod("Add"));

                il.Emit(OpCodes.Ret);

                return base.Compile();
            }
        }
        /// <summary>
        /// 集合删除方法代码生成器。
        /// </summary>
        private sealed class CollectionRemoveMethodGenerator : MethodILGenerateor<Action<object, object>>
        {
            /// <inheritdoc/>
            public CollectionRemoveMethodGenerator(TypeMetadata metadata)
                : base(metadata)
            {
            }
            /// <inheritdoc/>
            public override Delegate Compile()
            {
                var collectionType = typeof(ICollection<>).MakeGenericType(Metadata.ClrType);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, collectionType);

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Castclass, Metadata.ClrType);
                il.Emit(OpCodes.Callvirt, collectionType.GetMethod("Remove"));

                il.Emit(OpCodes.Ret);

                return base.Compile();
            }
        }
        /// <summary>
        /// 向指定集合添加对象。
        /// </summary>
        /// <param name="source">集合对象。</param>
        /// <param name="target">目标对象。</param>
        public void CollectionAdd(object source, object target)
        {
            if (CollectionAddMethod == null)
            {
                CollectionAddMethod = (Action<object, object>)new CollectionAddMethodGenerator(this).Compile();
            }
            CollectionAddMethod(source, target);
        }
        /// <summary>
        /// 从指定集合删除对象。
        /// </summary>
        /// <param name="source">集合对象。</param>
        /// <param name="target">目标对象。</param>
        public void CollectionRemove(object source, object target)
        {
            if (CollectionRemoveMethod == null)
            {
                CollectionRemoveMethod = (Action<object, object>)new CollectionRemoveMethodGenerator(this).Compile();
            }
            CollectionRemoveMethod(source, target);
        }
    }
}