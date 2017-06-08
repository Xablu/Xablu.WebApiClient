﻿using Fusillade;
using Polly;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xablu.WebApiClient.Exceptions;

namespace Xablu.WebApiClient.HttpExtensions
{
    public static class WebApiClientPollyExtensions
    {
        public static Task<TResult> GetAsync<TResult>(this IWebApiClient webApiClient, Priority priority, string path, int retryCount, int sleepDuration, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => webApiClient.GetAsync<TResult>(priority, path, cancellationToken), 
                retryCount, 
                sleepDuration);
        }

        public static Task<TResult> GetAsync<TResult>(this IWebApiClient webApiClient, Priority priority, string path, int retryCount, Func<int, TimeSpan> sleepDurationProvider, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => webApiClient.GetAsync<TResult>(priority, path, cancellationToken),
                retryCount,
                sleepDurationProvider);
        }

        public static Task<TResult> PostAsync<TContent, TResult>(this IWebApiClient webApiClient, Priority priority, string path, int retryCount, int sleepDuration, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => webApiClient.PostAsync<TContent, TResult>(priority, path, cancellationToken: cancellationToken),
                retryCount,
                sleepDuration);
        }

        public static Task<TResult> PostAsync<TContent, TResult>(this IWebApiClient webApiClient, Priority priority, string path, int retryCount, Func<int, TimeSpan> sleepDurationProvider, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => webApiClient.PostAsync<TContent, TResult>(priority, path, cancellationToken: cancellationToken),
                retryCount,
                sleepDurationProvider);
        }

        public static Task<TResult> PutAsync<TContent, TResult>(this IWebApiClient webApiClient, Priority priority, string path, int retryCount, int sleepDuration, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => webApiClient.PutAsync<TContent, TResult>(priority, path, cancellationToken: cancellationToken),
                retryCount,
                sleepDuration);
        }

        public static Task<TResult> PutAsync<TContent, TResult>(this IWebApiClient webApiClient, Priority priority, string path, int retryCount, Func<int, TimeSpan> sleepDurationProvider, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => webApiClient.PutAsync<TContent, TResult>(priority, path, cancellationToken: cancellationToken),
                retryCount,
                sleepDurationProvider);
        }

        public static Task<TResult> DeleteAsync<TResult>(this IWebApiClient webApiClient, Priority priority, string path, int retryCount, int sleepDuration, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => webApiClient.DeleteAsync<TResult>(priority, path, cancellationToken),
                retryCount,
                sleepDuration);
        }

        public static Task<TResult> DeleteAsync<TResult>(this IWebApiClient webApiClient, Priority priority, string path, int retryCount, Func<int, TimeSpan> sleepDurationProvider, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PollyDecorator(
                () => webApiClient.DeleteAsync<TResult>(priority, path, cancellationToken),
                retryCount,
                sleepDurationProvider);
        }

        internal static Task<TResult> PollyDecorator<TResult>(Func<Task<TResult>> action, int retryCount, int sleepDurationInSeconds)
        {
            return PollyDecorator(
                action,
                retryCount,
                (retryAttemt) => TimeSpan.FromSeconds(sleepDurationInSeconds));
        }

        internal static async Task<TResult> PollyDecorator<TResult>(Func<Task<TResult>> action, int retryCount, Func<int, TimeSpan> sleepDurationProvider)
        {
            TResult result = default(TResult);

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
                    throw new HttpResponseException(HttpStatusCode.RequestTimeout, e.Message, e.StackTrace);
                }
                throw;
            }

            return result;
        }
    }
}
