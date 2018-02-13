using Fusillade;
using Polly;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Xablu.WebApiClient.HttpExtensions
{
    public static class RestApiClientPollyExtensions
    {
        public static Task<IRestApiResult<TResult>> GetAsync<TResult>(this IRestApiClient apiClient,
            Priority priority,
            string path,
            int retryCount,
            int sleepDuration,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => apiClient.GetAsync<TResult>(priority, path, headers, httpResponseResolver, cancellationToken),
                retryCount,
                sleepDuration);
        }

        public static Task<IRestApiResult<TResult>> GetAsync<TResult>(this IRestApiClient apiClient, Priority priority,
            string path,
            int retryCount,
            Func<int, TimeSpan> sleepDurationProvider,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => apiClient.GetAsync<TResult>(priority, path, headers, httpResponseResolver, cancellationToken),
                retryCount,
                sleepDurationProvider);
        }

        public static Task<IRestApiResult<TResult>> PostAsync<TContent, TResult>(this IRestApiClient apiClient,
            Priority priority,
            string path,
            int retryCount,
            int sleepDuration,
            TContent content = default(TContent),
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver httpContentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => apiClient.PostAsync<TContent, TResult>(priority, path, content, headers, httpContentResolver, httpResponseResolver, cancellationToken),
                retryCount,
                sleepDuration);
        }

        public static Task<IRestApiResult<TResult>> PostAsync<TContent, TResult>(this IRestApiClient apiClient,
            Priority priority,
            string path,
            int retryCount,
            Func<int, TimeSpan> sleepDurationProvider,
            TContent content = default(TContent),
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver httpContentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => apiClient.PostAsync<TContent, TResult>(priority, path, content, headers, httpContentResolver, httpResponseResolver, cancellationToken),
                retryCount,
                sleepDurationProvider);
        }

        public static Task<IRestApiResult<TResult>> PutAsync<TContent, TResult>(this IRestApiClient apiClient,
            Priority priority,
            string path,
            int retryCount,
            int sleepDuration,
            TContent content = default(TContent),
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver httpContentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => apiClient.PutAsync<TContent, TResult>(priority, path, content, headers, httpContentResolver, httpResponseResolver, cancellationToken),
                retryCount,
                sleepDuration);
        }

        public static Task<IRestApiResult<TResult>> PutAsync<TContent, TResult>(this IRestApiClient apiClient,
            Priority priority,
            string path,
            int retryCount,
            Func<int, TimeSpan> sleepDurationProvider,
            TContent content = default(TContent),
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver httpContentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => apiClient.PutAsync<TContent, TResult>(priority, path, content, headers, httpContentResolver, httpResponseResolver, cancellationToken),
                retryCount,
                sleepDurationProvider);
        }

        public static Task<IRestApiResult<TResult>> DeleteAsync<TResult>(this IRestApiClient apiClient,
            Priority priority,
            string path,
            int retryCount,
            int sleepDuration,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => apiClient.DeleteAsync<TResult>(priority, path, headers, httpResponseResolver, cancellationToken),
                retryCount,
                sleepDuration);
        }

        public static Task<IRestApiResult<TResult>> DeleteAsync<TResult>(this IRestApiClient apiClient,
            Priority priority,
            string path,
            int retryCount,
            Func<int, TimeSpan> sleepDurationProvider,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => apiClient.DeleteAsync<TResult>(priority, path, headers, httpResponseResolver, cancellationToken),
                retryCount,
                sleepDurationProvider);
        }

        internal static Task<IRestApiResult<TResult>> PollyDecorator<TResult>(
            Func<Task<IRestApiResult<TResult>>> action, int retryCount, int sleepDurationInSeconds)
        {
            return PollyDecorator(
                action,
                retryCount,
                (retryAttemt) => TimeSpan.FromSeconds(sleepDurationInSeconds));
        }

        internal static async Task<IRestApiResult<TResult>> PollyDecorator<TResult>(
            Func<Task<IRestApiResult<TResult>>> action, int retryCount, Func<int, TimeSpan> sleepDurationProvider)
        {
            IRestApiResult<TResult> result = null;

            try
            {
                result = await Policy
                    .Handle<WebException>()
                    .WaitAndRetryAsync
                    (
                        retryCount: retryCount,
                        sleepDurationProvider: sleepDurationProvider
                    )
                    .ExecuteAsync(action).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (e.GetType().Namespace == "Java.Net")
                {
                    return new RestApiResult<TResult>(HttpStatusCode.RequestTimeout, default(TResult),
                        "Request Time-out", e.ToString());
                }
                throw;
            }

            return result;
        }
    }
}