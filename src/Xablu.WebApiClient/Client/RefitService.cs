using System;
using System.Net.Http;
using Fusillade;
using Refit;
using Xablu.WebApiClient.Logging;

namespace Xablu.WebApiClient.Client
{
    public class RefitService<T> : IRefitService<T>
    {
        private static readonly ILog Logger = LogProvider.For<RefitService<T>>();

        private readonly Func<DelegatingHandler> _delegatingHandler;

        private readonly Lazy<T> _background;
        private readonly Lazy<T> _userInitiated;
        private readonly Lazy<T> _speculative;

        public RefitService(string apiBaseAddress, bool autoRedirectRequests, Func<DelegatingHandler> delegatingHandler = null)
        {
            if (string.IsNullOrEmpty(apiBaseAddress))
                throw new ArgumentNullException(nameof(apiBaseAddress));

            _delegatingHandler = delegatingHandler;

            if (Logger.IsTraceEnabled())
            {
                Logger.Trace($"Base url set to: {apiBaseAddress} and delegatingHandler: {delegatingHandler != null}");
            }

            Func<HttpMessageHandler, T> createClient = messageHandler =>
            {
                HttpMessageHandler handler;

                if (_delegatingHandler != null)
                {
                    var delegatingHandlerInstance = _delegatingHandler.Invoke();
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

                return RestService.For<T>(client);
            };

            _background = new Lazy<T>(() => createClient(NetCache.Background));

            _userInitiated = new Lazy<T>(() => createClient(NetCache.UserInitiated));

            _speculative = new Lazy<T>(() => createClient(NetCache.Speculative));
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

        public T Background => _background.Value;

        public T UserInitiated => _userInitiated.Value;

        public T Speculative => _speculative.Value;
    }
}