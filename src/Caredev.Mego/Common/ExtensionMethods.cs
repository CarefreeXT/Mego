// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Res = Properties.Resources;
    /// <summary>
    /// 内部使用扩展方法。
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// 遍历<see cref="IEnumerable"/>循环调用指定方法。
        /// </summary>
        /// <typeparam name="T">集合元素类型。</typeparam>
        /// <param name="items">遍历集合对象。</param>
        /// <param name="action">循环调用函数。</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items) action(item);
        }
        /// <summary>
        /// 遍历<see cref="IEnumerable{T}"/>第一次调用指定方法，后续循环调用另一个方法。
        /// </summary>
        /// <typeparam name="T">集合元素类型。</typeparam>
        /// <param name="items">集合对象。</param>
        /// <param name="first">首次循环调用函数。</param>
        /// <param name="other">后续循环调用函数。</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> first, Action<T> other)
        {
            bool isfirst = true;
            foreach (var item in items)
            {
                if (isfirst)
                {
                    isfirst = false;
                    first(item);
                }
                else
                {
                    other(item);
                }
            }
        }
        /// <summary>
        /// 遍历<see cref="IEnumerable{T}"/>循环调用指定方法，同时循环间隔调用指定方法。
        /// </summary>
        /// <typeparam name="T">集合元素类型。</typeparam>
        /// <param name="items">遍历集合对象。</param>
        /// <param name="split">间隔调用函数。</param>
        /// <param name="action">循环调用函数。</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action split, Action<T> action)
        {
            bool isfirst = true;
            foreach (var item in items)
            {
                if (isfirst)
                    isfirst = false;
                else
                    split();
                action(item);
            }
        }
        /// <summary>
        /// 遍历<see cref="IEnumerable"/>循环调用指定方法。
        /// </summary>
        /// <param name="items">遍历集合对象。</param>
        /// <param name="action">循环调用函数。</param>
        public static void ForEach(this IEnumerable items, Action<object> action)
        {
            foreach (var item in items)
                action(item);
        }
        /// <summary>
        /// 遍历<see cref="IEnumerable"/>第一次调用指定方法，后续循环调用另一个方法。
        /// </summary>
        /// <param name="items">集合对象。</param>
        /// <param name="first">首次循环调用函数。</param>
        /// <param name="action">后续循环调用函数。</param>
        public static void ForEach(this IEnumerable items, Action<object> first, Action<object> action)
        {
            bool isfirst = true;
            foreach (var item in items)
            {
                if (isfirst)
                {
                    isfirst = false;
                    first(item);
                }
                else
                {
                    action(item);
                }
            }
        }
        /// <summary>
        /// 遍历<see cref="IEnumerable"/>循环调用指定方法，同时循环间隔调用指定方法。
        /// </summary>
        /// <param name="items">遍历集合对象。</param>
        /// <param name="split">间隔调用函数。</param>
        /// <param name="action">循环调用函数。</param>
        public static void ForEach(this IEnumerable items, Action split, Action<object> action)
        {
            bool isfirst = true;
            foreach (var item in items)
            {
                if (isfirst)
                    isfirst = false;
                else
                    split();
                action(item);
            }
        }
        /// <summary>
        /// 在字符串集合中判断指定的字符串是否存在，如果存在则计算出一个新不重复的字符串。
        /// </summary>
        /// <param name="stringCollection">指定的字符串集合。</param>
        /// <param name="str">判断的字符串。</param>
        /// <returns>唯一不重复的字符串。</returns>
        public static string Unique(this IEnumerable<string> stringCollection, string str)
        {
            if (stringCollection.Contains(str))
            {
                var c = str[str.Length - 1];
                string prefix = str;
                int index = 1;
                if ('0' <= c && c <= '9')
                {
                    index = c - 47;
                    prefix = str.Substring(0, str.Length - 1);
                }
                do
                {
                    str = prefix + (index++).ToString();
                } while (stringCollection.Contains(str));
            }
            return str;
        }
        /// <summary>
        /// 检索<see cref="LambdaExpression"/>表达中指示的成员<see cref="PropertyInfo"/>对象。
        /// </summary>
        /// <param name="lambda">检索表达式。</param>
        /// <returns>表示式中指示的<see cref="PropertyInfo"/>对象。</returns>
        public static PropertyInfo GetMember(this LambdaExpression lambda)
        {
            if (lambda.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new ArgumentException(Res.ExceptionLambdaFoundNotMemberAccess, nameof(lambda));
            }
            var propertyInfo = ((MemberExpression)lambda.Body).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException(Res.ExceptionLambdaFoundNotPropertyInfo, nameof(lambda));
            }
            return propertyInfo;
        }
        /// <summary>
        /// 用指定的键向字典添加指定的值，如果键已经存在则用该值更新。
        /// </summary>
        /// <typeparam name="TKey">键的类型。</typeparam>
        /// <typeparam name="TValue">值的类型。</typeparam>
        /// <param name="dicrionary">目标字典对象。</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dicrionary, TKey key, TValue value)
        {
            lock (dicrionary)
            {
                if (dicrionary.ContainsKey(key))
                {
                    dicrionary[key] = value;
                }
                else
                {
                    dicrionary.Add(key, value);
                }
            }
        }
        /// <summary>
        /// 用指定的键在字典中检索相应的值，如果值不存在则创建并添加到字典中。
        /// </summary>
        /// <typeparam name="TKey">键的类型。</typeparam>
        /// <typeparam name="TValue">值的类型。</typeparam>
        /// <param name="dicrionary">目标字典对象。</param>
        /// <param name="key">用于检索的键。</param>
        /// <param name="createValue">创建值的函数对象。</param>
        /// <returns>返回检索或创建的值。</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dicrionary, TKey key, Func<TKey, TValue> createValue)
        {
            lock (dicrionary)
            {
                if (!dicrionary.TryGetValue(key, out TValue value))
                {
                    value = createValue(key);
                    dicrionary.Add(key, value);
                }
                return value;
            }
        }
    }
}
#if (NET35 || NET40)
namespace Caredev.Mego
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    /// <summary>
    /// 自定义特性扩展方法。
    /// </summary>
    public static class CustomAttributeExtensions
    {
#region APIs that return a single attribute
        /// <summary>
        /// 检索指定类型的应用于指定的程序集的自定义属性。
        /// </summary>
        /// <param name="element">要检查的程序集。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <returns>匹配的自定义特性 attributeType 对象, 或 null 如果不找到任何此类属性。</returns>
        public static Attribute GetCustomAttribute(this Assembly element, Type attributeType)
        {
            return Attribute.GetCustomAttribute(element, attributeType);
        }
        /// <summary>
        /// 检索指定类型的应用于指定的模块的自定义属性。
        /// </summary>
        /// <param name="element">要检查的模块。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <returns>匹配的自定义特性 attributeType 对象, 或 null 如果不找到任何此类属性。</returns>
        public static Attribute GetCustomAttribute(this Module element, Type attributeType)
        {
            return Attribute.GetCustomAttribute(element, attributeType);
        }
        /// <summary>
        /// 检索指定类型的应用于指定的成员的自定义属性。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <returns>匹配的自定义特性 attributeType 对象, 或 null 如果不找到任何此类属性。</returns>
        public static Attribute GetCustomAttribute(this MemberInfo element, Type attributeType)
        {
            return Attribute.GetCustomAttribute(element, attributeType);
        }
        /// <summary>
        /// 检索指定类型的应用于指定的参数的自定义属性。
        /// </summary>
        /// <param name="element">要检查的参数。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <returns>匹配的自定义特性 attributeType 对象, 或 null 如果不找到任何此类属性。</returns>
        public static Attribute GetCustomAttribute(this ParameterInfo element, Type attributeType)
        {
            return Attribute.GetCustomAttribute(element, attributeType);
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的程序集的特性的集合。
        /// </summary>
        /// <typeparam name="T">要搜索的属性的类型。</typeparam>
        /// <param name="element">要检查的程序集。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static T GetCustomAttribute<T>(this Assembly element) where T : Attribute
        {
            return (T)GetCustomAttribute(element, typeof(T));
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的模块的特性的集合。
        /// </summary>
        /// <typeparam name="T">要搜索的属性的类型。</typeparam>
        /// <param name="element">要检查的模块。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static T GetCustomAttribute<T>(this Module element) where T : Attribute
        {
            return (T)GetCustomAttribute(element, typeof(T));
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的成员的特性的集合。
        /// </summary>
        /// <typeparam name="T">要搜索的属性的类型。</typeparam>
        /// <param name="element">要检查的成员。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static T GetCustomAttribute<T>(this MemberInfo element) where T : Attribute
        {
            return (T)GetCustomAttribute(element, typeof(T));
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的参数的特性的集合。
        /// </summary>
        /// <typeparam name="T">要搜索的属性的类型。</typeparam>
        /// <param name="element">要检查的参数。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static T GetCustomAttribute<T>(this ParameterInfo element) where T : Attribute
        {
            return (T)GetCustomAttribute(element, typeof(T));
        }
        /// <summary>
        /// 检索应用于指定的成员，并根据需要检查该参数的祖先的指定类型的自定义属性。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <param name="inherit">true 若要检查的祖先 element; 否则为 false。</param>
        /// <returns>自定义特性匹配 attributeType, ，或 null 如果不找到任何此类属性。</returns>
        public static Attribute GetCustomAttribute(this MemberInfo element, Type attributeType, bool inherit)
        {
            return Attribute.GetCustomAttribute(element, attributeType, inherit);
        }
        /// <summary>
        /// 检索应用于指定的参数，并根据需要检查该参数的祖先的指定类型的自定义属性。
        /// </summary>
        /// <param name="element">要检查的参数。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <param name="inherit">true 若要检查的祖先 element; 否则为 false。</param>
        /// <returns>自定义特性匹配 attributeType, ，或 null 如果不找到任何此类属性。</returns>
        public static Attribute GetCustomAttribute(this ParameterInfo element, Type attributeType, bool inherit)
        {
            return Attribute.GetCustomAttribute(element, attributeType, inherit);
        }
        /// <summary>
        /// 检索指定类型的应用于指定的成员，并根据需要检查该成员的祖先的自定义属性。
        /// </summary>
        /// <typeparam name="T">要搜索的属性的类型。</typeparam>
        /// <param name="element">要检查的成员。</param>
        /// <param name="inherit">true 若要检查的祖先 element; 否则为 false。</param>
        /// <returns>匹配的自定义特性 T,，或 null 如果不找到任何此类属性。</returns>
        public static T GetCustomAttribute<T>(this MemberInfo element, bool inherit) where T : Attribute
        {
            return (T)GetCustomAttribute(element, typeof(T), inherit);
        }
        /// <summary>
        /// 检索指定类型的应用于指定的参数，并根据需要检查该成员的祖先的自定义属性。
        /// </summary>
        /// <typeparam name="T">要搜索的属性的类型。</typeparam>
        /// <param name="element">要检查的参数。</param>
        /// <param name="inherit">true 若要检查的祖先 element; 否则为 false。</param>
        /// <returns>匹配的自定义特性 T，或 null 如果不找到任何此类属性。</returns>
        public static T GetCustomAttribute<T>(this ParameterInfo element, bool inherit) where T : Attribute
        {
            return (T)GetCustomAttribute(element, typeof(T), inherit);
        }
#endregion

#region APIs that return all attributes
        /// <summary>
        /// 检索应用于指定的程序集的自定义特性的集合。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <returns>应用于的自定义特性的集合 element，或如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this Assembly element)
        {
            return Attribute.GetCustomAttributes(element);
        }
        /// <summary>
        /// 检索应用于指定的模块的自定义特性的集合。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <returns>应用于的自定义特性的集合 element，或如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this Module element)
        {
            return Attribute.GetCustomAttributes(element);
        }
        /// <summary>
        /// 检索应用于指定的成员的自定义特性的集合。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <returns>应用于的自定义特性的集合 element，或如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element)
        {
            return Attribute.GetCustomAttributes(element);
        }
        /// <summary>
        /// 检索应用于指定的参数的自定义特性的集合。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <returns>应用于的自定义特性的集合 element，或如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element)
        {
            return Attribute.GetCustomAttributes(element);
        }
        /// <summary>
        /// 检索应用于指定成员的自定义特性的指定类型的集合，并根据需要检查该成员的祖先。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <param name="inherit">true 若要检查的祖先 element; 否则为 false。</param>
        /// <returns>应用于的自定义特性的集合 element，或如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, bool inherit)
        {
            return Attribute.GetCustomAttributes(element, inherit);
        }
        /// <summary>
        /// 检索应用于指定参数的自定义特性的指定类型的集合，并根据需要检查该成员的祖先。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <param name="inherit">true 若要检查的祖先 element; 否则为 false。</param>
        /// <returns>应用于的自定义特性的集合 element，或如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, bool inherit)
        {
            return Attribute.GetCustomAttributes(element, inherit);
        }
#endregion

#region APIs that return all attributes of a particular type
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的程序集的特性的集合。
        /// </summary>
        /// <param name="element">要检查的程序集。</param>
        /// <param name="attributeType">要搜索的特性的类型。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this Assembly element, Type attributeType)
        {
            return Attribute.GetCustomAttributes(element, attributeType);
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的模块的特性的集合。
        /// </summary>
        /// <param name="element">要检查的模块。</param>
        /// <param name="attributeType">要搜索的特性的类型。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this Module element, Type attributeType)
        {
            return Attribute.GetCustomAttributes(element, attributeType);
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的成员的特性的集合。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <param name="attributeType">要搜索的特性的类型。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, Type attributeType)
        {
            return Attribute.GetCustomAttributes(element, attributeType);
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的参数的特性的集合。
        /// </summary>
        /// <param name="element">要检查的参数。</param>
        /// <param name="attributeType">要搜索的特性的类型。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, Type attributeType)
        {
            return Attribute.GetCustomAttributes(element, attributeType);
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的程序集的特性的集合。
        /// </summary>
        /// <typeparam name="T">要搜索的特性的类型。</typeparam>
        /// <param name="element">要检查的程序集。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this Assembly element) where T : Attribute
        {
            return (IEnumerable<T>)GetCustomAttributes(element, typeof(T));
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的模块的特性的集合。
        /// </summary>
        /// <typeparam name="T">要搜索的特性的类型。</typeparam>
        /// <param name="element">要检查的模块。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this Module element) where T : Attribute
        {
            return (IEnumerable<T>)GetCustomAttributes(element, typeof(T));
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的成员的特性的集合。
        /// </summary>
        /// <typeparam name="T">要搜索的特性的类型。</typeparam>
        /// <param name="element">要检查的成员。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element) where T : Attribute
        {
            return (IEnumerable<T>)GetCustomAttributes(element, typeof(T));
        }
        /// <summary>
        /// 检索具有指定类型的自定义应用于指定的参数的特性的集合。
        /// </summary>
        /// <typeparam name="T">要搜索的特性的类型。</typeparam>
        /// <param name="element">要检查的参数。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this ParameterInfo element) where T : Attribute
        {
            return (IEnumerable<T>)GetCustomAttributes(element, typeof(T));
        }
        /// <summary>
        /// 检索应用于指定成员的自定义特性的指定类型的集合，并根据需要检查该成员的祖先。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <param name="inherit">true 若要检查的祖先 element; 否则为 false。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, Type attributeType, bool inherit)
        {
            return Attribute.GetCustomAttributes(element, attributeType, inherit);
        }
        /// <summary>
        /// 检索应用于指定参数的自定义特性的指定类型的集合，并根据需要检查该成员的祖先。
        /// </summary>
        /// <param name="element">要检查的参数。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <param name="inherit">true 若要检查的祖先 element; 否则为 false。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, Type attributeType, bool inherit)
        {
            return Attribute.GetCustomAttributes(element, attributeType, inherit);
        }
        /// <summary>
        /// 检索应用于指定成员的指定类型自定义特性的集合，并根据需要检查该成员的祖先。
        /// </summary>
        /// <typeparam name="T">要搜索的特性的类型。</typeparam>
        /// <param name="element">要检查的成员。</param>
        /// <param name="inherit">true 若要检查的祖先 element; 否则为 false。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element, bool inherit) where T : Attribute
        {
            return (IEnumerable<T>)GetCustomAttributes(element, typeof(T), inherit);
        }
        /// <summary>
        /// 检索应用于指定参数的指定类型自定义特性的集合，并根据需要检查该成员的祖先。
        /// </summary>
        /// <typeparam name="T">要搜索的特性的类型。</typeparam>
        /// <param name="element">要检查的参数。</param>
        /// <param name="inherit">true 若要检查的祖先 element; 否则为 false。</param>
        /// <returns>应用于的自定义特性的集合 element 匹配 T, 如果没有此类属性存在为空集合。</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this ParameterInfo element, bool inherit) where T : Attribute
        {
            return (IEnumerable<T>)GetCustomAttributes(element, typeof(T), inherit);
        }
#endregion

#region IsDefined
        /// <summary>
        /// 指示是否为指定类型的自定义特性应用于指定的程序集。
        /// </summary>
        /// <param name="element">要检查的程序集。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <returns>true 表示指定类型的特性应用于 element; 否则为 false。</returns>
        public static bool IsDefined(this Assembly element, Type attributeType)
        {
            return Attribute.IsDefined(element, attributeType);
        }
        /// <summary>
        /// 指示是否为指定类型的自定义特性应用于指定的模块。
        /// </summary>
        /// <param name="element">要检查的模块。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <returns>true 表示指定类型的特性应用于 element; 否则为 false。</returns>
        public static bool IsDefined(this Module element, Type attributeType)
        {
            return Attribute.IsDefined(element, attributeType);
        }
        /// <summary>
        /// 指示是否为指定类型的自定义特性应用于指定的成员。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <returns>true 表示指定类型的特性应用于 element; 否则为 false。</returns>
        public static bool IsDefined(this MemberInfo element, Type attributeType)
        {
            return Attribute.IsDefined(element, attributeType);
        }
        /// <summary>
        /// 指示是否为指定类型的自定义特性应用于指定的参数。
        /// </summary>
        /// <param name="element">要检查的参数。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <returns>true 表示指定类型的特性应用于 element; 否则为 false。</returns>
        public static bool IsDefined(this ParameterInfo element, Type attributeType)
        {
            return Attribute.IsDefined(element, attributeType);
        }
        /// <summary>
        /// 该值指示是否指定类型的字段或其派生类型的一个或多个特性应用于此成员。
        /// </summary>
        /// <param name="element">要检查的成员。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <param name="inherit">true 搜索此成员继承链，以查找这些属性;否则为 false。</param>
        /// <returns>true 如果一个或多个实例 attributeType 或其派生任何的类型为应用于此成员; 否则为 false。</returns>
        public static bool IsDefined(this MemberInfo element, Type attributeType, bool inherit)
        {
            return Attribute.IsDefined(element, attributeType, inherit);
        }
        /// <summary>
        /// 该值指示是否指定类型的字段或其派生类型的一个或多个特性应用于此参数。
        /// </summary>
        /// <param name="element">要检查的参数。</param>
        /// <param name="attributeType">要搜索的属性的类型。</param>
        /// <param name="inherit">true 搜索此成员继承链，以查找这些属性;否则为 false。</param>
        /// <returns>true 如果一个或多个实例 attributeType 或其派生任何的类型为应用于此参数; 否则为 false。</returns>
        public static bool IsDefined(this ParameterInfo element, Type attributeType, bool inherit)
        {
            return Attribute.IsDefined(element, attributeType, inherit);
        }
#endregion
    }
    /// <summary>
    /// 定义<see cref="PropertyInfo"/>相关扩展方法。
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// 设置指定对象的属性值。
        /// </summary>
        /// <param name="propertyInfo"><see cref="PropertyInfo"/>实例对象。</param>
        /// <param name="obj">将设置其属性值的对象。</param>
        /// <param name="value">新的属性值。</param>
        public static void SetValue(this PropertyInfo propertyInfo, object obj, object value)
        {
            propertyInfo.SetValue(obj, value, null);
        }
    }
}
#endif
#if NET35
namespace Caredev.Mego
{
    using System;
    using System.Text;
    /// <summary>
    /// 定义<see cref="StringBuilder"/>相关扩展方法。
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// 从当前<see cref="StringBuilder"/>实例中移除所有字符。
        /// </summary>
        /// <param name="stringBuilder">要移除的目标对象。</param>
        /// <returns>其<see cref="StringBuilder.Length"/>为 0（零）的对象。</returns>
        public static StringBuilder Clear(this StringBuilder stringBuilder)
        {
            stringBuilder.Length = 0;
            return stringBuilder;
        }
    }
}
#endif