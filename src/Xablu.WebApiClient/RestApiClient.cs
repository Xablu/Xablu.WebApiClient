using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fusillade;
using Xablu.WebApiClient.Resolvers;
using Xablu.WebApiClient.HttpExtensions;

namespace Xablu.WebApiClient
{
    public abstract class RestApiClient
        : IRestApiClient
    {
        private readonly RestApiClientOptions _restApiClientOptions;

        private bool _isDisposed;
        private Lazy<HttpClient> _explicit;
        private Lazy<HttpClient> _background;
        private Lazy<HttpClient> _userInitiated;
        private Lazy<HttpClient> _speculative;

        public RestApiClient(string apiBaseAddress)
        {
            _restApiClientOptions = new RestApiClientOptions();

            Initialize();
        }

        public RestApiClient(RestApiClientOptions options)
        {
            _restApiClientOptions = options ?? throw new ArgumentNullException(nameof(options));

            Initialize();
        }

        private void Initialize()
        {
            var apiBaseAddress = _restApiClientOptions.ApiBaseAddress;
            var httpHandler = HttpMessageHandlerBuilder;

            if (string.IsNullOrEmpty(apiBaseAddress))
                throw new InvalidOperationException("The 'RestApiClient' failed to initialize. Make sure you set a value for the 'ApiBaseAddress' option.");

            HttpClient CreateClient(HttpMessageHandler messageHandler) => new HttpClient(messageHandler)
            {
                BaseAddress = new Uri(apiBaseAddress)
            };

            _explicit = new Lazy<HttpClient>(() => CreateClient(
                new RateLimitedHttpMessageHandler(httpHandler.Invoke(), Priority.Explicit)));

            _background = new Lazy<HttpClient>(() => CreateClient(
                new RateLimitedHttpMessageHandler(httpHandler.Invoke(), Priority.Background)));

            _userInitiated = new Lazy<HttpClient>(() => CreateClient(
                new RateLimitedHttpMessageHandler(httpHandler.Invoke(), Priority.UserInitiated)));

            _speculative = new Lazy<HttpClient>(() => CreateClient(
                new RateLimitedHttpMessageHandler(httpHandler.Invoke(), Priority.Speculative)));
        }

        /// <summary>
        /// Gets or sets a delegate which will instantiate an instance of <see cref="HttpMessageHandler"/> class used to process the HTTP requests.
        /// </summary>
        /// <remarks>
        /// The <see cref="RestApiClient"/> will (lazily) create an instance of the <see cref="HttpClient"/> object for each of the different priorities.
        /// The delegate supplied to the <see cref="HttpMessageHandlerBuilder"/> property is called to supply the <see cref="HttpClient"/> with an implementation
        /// of the <see cref="HttpMessageHandler"/> class. 
        /// 
        /// You can supply a different delegate to create a platform specific implementation of the <see cref="HttpMessageHander"/> class by overriding the property
        /// and returning your <see cref="HttpMessageHander"/> implementation.
        ///
        /// By default the delegate will create an instance of the <see cref="HttpClientHandler"/> (standard .NET implementation).   
        /// </remarks>
        protected virtual Func<HttpMessageHandler> HttpMessageHandlerBuilder => () => new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.GZip };

        public virtual string AuthorizeToken { get; set; }

        public virtual async Task<IRestApiResult<TResult>> GetAsync<TResult>(
            Priority priority,
            string path,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, path);

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken)
                .ConfigureAwait(false);
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

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken)
                .ConfigureAwait(false);
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

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken)
                .ConfigureAwait(false);
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

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual async Task<IRestApiResult<TResult>> DeleteAsync<TResult>(
            Priority priority,
            string path,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod("DELETE"), path);

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken)
                .ConfigureAwait(false);
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

            var response = await httpClient
                .SendAsync(httpRequestMessage, cancellationToken)
                .ConfigureAwait(false);

            if (httpResponseResolver == null)
                httpResponseResolver = _restApiClientOptions.DefaultResponseResolver;

            return await response
                .BuildRestApiResult<TResult>(httpResponseResolver)
                .ConfigureAwait(false);
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
                            : _restApiClientOptions.DefaultContentResolver.ResolveHttpContent(content);
                    }
                }
            }

            return httpContent;
        }

        public virtual HttpClient GetRestApiClient(Priority prioriy)
        {
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
            if (!string.IsNullOrEmpty(AuthorizeToken))
                message.Headers.Add("Authorization", $"Bearer {AuthorizeToken}");

            if (_restApiClientOptions.DefaultHeaders != null)
            {
                foreach (var defaultHeader in _restApiClientOptions.DefaultHeaders)
                {
                    message.Headers.Add(defaultHeader.Key, defaultHeader.Value);
                }
            }

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