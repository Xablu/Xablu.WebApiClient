using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Common.Request;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class GraphQLService
    {
        private GraphQLHttpClientOptions _graphQLClientOptions;
        private Dictionary<string, string> _headers;

        public GraphQLService(string baseUrl = "", Func<DelegatingHandler> delegatingHandler = null, Dictionary<string, string> headers = null)
        {
            if (delegatingHandler != null)
            {
                _graphQLClientOptions = new GraphQLHttpClientOptions
                {
                    HttpMessageHandler = delegatingHandler?.Invoke()
                };
            }
            BaseURL = baseUrl;
            Headers = headers;

        }

        public GraphQLHttpClient Client { get; set; }

        public string BaseURL { get; set; }

        public Dictionary<string, string> Headers
        {
            get { return _headers; }
            set
            {
                _headers = value;
                InitClient(BaseURL, value);
            }
        }


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
    }
}



