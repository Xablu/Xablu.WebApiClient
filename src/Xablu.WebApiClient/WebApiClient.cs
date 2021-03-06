using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Polly;
using Polly.Wrap;
using Refit;
using Xablu.WebApiClient.Attributes;
using Xablu.WebApiClient.Enums;
using Xablu.WebApiClient.Services.GraphQL;
using Xablu.WebApiClient.Services.Rest;

namespace Xablu.WebApiClient
{
    public class WebApiClient
    {
        /// <summary>
        /// Default RequestOptions object. If defined, it will be used when no options are specified
        /// </summary>
        public static RequestOptions DefaultRequestOptions { get; set; }

        /// <summary>
        /// Default value for retry intents
        /// </summary>
        public static int DefaultRetryCount { get; set; } = 1;

        /// <summary>
        /// Default timeout for requests
        /// </summary>
        public static int DefaultTimeout { get; set; } = 60;

        /// <summary>
        /// Default priority for requests. Matches fusillade policy
        /// </summary>
        public static Priority DefaultPriority { get; set; } = Priority.UserInitiated;

        /// <summary>
        /// Default should retry condition. Default value is null
        /// </summary>
        public static Func<Exception, bool> DefaultShouldRetry { get; set; }

        protected RequestOptions GetDefaultOptions()
        {
            return DefaultRequestOptions
                ?? new RequestOptions
                {
                    Priority = DefaultPriority,
                    RetryCount = DefaultRetryCount,
                    Timeout = DefaultTimeout,
                    ShouldRetry = DefaultShouldRetry
                };
        }
    }

    public class WebApiClient<T> : WebApiClient, IWebApiClient<T>
        where T : class
    { 
        private readonly Lazy<IRefitService<T>> _refitService;
        private readonly Lazy<IGraphQLService> _graphQLService;

        internal WebApiClient(
            string baseUrl,
            bool autoRedirectRequests = true,
            Func<DelegatingHandler> delegatingHandler = default,
            IDictionary<string, string> defaultHeaders = default,
            RefitSettings refitSettings = null)
        {
            _refitService = new Lazy<IRefitService<T>>(() => new RefitService<T>(baseUrl, autoRedirectRequests, delegatingHandler, defaultHeaders, refitSettings));
            _graphQLService = new Lazy<IGraphQLService>(() => new GraphQLService(baseUrl, autoRedirectRequests, delegatingHandler, defaultHeaders));
        }

        public Task Call(Func<T, Task> operation)
        {
            return Call(operation, GetDefaultOptions());
        }

        public Task<TResult> Call<TResult>(Func<T, Task<TResult>> operation)
        {
            return Call<TResult>(operation, GetDefaultOptions());
        }
         
        public Task Call(Func<T, Task> operation, RequestOptions options)
        {
            return Call(operation, options.Priority, options.RetryCount, options.ShouldRetry, options.Timeout);
        }

        public Task<TResult> Call<TResult>(Func<T, Task<TResult>> operation, RequestOptions options)
        {
            return Call<TResult>(operation, options.Priority, options.RetryCount, options.ShouldRetry, options.Timeout);
        }

        public async Task Call(Func<T, Task> operation, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout)
        {
            var service = _refitService.Value.GetByPriority(priority);

            var policy = GetWrappedPolicy(retryCount, shouldRetry, timeout);

            Debug.WriteLine($"WebApiClient call with parameters: Priority: {priority}, retryCount: {retryCount}, has should retry condition: {shouldRetry != null}, timeout: {timeout}");
            
            await policy.ExecuteAsync(() => operation.Invoke(service));
        }

        public async Task<TResult> Call<TResult>(Func<T, Task<TResult>> operation, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout)
        {
            var service = _refitService.Value.GetByPriority(priority);

            var policy = GetWrappedPolicy<TResult>(retryCount, shouldRetry, timeout);

            Debug.WriteLine($"WebApiClient call with parameters: Priority: {priority}, retryCount: {retryCount}, has should retry condition: {shouldRetry != null}, timeout: {timeout}");
            
            return await policy.ExecuteAsync(() => operation.Invoke(service));
        }

        public Task<TResponseModel> SendQueryAsync<TResponseModel>(QueryRequest<TResponseModel> request, CancellationToken cancellationToken = default)
           where TResponseModel : class, new()
        {
            return SendQueryAsync(request, GetDefaultOptions(), cancellationToken);
        }

        public Task<TResponseModel> SendQueryAsync<TResponseModel>(QueryRequest<TResponseModel> request, RequestOptions options, CancellationToken cancellationToken = default)
            where TResponseModel : class, new()
        {
            return SendQueryAsync(request, options.Priority, options.RetryCount, options.ShouldRetry, options.Timeout, cancellationToken);
        }

        public async Task<TResponseModel> SendQueryAsync<TResponseModel>(QueryRequest<TResponseModel> request, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default)
            where TResponseModel : class, new()
        {  
            var service = _graphQLService.Value.GetByPriority(priority);
            service.Options.EndPoint = new Uri(_graphQLService.Value.BaseUrl + GetGraphQLEndpoint());

            var policy = GetWrappedPolicy(retryCount, shouldRetry, timeout);

            Debug.WriteLine($"WebApiClient call with parameters: Priority: {priority}, retryCount: {retryCount}, has should retry condition: {shouldRetry != null}, timeout: {timeout}");

            var result = await policy.ExecuteAsync(async () => await service.SendQueryAsync<TResponseModel>(request, cancellationToken));

            if (result.Errors != null && result.Errors.Any())
            {
                string message = "GraphQL" + (result.Errors.Length == 1 ? " Error: " : " Errors: ");
                message += string.Join(" /// ", result.Errors.Select(e => e.Message));
                throw new Exception(message);
            }

            return result.Data;
        }

        public Task<TResponseModel> SendMutationAsync<TResponseModel>(MutationRequest<TResponseModel> request, CancellationToken cancellationToken = default)
            where TResponseModel : class, new()
        {
            return SendMutationAsync(request, GetDefaultOptions(), cancellationToken);
        }

        public Task<TResponseModel> SendMutationAsync<TResponseModel>(MutationRequest<TResponseModel> request, RequestOptions options, CancellationToken cancellationToken = default)
            where TResponseModel : class, new()
        {
            return SendMutationAsync(request, options.Priority, options.RetryCount, options.ShouldRetry, options.Timeout, cancellationToken);
        }
         
        public async Task<TResponseModel> SendMutationAsync<TResponseModel>(MutationRequest<TResponseModel> request, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default)
            where TResponseModel : class, new()
        { 
            var service = _graphQLService.Value.GetByPriority(priority);
            service.Options.EndPoint = new Uri(_graphQLService.Value.BaseUrl + GetGraphQLEndpoint()); 

            var policy = GetWrappedPolicy(retryCount, shouldRetry, timeout);

            Debug.WriteLine($"WebApiClient call with parameters: Priority: {priority}, retryCount: {retryCount}, has should retry condition: {shouldRetry != null}, timeout: {timeout}");

            var result = await policy.ExecuteAsync(async () => await service.SendMutationAsync<TResponseModel>(request, cancellationToken));

            if (result.Errors != null && result.Errors.Any())
            {
                string message = "GraphQL" + (result.Errors.Length == 1 ? " Error: " : " Errors: ");
                message += string.Join(" /// ", result.Errors.Select(e => e.Message));
                throw new Exception(message);
            }

            return result.Data;
        }

        public Task<TResponseModel> SendMutationAsync<TResponseModel>(string mutationString, object queryVariable, CancellationToken cancellationToken = default)
            where TResponseModel : class, new()
        {
            return SendMutationAsync<TResponseModel>(mutationString, queryVariable, GetDefaultOptions(), cancellationToken);
        }

        public Task<TResponseModel> SendMutationAsync<TResponseModel>(string mutationString, object queryVariable, RequestOptions options, CancellationToken cancellationToken = default)
            where TResponseModel : class, new()
        {
            return SendMutationAsync<TResponseModel>(mutationString, queryVariable, options.Priority, options.RetryCount, options.ShouldRetry, options.Timeout, cancellationToken);
        }

        public async Task<TResponseModel> SendMutationAsync<TResponseModel>(string mutationString, object queryVariables, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default)
           where TResponseModel : class, new()
        { 
            var service = _graphQLService.Value.GetByPriority(priority);
            service.Options.EndPoint = new Uri(_graphQLService.Value.BaseUrl + GetGraphQLEndpoint());

            var policy = GetWrappedPolicy(retryCount, shouldRetry, timeout);

            Debug.WriteLine($"WebApiClient call with parameters: Priority: {priority}, retryCount: {retryCount}, has should retry condition: {shouldRetry != null}, timeout: {timeout}");

            var mutationRequest = new GraphQLRequest
            {
                Query = mutationString,
                Variables = queryVariables
            };

            var result = await policy.ExecuteAsync(async () => await service.SendMutationAsync<TResponseModel>(mutationRequest, cancellationToken));

            if (result.Errors != null && result.Errors.Any())
            {
                string message = "GraphQL" + (result.Errors.Length == 1 ? " Error: " : " Errors: ");
                message += string.Join(" /// ", result.Errors.Select(e => e.Message));
                throw new Exception(message);
            }
             
            return result.Data;
        }

        private static AsyncPolicyWrap GetWrappedPolicy(int retryCount, Func<Exception, bool> shouldRetry, int timeout)
        {
            var retryPolicy = Policy.Handle<Exception>(e => shouldRetry?.Invoke(e) ?? true)
                                    .RetryAsync(retryCount,
                                                onRetry: (Exception ex, int count) => Debug.WriteLine($"Retrying call. Count: {count} | Exception: {ex.Message}"));
            var timeoutPolicy = Policy.TimeoutAsync(timeout,
                                                    onTimeoutAsync: async (context, span, task) => Debug.WriteLine("Call timed out"));

            return Policy.WrapAsync(retryPolicy, timeoutPolicy);
        }

        private static AsyncPolicyWrap<TResult> GetWrappedPolicy<TResult>(int retryCount, Func<Exception, bool> shouldRetry, int timeout)
        {
            var retryPolicy = Policy.Handle<Exception>(e => shouldRetry?.Invoke(e) ?? true)
                                    .RetryAsync(retryCount,
                                                onRetry: (Exception ex, int count) => Debug.WriteLine($"Retrying call. Count: {count} | Exception: {ex.Message}"))
                                    .AsAsyncPolicy<TResult>();
            var timeoutPolicy = Policy.TimeoutAsync<TResult>(timeout,
                                                             onTimeoutAsync: async (context, span, task) => Debug.WriteLine("Call timed out"));

            return Policy.WrapAsync<TResult>(retryPolicy, timeoutPolicy);
        }

        private static string GetGraphQLEndpoint()
        {
            var attributes = typeof(T).GetCustomAttributes(false);
            var graphQLEndpointAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(GraphQLEndpointAttribute));
            if (graphQLEndpointAttribute == null)
            {
                throw new RequestException("GraphQL endpoint not found");
            }
            return ((GraphQLEndpointAttribute)graphQLEndpointAttribute).Path;
        }
    }
}
