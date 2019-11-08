using System;
using System.Collections.Generic;
using System.Net.Http;
using Fusillade;
using Refit;
using Xablu.WebApiClient.Native;
using Xablu.WebApiClient.Logging; 

namespace Xablu.WebApiClient.Services.Rest
{
    public class RefitService<T> : IRefitService<T>
        where T : class
    {
        private static readonly ILog Logger = LogProvider.For<RefitService<T>>();
         
        private readonly Lazy<T> _background;
        private readonly Lazy<T> _userInitiated;
        private readonly Lazy<T> _speculative;

        public RefitService(string apiBaseAddress, bool autoRedirectRequests, Func<DelegatingHandler> delegatingHandler, IDictionary<string, string> defaultHeaders)
        {
            if (string.IsNullOrEmpty(apiBaseAddress))
                throw new ArgumentNullException(nameof(apiBaseAddress));
             
            if (Logger.IsTraceEnabled())
            {
                Logger.Trace($"Base url set to: {apiBaseAddress} and delegatingHandler: {delegatingHandler != null}");
            }

            Func<HttpMessageHandler, T> createClient = messageHandler =>
            { 
                HttpMessageHandler handler;

                if (delegatingHandler != null)
                {
                    var delegatingHandlerInstance = delegatingHandler.Invoke();
                    delegatingHandlerInstance.InnerHandler = messageHandler;
                    handler = delegatingHandlerInstance; 
                }
                else
                {
                    handler = messageHandler; 
                }

                if(!autoRedirectRequests)
                    DisableAutoRedirects(messageHandler);

                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(apiBaseAddress)
                };

                if (defaultHeaders != default)
                {
                    foreach(var header in defaultHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                return RestService.For<T>(client);
            };

            _background = new Lazy<T>(() => createClient(new RateLimitedHttpMessageHandler(new NativeHttpClientHandler(), Priority.Background)));

            _userInitiated = new Lazy<T>(() => createClient(new RateLimitedHttpMessageHandler(new NativeHttpClientHandler(), Priority.UserInitiated)));

            _speculative = new Lazy<T>(() => createClient(new RateLimitedHttpMessageHandler(new NativeHttpClientHandler(), Priority.Speculative)));
        }

        protected virtual void DisableAutoRedirects(HttpMessageHandler messageHandler)
        {
            if (messageHandler is DelegatingHandler internalDelegate
                && internalDelegate.InnerHandler is HttpClientHandler internalClientHandler)
            {
                if (Logger.IsTraceEnabled())
                {
                    Logger.Trace("Disabling AutoRedirects");
                }
                internalClientHandler.AllowAutoRedirect = false;
            }
        }

        public T GetByPriority(Enums.Priority priority)
        {
            switch (priority)
            {
                case Enums.Priority.Background:
                    return _background.Value;
                case Enums.Priority.Speculative:
                    return _speculative.Value;
                case Enums.Priority.UserInitiated:
                default:
                    return _userInitiated.Value;
            }
        } 
    }
}