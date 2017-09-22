using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Fusillade;
using Xablu.WebApiClient.Resolvers;
using Xablu.WebApiClient.HttpExtensions;
using Newtonsoft.Json;
using System.Net;

namespace Xablu.WebApiClient
{
    public class RestApiClient
        : IRestApiClient
    {
        private readonly Func<HttpMessageHandler> _httpHandler;
        private readonly JsonSerializer _serializer = new JsonSerializer();

        private string _apiBaseAddress;
        private IHttpContentResolver _defaultHttpContentResolver;
        private IHttpResponseResolver _defaultHttpResponseResolver;
        private bool _isDisposed;
        private Lazy<HttpClient> _explicit;
        private Lazy<HttpClient> _background;
        private Lazy<HttpClient> _userInitiated;
        private Lazy<HttpClient> _speculative;

        public RestApiClient()
        {
            _httpHandler = () => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
        }

        public RestApiClient(string apiBaseAddress)
            : this()
        {
            SetBaseAddress(apiBaseAddress);
        }

        public RestApiClient(Func<HttpMessageHandler> handler)
        {
            _httpHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public RestApiClient(string apiBaseAddress, Func<HttpMessageHandler> handler)
            : this(handler)
        {
            SetBaseAddress(apiBaseAddress);
        }

        public void SetBaseAddress(string apiBaseAddress)
        {
            if (apiBaseAddress == null)
                throw new ArgumentNullException(nameof(apiBaseAddress));

            if (_apiBaseAddress == apiBaseAddress) return;
            _apiBaseAddress = apiBaseAddress;

            HttpClient CreateClient(HttpMessageHandler messageHandler) => new HttpClient(messageHandler)
            {
                BaseAddress = new Uri(apiBaseAddress)
            };

            _explicit = new Lazy<HttpClient>(() => CreateClient(
                new RateLimitedHttpMessageHandler(_httpHandler.Invoke(), Priority.Explicit)));

            _background = new Lazy<HttpClient>(() => CreateClient(
                new RateLimitedHttpMessageHandler(_httpHandler.Invoke(), Priority.Background)));

            _userInitiated = new Lazy<HttpClient>(() => CreateClient(
                new RateLimitedHttpMessageHandler(_httpHandler.Invoke(), Priority.UserInitiated)));

            _speculative = new Lazy<HttpClient>(() => CreateClient(
                new RateLimitedHttpMessageHandler(_httpHandler.Invoke(), Priority.Speculative)));
        }

        /// <summary>
        /// Gets or sets the default implementation of the <see cref="IHttpContentResolver"/> interface associated with the WebApiClient.
        /// </summary>
        /// <remarks>
        /// The <see cref="IHttpContentResolver"/> implementation is responsible for serializing content which needs to be send to the server
        /// using a HTTP POST, PUT or PATCH request.
        /// 
        /// This property can be used to override the default <see cref="IHttpContentResolver"/> that is used by the <see cref="RestApiClient"/>.
        /// If no other value is supplied the <see cref="RestApiClient"/> by default uses the <see cref="SimpleJsonContentResolver"/>. This resolver will
        /// try to serialize the content to a JSON message and returns the proper <see cref="System.Net.Http.HttpContent"/> instance.
        /// 
        /// NOTE: this property is not thread safe! You should only set this property when initializing the <see cref="RestApiClient"/>. If
        /// you want to override the <see cref="IHttpContentResolver"/> for a particular request you should supply the appropiate <see cref="IHttpContentResolver"/>
        /// with one of the <see cref="GetAsync"/>, <see cref="PostAsync"/>, <see cref="PutAsync"/>, <see cref="PatchAsync"/> or <see cref="DeleteAsync"/> methods.  
        /// </remarks>
        protected virtual IHttpContentResolver DefaultHttpContentResolver
        {
            get => _defaultHttpContentResolver ?? (_defaultHttpContentResolver = new SimpleJsonContentResolver(_serializer));
            set => _defaultHttpContentResolver = value;
        }

        /// <summary>
        /// Gets or sets the default implementation of the <see cref="IHttpResponseResolver"/> interface associated with the RestApiClient.
        /// </summary>
        /// <remarks>
        /// The <see cref="IHttpResponseResolver"/> implementation is responsible for deserialising the <see cref="System.Net.Http.HttpResponseMessage"/>
        /// into the required result object.
        /// 
        /// This property can be used to override the default <see cref="IHttpResponseResolver"/> that is used by the <see cref="RestApiClient"/>.
        /// If no other value is supplied the <see cref="RestApiClient"/> by default uses the <see cref="SimpleJsonResponseResolver"/>. This resolver will
        /// assumes the response is a JSON message and tries to deserialize it into the required result object.
        /// 
        /// NOTE: this property is not thread safe! You should only set this property when initializing the <see cref="RestApiClient"/>. If
        /// you want to override the <see cref="IHttpResponseResolver"/> for a particular request you should supply the appropiate <see cref="IHttpContentResolver"/>
        /// with one of the <see cref="GetAsync"/>, <see cref="PostAsync"/>, <see cref="PutAsync"/>, <see cref="PatchAsync"/> or <see cref="DeleteAsync"/> methods.  
        /// </remarks>
        protected virtual IHttpResponseResolver DefaultHttpResponseResolver
        {
            get => _defaultHttpResponseResolver ?? (_defaultHttpResponseResolver = new SimpleJsonResponseResolver(_serializer));
            set => _defaultHttpResponseResolver = value;
        }

        public virtual string DefaultAcceptHeader => "application/json";
        public virtual string AuthorizeToken { get; set; }

        public virtual async Task<IRestApiResult<TResult>> GetAsync<TResult>(
            Priority priority,
            string path,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, path);

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken);
        }

        public virtual async Task<IRestApiResult<TResult>> PatchAsync<TContent, TResult>(
            Priority priority,
            string path,
            TContent content = default(TContent),
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver httpContentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = ResolveHttpContent(content, httpContentResolver);
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), path)
            {
                Content = httpContent
            };

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken);
        }

        public virtual async Task<IRestApiResult<TResult>> PostAsync<TContent, TResult>(
            Priority priority,
            string path,
            TContent content = default(TContent),
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver httpContentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = ResolveHttpContent(content, httpContentResolver);
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod("POST"), path)
            {
                Content = httpContent
            };

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken);
        }

        public virtual async Task<IRestApiResult<TResult>> PutAsync<TContent, TResult>(
            Priority priority,
            string path,
            TContent content = default(TContent),
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver httpContentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = ResolveHttpContent(content, httpContentResolver);
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod("PUT"), path)
            {
                Content = httpContent
            };

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken);
        }

        public virtual async Task<IRestApiResult<TResult>> DeleteAsync<TResult>(
            Priority priority,
            string path,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod("DELETE"), path);

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken);
        }

        protected virtual async Task<IRestApiResult<TResult>> SendAsync<TResult>(
            Priority priority,
            HttpRequestMessage httpRequestMessage,
            IList<KeyValuePair<string, string>> headers,
            IHttpResponseResolver httpResponseResolver,
            CancellationToken cancellationToken)
        {
            var httpClient = GetRestApiClient(priority);

            SetHttpRequestHeaders(httpRequestMessage, headers);

            var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);

            if (httpResponseResolver == null)
                httpResponseResolver = DefaultHttpResponseResolver;

            return await response.BuildRestApiResult<TResult>(httpResponseResolver);
        }

        protected virtual HttpContent ResolveHttpContent<TContent>(
            TContent content,
            IHttpContentResolver httpContentResolver = null)
        {
            HttpContent httpContent = null;

            if (!EqualityComparer<TContent>.Default.Equals(content, default(TContent)))
            {
                if (content is HttpContent)
                {
                    httpContent = content as HttpContent;
                }
                else
                {
                    if (httpContentResolver != null)
                    {
                        httpContent = httpContentResolver.ResolveHttpContent(content);
                    }
                    else
                    {
                        var contentAsDictionary = content as Dictionary<string, string>;

                        httpContent = contentAsDictionary != null
                            ? new DictionaryContentResolver().ResolveHttpContent(content as Dictionary<string, string>)
                            : DefaultHttpContentResolver.ResolveHttpContent(content);
                    }
                }
            }

            return httpContent;
        }

        public virtual HttpClient GetRestApiClient(Priority prioriy)
        {
            if (_apiBaseAddress == null)
                throw new ArgumentNullException(nameof(_apiBaseAddress),
                    "Api base adress is not set. Call SetBaseAddress() to initialize.");

            switch (prioriy)
            {
                case Priority.UserInitiated:
                    return _userInitiated.Value;
                case Priority.Speculative:
                    return _speculative.Value;
                case Priority.Background:
                    return _background.Value;
                case Priority.Explicit:
                    return _explicit.Value;
                default:
                    return _background.Value;
            }
        }

        protected virtual void SetHttpRequestHeaders(HttpRequestMessage message, IList<KeyValuePair<string, string>> headers)
        {
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(DefaultAcceptHeader));
            message.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            if (!string.IsNullOrEmpty(AuthorizeToken))
                message.Headers.Add("Authorize", $"Bearer {AuthorizeToken}");

            if (headers == null) return;

            foreach (var header in headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                _background.Value?.Dispose();
                _explicit.Value?.Dispose();
                _speculative.Value?.Dispose();
                _userInitiated.Value?.Dispose();
            }

            _background = null;
            _explicit = null;
            _speculative = null;
            _userInitiated = null;

            _isDisposed = true;
        }
    }
}