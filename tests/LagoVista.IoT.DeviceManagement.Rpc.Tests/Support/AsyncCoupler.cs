using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests.Support
{
    public sealed class AsyncRequest<TResult>
    {
        public AsyncRequest(string correlationId)
        {
            CompletionSource = new TaskCompletionSource<TResult>();
            CorrelationId = correlationId;
            Details = new List<string>();
        }

        public List<string> Details { get; private set; }

        public string CorrelationId { get; private set; }

        public DateTime Enqueued { get; private set; }

        public TaskCompletionSource<TResult> CompletionSource { get; private set; }
    }

    public class AsyncCoupler : IAsyncCoupler
    {
        private readonly Guid _id = Guid.NewGuid();

        protected ILogger Logger { get; }
        protected IUsageMetrics UsageMetrics { get; private set; }
        protected ConcurrentDictionary<string, AsyncRequest<object>> Sessions { get; } = new ConcurrentDictionary<string, AsyncRequest<object>>();

        public AsyncCoupler(ILogger logger, IUsageMetrics usageMetrics)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            UsageMetrics = usageMetrics ?? throw new ArgumentNullException(nameof(usageMetrics));
            Console.WriteLine($"{_id} AsyncCoupler.CTOR");
        }

        public IUsageMetrics GetAndResetReadMetrics(DateTime dateStamp, string hostVersion)
        {
            UsageMetrics.Version = hostVersion;
            lock (UsageMetrics)
            {
                var clonedMetrics = UsageMetrics.Clone();
                clonedMetrics.SetDatestamp(dateStamp);

                clonedMetrics.EndTimeStamp = dateStamp.ToJSONString();
                clonedMetrics.StartTimeStamp = UsageMetrics.StartTimeStamp;
                clonedMetrics.Status = "Running";

                clonedMetrics.Calculate();

                UsageMetrics.Reset(clonedMetrics.EndTimeStamp);
                return clonedMetrics;
            }
        }

        public int ActiveSessions => Sessions.Count;

        public Task<InvokeResult> CompleteAsync<TAsyncResult>(string correlationId, TAsyncResult item)
        {
            Console.WriteLine($"{_id} AsyncCoupler.CompleteAsync: Sessions.Count: {Sessions.Count}");
            if (Sessions.TryRemove(correlationId, out var requestAwaiter))
            {
                Console.WriteLine($"{_id} AsyncCoupler.CompleteAsync: removed correlationId: {correlationId}");
                requestAwaiter.CompletionSource.SetResult(item);
                return Task.FromResult(InvokeResult.Success);
            }
            else
            {
                Console.WriteLine($"{_id} AsyncCoupler.CompleteAsync: correlationId not found: {correlationId}");
                return Task.FromResult(InvokeResult.FromErrors(new ErrorMessage($"Correlation id not found: {correlationId}.") { Details = $"CorrelationId={correlationId}" }));
            }
        }

        private void RegisterAsyncRequest(string correlationId)
        {
            if (!Sessions.TryAdd(correlationId, new AsyncRequest<object>(correlationId)))
            {
                Console.WriteLine($"{_id} AsyncCoupler.RegisterAsyncRequest: Could not add correlation id: {correlationId}");
                throw new Exception($"Could not add correlation id {correlationId}.");
            }
            UsageMetrics.ActiveCount++;
            Console.WriteLine($"{_id} AsyncCoupler.RegisterAsyncRequest: success: {correlationId}");
        }

        private AsyncRequest<object> Wait(string correlationId, TimeSpan timeout)
        {
            // if the async request isn't found it's because it's already been completed (race condition)
            if (Sessions.TryGetValue(correlationId, out var asyncRequest))
            {
                var timedOut = !asyncRequest.CompletionSource.Task.Wait(timeout);
                if (timedOut)
                {
                    Console.WriteLine($"{_id} AsyncCoupler.Wait: timed out waiting for: {correlationId}");

                    // no need to check success except for debugging
                    if (Sessions.TryRemove(correlationId, out var requestAwaiter))
                    {
                        Console.WriteLine($"{_id} AsyncCoupler.Wait - timed out: removed correlationId: {correlationId}");
                    }
                }
                Console.WriteLine($"{_id} AsyncCoupler.Wait: completed: {correlationId}");
                UsageMetrics.MessagesProcessed++;
                UsageMetrics.ActiveCount--;
                UsageMetrics.ElapsedMS = (DateTime.Now - asyncRequest.Enqueued).TotalMilliseconds;
            }
            return asyncRequest;
        }

        private Task<InvokeResult<TAsyncResult>> GetAsyncResult<TAsyncResult>(AsyncRequest<object> asyncRequest)
        {
            Console.WriteLine($"{_id} AsyncCoupler.GetAsyncResult: IsCompleted: {asyncRequest.CompletionSource.Task.IsCompleted}");
            if (!asyncRequest.CompletionSource.Task.IsCompleted)
            {
                UsageMetrics.ErrorCount++;
                return Task.FromResult(InvokeResult<TAsyncResult>.FromError("Timeout waiting for response."));
            }

            Console.WriteLine($"{_id} AsyncCoupler.GetAsyncResult: Result == null: {asyncRequest.CompletionSource.Task.Result == null}");
            if (asyncRequest.CompletionSource.Task.Result == null)
            {
                UsageMetrics.ErrorCount++;
                return Task.FromResult(InvokeResult<TAsyncResult>.FromError("Null Response From Completion Routine."));
            }

            var result = asyncRequest.CompletionSource.Task.Result;
            if (result is TAsyncResult typedResult)
            {
                Console.WriteLine($"{_id} AsyncCoupler.GetAsyncResult: returning typed result.");
                return Task.FromResult(InvokeResult<TAsyncResult>.Create(typedResult));
            }
            else
            {
                UsageMetrics.ErrorCount++;
                Console.WriteLine($"{_id} AsyncCoupler.GetAsyncResult: Type Mismatch - Expected: {typeof(TAsyncResult).Name} - Actual: {result.GetType().Name}.");
                return Task.FromResult(InvokeResult<TAsyncResult>.FromError($"Type Mismatch - Expected: {typeof(TAsyncResult).Name} - Actual: {result.GetType().Name}."));
            }
        }

        public Task<InvokeResult<TAsyncResult>> WaitOnAsync<TAsyncResult>(string correlationId, TimeSpan timeout)
        {
            Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: {correlationId}");
            try
            {
                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: RegisterAsyncRequest: {correlationId}");
                RegisterAsyncRequest(correlationId);

                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: Wait");
                var asyncRequest = Wait(correlationId, timeout);

                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: GetAsyncResult: {correlationId}");
                return GetAsyncResult<TAsyncResult>(asyncRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: {correlationId}: exception: {ex.GetType().Name}: {ex.Message}");
                Logger.AddException("AsyncCoupler_WaitOnAsync", ex);
                UsageMetrics.ErrorCount++;

                return Task.FromResult(InvokeResult<TAsyncResult>.FromException("AsyncCoupler_WaitOnAsync", ex));
            }
        }

        public Task<InvokeResult<TAsyncResult>> WaitOnAsync<TAsyncResult>(Action action, string correlationId, TimeSpan timeout)
        {
            try
            {
                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: RegisterAsyncRequest: {correlationId}");
                RegisterAsyncRequest(correlationId);

                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: action: {correlationId}");
                action();

                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: Wait: {correlationId}");
                var asyncRequest = Wait(correlationId, timeout);

                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: GetAsyncResult: {correlationId}");
                return GetAsyncResult<TAsyncResult>(asyncRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: {correlationId}: exception: {ex.GetType().Name}: {ex.Message}");
                Logger.AddException("AsyncCoupler_WaitOnAsync", ex);
                UsageMetrics.ErrorCount++;

                return Task.FromResult(InvokeResult<TAsyncResult>.FromException("AsyncCoupler_WaitOnAsync", ex));
            }
        }

        public async Task<InvokeResult<TAsyncResult>> WaitOnAsync<TAsyncResult>(Func<Task> function, string correlationId, TimeSpan timeout)
        {
            try
            {
                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: RegisterAsyncRequest: {correlationId}");
                RegisterAsyncRequest(correlationId);

                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: function: {correlationId}");
                await function();

                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: Wait: {correlationId}");
                var asyncRequest = Wait(correlationId, timeout);

                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: GetAsyncResult: {correlationId}");
                return await GetAsyncResult<TAsyncResult>(asyncRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{_id} AsyncCoupler.WaitOnAsync: {correlationId}: exception: {ex.GetType().Name}: {ex.Message}");
                Logger.AddException("AsyncCoupler_WaitOnAsync", ex);
                UsageMetrics.ErrorCount++;

                return await Task.FromResult(InvokeResult<TAsyncResult>.FromException("AsyncCoupler_WaitOnAsync", ex));
            }
        }
    }

    public class AsyncCoupler<TAsyncResult> : AsyncCoupler, IAsyncCoupler<TAsyncResult>
    {
        public AsyncCoupler(ILogger logger, IUsageMetrics usageMetrics) : base(logger, usageMetrics)
        {
        }

        public Task<InvokeResult> CompleteAsync(string correlationId, TAsyncResult item)
        {
            return CompleteAsync<TAsyncResult>(correlationId, item);
        }

        public Task<InvokeResult<TAsyncResult>> WaitOnAsync(string correlationId, TimeSpan timeout)
        {
            return WaitOnAsync<TAsyncResult>(correlationId, timeout);
        }

        public Task<InvokeResult<TAsyncResult>> WaitOnAsync(Action action, string correlationId, TimeSpan timeout)
        {
            return WaitOnAsync<TAsyncResult>(action, correlationId, timeout);
        }

        public Task<InvokeResult<TAsyncResult>> WaitOnAsync(Func<Task> function, string correlationId, TimeSpan timeout)
        {
            return WaitOnAsync<TAsyncResult>(function, correlationId, timeout);
        }
    }
}

