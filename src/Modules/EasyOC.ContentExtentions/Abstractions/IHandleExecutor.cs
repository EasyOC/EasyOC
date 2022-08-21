using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.ContentExtentions.Handlers
{
    public interface IHandleExecutor
    {
        IEnumerable<TResult> Invoke<TEvents, T1, TResult>(IEnumerable<TEvents> events, Func<TEvents, T1, TResult> dispatch, T1 arg1);
        void Invoke<TEvents, T1>(IEnumerable<TEvents> events, Action<TEvents, T1> dispatch, T1 arg1);
        IEnumerable<TResult> Invoke<TEvents, TResult>(IEnumerable<TEvents> events, Func<TEvents, IEnumerable<TResult>> dispatch);
        IEnumerable<TResult> Invoke<TEvents, TResult>(IEnumerable<TEvents> events, Func<TEvents, TResult> dispatch);
        void Invoke<TEvents>(IEnumerable<TEvents> events, Action<TEvents> dispatch);
        Task InvokeAsync<TEvents, T1, T2, T3, T4, T5>(IEnumerable<TEvents> events, Func<TEvents, T1, T2, T3, T4, T5, Task> dispatch, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        Task InvokeAsync<TEvents, T1, T2, T3, T4>(IEnumerable<TEvents> events, Func<TEvents, T1, T2, T3, T4, Task> dispatch, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        Task InvokeAsync<TEvents, T1, T2, T3>(IEnumerable<TEvents> events, Func<TEvents, T1, T2, T3, Task> dispatch, T1 arg1, T2 arg2, T3 arg3);
        Task InvokeAsync<TEvents, T1, T2>(IEnumerable<TEvents> events, Func<TEvents, T1, T2, Task> dispatch, T1 arg1, T2 arg2);
        Task<IEnumerable<TResult>> InvokeAsync<TEvents, T1, TResult>(IEnumerable<TEvents> events, Func<TEvents, T1, Task<TResult>> dispatch, T1 arg1);
        Task InvokeAsync<TEvents, T1>(IEnumerable<TEvents> events, Func<TEvents, T1, Task> dispatch, T1 arg1);
        Task<IEnumerable<TResult>> InvokeAsync<TEvents, TResult>(IEnumerable<TEvents> events, Func<TEvents, Task<IEnumerable<TResult>>> dispatch);
        Task<IEnumerable<TResult>> InvokeAsync<TEvents, TResult>(IEnumerable<TEvents> events, Func<TEvents, Task<TResult>> dispatch);
        Task InvokeAsync<TEvents>(IEnumerable<TEvents> events, Func<TEvents, Task> dispatch);
    };
    public interface IHandleExecutor<TCorrelationType> : IHandleExecutor
        where TCorrelationType : class
    {

    }
}
