﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.StateMachine.EventDriven
{
    /// <summary>
    /// 表示事件驱动的有限状态机。
    /// </summary>
    public class EventDrivenFSM : FSM<EventDrivenFSMState, EventDrivenFSMTransition>, IDisposable
    {
        /// <summary>
        /// 按照指定旧、新状态进行增量清除。
        /// </summary>
        /// <param name="oldState">指定的旧状态。</param>
        /// <param name="newState">指定的新状态。</param>
        protected void IncrementalClear(EventDrivenFSMState oldState, EventDrivenFSMState newState)
        {
            if (oldState == newState) return;

            foreach (var transition in
                (
                    oldState?.RecurGetTransitions<EventDrivenFSMState, EventDrivenFSMTransition>() ?? Enumerable.Empty<EventDrivenFSMTransition>()
                ).Except(
                    newState?.RecurGetTransitions<EventDrivenFSMState, EventDrivenFSMTransition>() ?? Enumerable.Empty<EventDrivenFSMTransition>()
                )
            )
            {
                transition.EventInvoke -= this.Transition_EventInvoke;
            }
        }

#pragma warning disable 1591
        protected override void this_StartStateChanged(object sender, ValueChangedEventArgs<EventDrivenFSMState> e)
        {
            base.this_StartStateChanged(sender, e);

            this.IncrementalClear(e.OldValue, e.NewValue);
        }
#pragma warning restore 1591

        /// <summary>
        /// 事件驱动的有限状态机模型（ <see cref="EventDrivenFSM"/> ）的状态（ <see cref="EventDrivenFSMTransition"/> ）的事件引发（ <see cref="EventDrivenFSMTransition.EventInvoke"/> ）事件的处理方法。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        protected virtual void Transition_EventInvoke(object sender, EventInvokeEventArgs e)
        {
            if (sender is EventDrivenFSMTransition transition)
            {
                if (this.CurrentState != null)
                    this.Transit(transition, e.InvokeArgs);
            }
        }

        /// <summary>
        /// <see cref="EventDrivenFSM"/> 的转换操作。此操作沿指定的转换进行，接受指定的参数。
        /// </summary>
        /// <param name="transition">指定的转换。</param>
        /// <param name="args">指定的参数。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public override bool Transit(EventDrivenFSMTransition transition, params object[] args)
        {
            return base.Transit(transition, new object[] { transition }.Concat(args ?? Enumerable.Empty<object>()).ToArray());
        }

        /// <summary>
        /// <see cref="EventDrivenFSM"/> 的转换操作。此操作将使有限状态机模型的当前状态转换为指定的状态，接受指定的参数。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="args">指定的参数。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        protected override bool Transit(EventDrivenFSMState state, EventDrivenFSMTransition transition, params object[] args)
        {
            // 引发退出动作。
            this.CurrentState?.ExitAction?.Invoke(this.CurrentState);

            // 切换当前状态。
            this.CurrentState = state;

            // 引发转换动作。
            transition.TransitAction?.Invoke(
                transition,
                args?.FirstOrDefault(),
                args?.Length == 0 ? null : args.Skip(1).ToArray()
            );

            // 引发进入动作。
            state?.EntryAction?.Invoke(state);

            return true;
        }

        /// <summary>
        /// 为 <see cref="EventDrivenFSM"/> 的一个指定状态添加指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool AttachTransition(EventDrivenFSMState state, EventDrivenFSMTransition transition)
        {
            if (this.States.Contains(state))
            {
                if (state.Transitions.Contains(transition))
                    return false;
                else
                {
                    state.AttachTransition(transition);
                    transition.EventInvoke += this.Transition_EventInvoke;
                    return true;
                }
            }
            else return false;
        }

        /// <summary>
        /// 从 <see cref="EventDrivenFSM"/> 的一个指定状态移除指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool RemoveTransition(EventDrivenFSMState state, EventDrivenFSMTransition transition)
        {
            if (this.States.Contains(state))
            {
                if (state.Transitions.Contains(transition))
                {
                    state.RemoveTransition(transition);
                    transition.EventInvoke -= this.Transition_EventInvoke;
                    return true;
                }
                else return false;
            }
            else return false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

#pragma warning disable 1591
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                this.StartState.RecurGetTransitions<EventDrivenFSMState, EventDrivenFSMTransition>().ToList().ForEach(transition => transition.EventInvoke -= this.Transition_EventInvoke);
                this.StartState = null;

                disposedValue = true;
            }
        }
#pragma warning restore 1591

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~EventDrivenFSM() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
