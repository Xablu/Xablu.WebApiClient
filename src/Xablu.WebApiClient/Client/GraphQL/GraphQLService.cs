using System;
using System.Collections.Generic;
using System.Net.Http;
using GraphQL.Client.Http;
using GraphQL.Common.Request;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class GraphQLService
    {
        private GraphQLHttpClientOptions _graphQLClientOptions;
        public GraphQLService(string baseUrl = "", Func<DelegatingHandler> delegatingHandler = null, Dictionary<string, string> headers = null)
        {
            if (delegatingHandler != null)
            {
                _graphQLClientOptions = new GraphQLHttpClientOptions
                {
                    HttpMessageHandler = delegatingHandler?.Invoke()
                };
            }

            InitClient(baseUrl, headers);
        }

        private GraphQLHttpClient Client { get; set; }

        private void InitClient(string baseUrl, Dictionary<string, string> headers = null)
        {
            Client = new GraphQLHttpClient(baseUrl);

            if (_graphQLClientOptions != null)
            {
                Client.Options = _graphQLClientOptions;
            }

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }

        public void SendQueryAsync(string String)
        {
            var test = new GraphQLRequest();

        }
    }
}
