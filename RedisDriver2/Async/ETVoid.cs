﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RedisDriver.Async
{
    [AsyncMethodBuilder(typeof(AsyncETVoidMethodBuilder))]
    public struct ETVoid
    {
        public void NoAwait()
        {
        }

        [DebuggerHidden]
        public Awaiter GetAwaiter()
        {
            return new Awaiter();
        }

        public struct Awaiter : ICriticalNotifyCompletion
        {
            [DebuggerHidden]
            public bool IsCompleted => true;

            [DebuggerHidden]
            public void GetResult()
            {
                throw new InvalidOperationException("ETAvoid cant await, use NoWait method instead!");
            }

            [DebuggerHidden]
            public void OnCompleted(Action continuation)
            {
            }

            [DebuggerHidden]
            public void UnsafeOnCompleted(Action continuation)
            {
            }
        }
    }
}