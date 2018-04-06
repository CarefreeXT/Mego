// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators
{
    using Caredev.Mego.Resolve.Generators.Fragments;
    using System;
    using System.Collections.Generic;
    using System.Text;
    /// <summary>
    /// SQL 语句写入器。
    /// </summary>
    public class SqlWriter
    {
        private readonly StringBuilder writer = new StringBuilder();
        private readonly Dictionary<string, object> _Propertys = new Dictionary<string, object>();
        /// <summary>
        /// 当前写入的源对象。
        /// </summary>
        public SourceFragment Current { get; private set; }
        /// <summary>
        /// 指定源设置<see cref="Current"/>，然后执行相应活动。
        /// </summary>
        /// <param name="action">执行活动。</param>
        /// <param name="fragment">目标源。</param>
        public void Enter(Action action, SourceFragment fragment)
        {
            var old = Current;
            Current = fragment;
            action();
            Current = old;
        }
        /// <summary>
        /// 设置写入器属性值。
        /// </summary>
        /// <param name="key">属性名。</param>
        /// <param name="value">属性值。</param>
        public void SetProperty(string key, object value)
        {
            lock (_Propertys)
            {
                if (_Propertys.ContainsKey(key))
                {
                    if (value == null)
                    {
                        _Propertys.Remove(key);
                    }
                    else
                    {
                        _Propertys[key] = value;
                    }
                }
                else if (value != null)
                {
                    _Propertys.Add(key, value);
                }
            }
        }
        /// <summary>
        /// 获取指定的属性值。
        /// </summary>
        /// <typeparam name="T">属性类型。</typeparam>
        /// <param name="key">属性名。</param>
        /// <returns>返回找到的属性值，如果属性不存则返回默认值。</returns>
        public T GetProperty<T>(string key)
        {
            if (_Propertys.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            return default(T);
        }
        /// <inheritdoc/>
        public override string ToString() => writer.ToString();
        /// <summary>
        /// 写入行结束符。
        /// </summary>
        public void WriteLine() => writer.AppendLine();
        /// <summary>
        /// 写入指定内容和行结束符。
        /// </summary>
        /// <param name="content">需要写入的内容。</param>
        public void WriteLine(string content) => writer.AppendLine(content);
        /// <summary>
        /// 写入一个<see cref="bool"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(bool value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="char"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(char value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="ulong"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(ulong value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="uint"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(uint value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="byte"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(byte value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="string"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(string value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="float"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(float value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="ushort"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(ushort value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="object"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(object value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="char"/>数组值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(char[] value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="sbyte"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(sbyte value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="decimal"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(decimal value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="short"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(short value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="int"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(int value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="long"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(long value) => writer.Append(value);
        /// <summary>
        /// 写入一个<see cref="double"/>值。
        /// </summary>
        /// <param name="value">写入的值。</param>
        public void Write(double value) => writer.Append(value);
    }
}