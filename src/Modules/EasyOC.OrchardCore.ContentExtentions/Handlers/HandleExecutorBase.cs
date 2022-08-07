using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.ContentExtentions.Handlers
{
    public class HandleExecutorBase<TCorrelationType> : IHandleExecutor<TCorrelationType> where TCorrelationType : class
    {
        private readonly INotifier _notifier;
        private readonly ILogger _logger;
        private readonly ISession _session;
        private readonly IHtmlLocalizer H;
        public HandleExecutorBase(INotifier notifier, ILogger<TCorrelationType> logger, IHtmlLocalizer<TCorrelationType> h, ISession session)
        {
            _notifier = notifier;
            _logger = logger;
            H = h;
            _session = session;
        }
        protected virtual void HandleException(Exception ex, string sourceType, string method)
        {
            _notifier.ErrorAsync(H["Opration faild."]);

            if (_session.CurrentTransaction != null)
            {
                _session.CancelAsync();
            }
            if (IsLogged(ex))
            {
                _logger.LogError(ex, "{Type} thrown from {Method} by {Exception}",
                     sourceType,
                     method,
                     ex.GetType().Name);
            }

            if (ex.IsFatal())
            {
                throw ex;
            }
        }
        private static bool IsLogged(Exception ex)
        {
            return !ex.IsFatal();
        }
        #region Bellow codes Just for Sample

        ///UNDONE: Rewrite extention methods

        /// <summary>
        /// Safely invoke methods by catching non fatal exceptions and logging them
        /// </summary>
        public virtual void Invoke<TEvents>(IEnumerable<TEvents> events, Action<TEvents> dispatch)
        {
            foreach (var sink in events)
            {
                try
                {
                    dispatch(sink);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        /// <summary>
        /// Safely invoke methods by catching non fatal exceptions and logging them
        /// </summary>
        public virtual void Invoke<TEvents, T1>(IEnumerable<TEvents> events, Action<TEvents, T1> dispatch, T1 arg1)
        {
            foreach (var sink in events)
            {
                try
                {
                    dispatch(sink, arg1);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        public virtual IEnumerable<TResult> Invoke<TEvents, TResult>(IEnumerable<TEvents> events, Func<TEvents, TResult> dispatch)
        {
            var results = new List<TResult>();

            foreach (var sink in events)
            {
                try
                {
                    var result = dispatch(sink);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        public virtual IEnumerable<TResult> Invoke<TEvents, T1, TResult>(IEnumerable<TEvents> events, Func<TEvents, T1, TResult> dispatch, T1 arg1)
        {
            var results = new List<TResult>();

            foreach (var sink in events)
            {
                try
                {
                    var result = dispatch(sink, arg1);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        public virtual IEnumerable<TResult> Invoke<TEvents, TResult>(IEnumerable<TEvents> events, Func<TEvents, IEnumerable<TResult>> dispatch)
        {
            var results = new List<TResult>();

            foreach (var sink in events)
            {
                try
                {
                    var result = dispatch(sink);
                    results.AddRange(result);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        /// <summary>
        /// Safely invoke methods by catching non fatal exceptions and logging them
        /// </summary>
        public virtual async Task InvokeAsync<TEvents>(IEnumerable<TEvents> events, Func<TEvents, Task> dispatch)
        {
            foreach (var sink in events)
            {
                try
                {
                    await dispatch(sink);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        /// <summary>
        /// Safely invoke methods by catching non fatal exceptions and logging them
        /// </summary>
        public virtual async Task InvokeAsync<TEvents, T1>(IEnumerable<TEvents> events, Func<TEvents, T1, Task> dispatch, T1 arg1)
        {
            foreach (var sink in events)
            {
                try
                {
                    await dispatch(sink, arg1);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        /// <summary>
        /// Safely invoke methods by catching non fatal exceptions and logging them
        /// </summary>
        public virtual async Task InvokeAsync<TEvents, T1, T2>(IEnumerable<TEvents> events, Func<TEvents, T1, T2, Task> dispatch, T1 arg1, T2 arg2)
        {
            foreach (var sink in events)
            {
                try
                {
                    await dispatch(sink, arg1, arg2);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        /// <summary>
        /// Safely invoke methods by catching non fatal exceptions and logging them
        /// </summary>
        public virtual async Task InvokeAsync<TEvents, T1, T2, T3>(IEnumerable<TEvents> events, Func<TEvents, T1, T2, T3, Task> dispatch, T1 arg1, T2 arg2, T3 arg3)
        {
            foreach (var sink in events)
            {
                try
                {
                    await dispatch(sink, arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        /// <summary>
        /// Safely invoke methods by catching non fatal exceptions and logging them
        /// </summary>
        public virtual async Task InvokeAsync<TEvents, T1, T2, T3, T4>(IEnumerable<TEvents> events, Func<TEvents, T1, T2, T3, T4, Task> dispatch, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            foreach (var sink in events)
            {
                try
                {
                    await dispatch(sink, arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        /// <summary>
        /// Safely invoke methods by catching non fatal exceptions and logging them
        /// </summary>
        public virtual async Task InvokeAsync<TEvents, T1, T2, T3, T4, T5>(IEnumerable<TEvents> events, Func<TEvents, T1, T2, T3, T4, T5, Task> dispatch, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            foreach (var sink in events)
            {
                try
                {
                    await dispatch(sink, arg1, arg2, arg3, arg4, arg5);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        public virtual async Task<IEnumerable<TResult>> InvokeAsync<TEvents, TResult>(IEnumerable<TEvents> events, Func<TEvents, Task<TResult>> dispatch)
        {
            var results = new List<TResult>();

            foreach (var sink in events)
            {
                try
                {
                    var result = await dispatch(sink);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        public virtual async Task<IEnumerable<TResult>> InvokeAsync<TEvents, T1, TResult>(IEnumerable<TEvents> events, Func<TEvents, T1, Task<TResult>> dispatch, T1 arg1)
        {
            var results = new List<TResult>();

            foreach (var sink in events)
            {
                try
                {
                    var result = await dispatch(sink, arg1);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        public virtual async Task<IEnumerable<TResult>> InvokeAsync<TEvents, TResult>(IEnumerable<TEvents> events, Func<TEvents, Task<IEnumerable<TResult>>> dispatch)
        {
            var results = new List<TResult>();

            foreach (var sink in events)
            {
                try
                {
                    var result = await dispatch(sink);
                    results.AddRange(result);
                }
                catch (Exception ex)
                {
                    HandleException(ex, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }
        #endregion



    }
}
