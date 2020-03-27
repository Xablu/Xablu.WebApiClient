using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;

using Polly;
using Polly.Wrap;
using Xablu.WebApiClient.Attributes;
using Xablu.WebApiClient.Enums;
using Xablu.WebApiClient.Services.GraphQL;
using Xablu.WebApiClient.Services.Rest;

namespace Xablu.WebApiClient
{
    public class WebApiClient<T> : IWebApiClient<T>
    { 
        private readonly IRefitService<T> _refitService;
        private readonly IGraphQLService _graphQLService;

        public WebApiClient(
            IRefitService<T> refitService,
            IGraphQLService graphQLService)
        {
            _refitService = refitService;
            _graphQLService = graphQLService;
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
            var service = _refitService.GetByPriority(priority);

            var policy = GetWrappedPolicy(retryCount, shouldRetry, timeout);

            Debug.WriteLine($"WebApiClient call with parameters: Priority: {priority}, retryCount: {retryCount}, has should retry condition: {shouldRetry != null}, timeout: {timeout}");
            
            await policy.ExecuteAsync(() => operation.Invoke(service));
        }

        public async Task<TResult> Call<TResult>(Func<T, Task<TResult>> operation, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout)
        {
            var service = _refitService.GetByPriority(priority);

            var policy = GetWrappedPolicy<TResult>(retryCount, shouldRetry, timeout);

            Debug.WriteLine($"WebApiClient call with parameters: Priority: {priority}, retryCount: {retryCount}, has should retry condition: {shouldRetry != null}, timeout: {timeout}");
            
            return await policy.ExecuteAsync(() => operation.Invoke(service));
        }

        public Task<TModel> SendQueryAsync<TModel>(Request<TModel> request, CancellationToken cancellationToken = default)
           where TModel : class, new()
        {
            return SendQueryAsync(request, GetDefaultOptions(), cancellationToken);
        }

        public Task<TModel> SendQueryAsync<TModel>(Request<TModel> request, RequestOptions options, CancellationToken cancellationToken = default)
            where TModel : class, new()
        {
            return SendQueryAsync(request, options.Priority, options.RetryCount, options.ShouldRetry, options.Timeout, cancellationToken);
        }

        public async Task<TModel> SendQueryAsync<TModel>(Request<TModel> request, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default)
            where TModel : class, new()
        {
            var service = _graphQLService.GetByPriority(priority);
            service.Options.EndPoint = new Uri(_graphQLService.BaseUrl + GetGraphQLEndpoint());

            var policy = GetWrappedPolicy(retryCount, shouldRetry, timeout);

            Debug.WriteLine($"WebApiClient call with parameters: Priority: {priority}, retryCount: {retryCount}, has should retry condition: {shouldRetry != null}, timeout: {timeout}");

            var result = await policy.ExecuteAsync(async () => await service.SendQueryAsync<TModel>(request, cancellationToken));

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
            var service = _graphQLService.GetByPriority(priority);
            service.Options.EndPoint = new Uri(_graphQLService.BaseUrl + GetGraphQLEndpoint());

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
            var service = _graphQLService.GetByPriority(priority);
            service.Options.EndPoint = new Uri(_graphQLService.BaseUrl + GetGraphQLEndpoint());

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

        private RequestOptions GetDefaultOptions()
        {
            return RequestOptions.DefaultRequestOptions
                ?? new RequestOptions
                {
                    Priority = RequestOptions.DefaultPriority,
                    RetryCount = RequestOptions.DefaultRetryCount,
                    Timeout = RequestOptions.DefaultTimeout,
                    ShouldRetry = RequestOptions.DefaultShouldRetry
                };
        }
    }
}
