using System;
using System.Collections.Generic;
using System.Net.Http;
using Fusillade;
using GraphQL.Client.Http;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class GraphQLService : IGraphQLService
    {
        private readonly Lazy<GraphQLHttpClient> _background;
        private readonly Lazy<GraphQLHttpClient> _userInitiated;
        private readonly Lazy<GraphQLHttpClient> _speculative;

        public string BaseUrl { get; private set; }

        public GraphQLService(string apiBaseAddress, bool autoRedirectRequests, Func<DelegatingHandler> delegatingHandler, IDictionary<string, string> defaultHeaders)
        {
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

            _background = new Lazy<GraphQLHttpClient>(() => createClient(NetCache.Background));

            _userInitiated = new Lazy<GraphQLHttpClient>(() => createClient(NetCache.UserInitiated));

            _speculative = new Lazy<GraphQLHttpClient>(() => createClient(NetCache.Speculative));
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



