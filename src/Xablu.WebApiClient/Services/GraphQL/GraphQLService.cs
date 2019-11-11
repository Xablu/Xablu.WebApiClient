using System;
using System.Collections.Generic;
using System.Net.Http;
using Fusillade;
using GraphQL.Client.Http;
using Xablu.WebApiClient.Logging;
using Xablu.WebApiClient.Native;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class GraphQLService : IGraphQLService
    {
        private static readonly ILog Logger = LogProvider.For<GraphQLService>();

        private readonly Lazy<GraphQLHttpClient> _background;
        private readonly Lazy<GraphQLHttpClient> _userInitiated;
        private readonly Lazy<GraphQLHttpClient> _speculative;

        public string BaseUrl { get; private set; }

        public GraphQLService(string apiBaseAddress, bool autoRedirectRequests, Func<DelegatingHandler> delegatingHandler, IDictionary<string, string> defaultHeaders)
        {
            if (string.IsNullOrEmpty(apiBaseAddress))
                throw new ArgumentNullException(nameof(apiBaseAddress));

            if (Logger.IsTraceEnabled())
            {
                Logger.Trace($"Base url set to: {apiBaseAddress} and delegatingHandler: {delegatingHandler != null}");
            }

            BaseUrl = apiBaseAddress;

            Func<HttpMessageHandler, GraphQLHttpClient> createClient = messageHandler =>
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

                if (!autoRedirectRequests)
                    DisableAutoRedirects(messageHandler);

                // note: the string parameter is just a placeholder here
                var client = new GraphQLHttpClient(apiBaseAddress);

                client.Options = new GraphQLHttpClientOptions { HttpMessageHandler = handler };

                if (defaultHeaders != default)
                {
                    foreach (var header in defaultHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                return client;
            };

            _background = new Lazy<GraphQLHttpClient>(() => createClient(new RateLimitedHttpMessageHandler(new NativeHttpClientHandler(), Priority.Background)));

            _userInitiated = new Lazy<GraphQLHttpClient>(() => createClient(new RateLimitedHttpMessageHandler(new NativeHttpClientHandler(), Priority.UserInitiated)));

            _speculative = new Lazy<GraphQLHttpClient>(() => createClient(new RateLimitedHttpMessageHandler(new NativeHttpClientHandler(), Priority.Speculative)));
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

        public GraphQLHttpClient GetByPriority(Enums.Priority priority)
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



