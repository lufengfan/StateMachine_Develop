﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu
{
    /// <summary>
    /// 为与某个值发生改变相关的事件（发生在改变后）提供数据。
    /// </summary>
    /// <typeparam name="T">值的类型。</typeparam>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        private T oldValue;
        private T newValue;

        /// <summary>
        /// 获取旧值。
        /// </summary>
        public T OldValue { get => this.oldValue; }
        /// <summary>
        /// 获取新值。
        /// </summary>
        public T NewValue { get => this.newValue; }

        /// <summary>
        /// 初始化 <see cref="ValueChangedEventArgs{T}"/> 类的新实例。实例使用指定的旧值和新值。
        /// </summary>
        /// <param name="oldValue">指定的旧值。</param>
        /// <param name="newValue">指定的新值。</param>
        public ValueChangedEventArgs(T oldValue, T newValue) : base()
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
