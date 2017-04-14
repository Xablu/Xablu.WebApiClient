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

namespace Xablu.WebApiClient
{
    public class WebApiClient : IWebApiClient
    {
        private string _apiBaseAddress;

        private IHttpContentResolver _httpContentResolver;
        private IHttpResponseResolver _httpResponseResolver;
        private bool _isDisposed;
        private Lazy<HttpClient> _explicit;
        private Lazy<HttpClient> _background;
        private Lazy<HttpClient> _userInitiated;
        private Lazy<HttpClient> _speculative;
        private Func<HttpMessageHandler> httpHandler;

        public WebApiClient(string apiBaseAddress, Func<HttpMessageHandler> handler)
        {
            httpHandler = handler;
            SetBaseAddress(apiBaseAddress);
        }

        public WebApiClient(string apiBaseAddress)
        {
            if (httpHandler == null)
                throw new ArgumentNullException(nameof(httpHandler));

            SetBaseAddress(apiBaseAddress);
        }

        public void SetBaseAddress(string apiBaseAddress)
        {
            if (apiBaseAddress == null)
                throw new ArgumentNullException(nameof(apiBaseAddress));

            if (_apiBaseAddress == apiBaseAddress) return;
            _apiBaseAddress = apiBaseAddress;

            Func<HttpMessageHandler, HttpClient> createClient = messageHandler => new HttpClient(messageHandler) { BaseAddress = new Uri(apiBaseAddress) };

            _explicit = new Lazy<HttpClient>(() => createClient(
                new RateLimitedHttpMessageHandler(httpHandler.Invoke(), Fusillade.Priority.Explicit)));
            
            _background = new Lazy<HttpClient>(() => createClient(
                new RateLimitedHttpMessageHandler(httpHandler.Invoke(), Fusillade.Priority.Background)));

            _userInitiated = new Lazy<HttpClient>(() => createClient(
                new RateLimitedHttpMessageHandler(httpHandler.Invoke(), Fusillade.Priority.UserInitiated)));

            _speculative = new Lazy<HttpClient>(() => createClient(
                new RateLimitedHttpMessageHandler(httpHandler.Invoke(), Fusillade.Priority.Speculative)));
        }

        private JsonSerializer _serializer = new JsonSerializer();

        /// <summary>
        /// Gets or sets the implementation of the <see cref="IHttpContentResolver"/> interface associated with the WebApiClient.
        /// </summary>
        /// <remarks>
        /// The <see cref="IHttpContentResolver"/> implementation is responsible for serializing content which needs to be send to the server
        /// using a HTTP POST or PUT request.
        /// 
        /// When no other value is supplied the <see cref="WebApiClient"/> by default uses the <see cref="SimpleJsonContentResolver"/>. This resolver will
        /// try to serialize the content to a JSON message and returns the proper <see cref="System.Net.Http.HttpContent"/> instance.
        /// </remarks>
        public IHttpContentResolver HttpContentResolver
        {
            get { return _httpContentResolver ?? (_httpContentResolver = new SimpleJsonContentResolver(_serializer)); }
            set { _httpContentResolver = value; }
        }

        /// <summary>
        /// Gets or sets the implementation of the <see cref="IHttpResponseResolver"/> interface associated with the WebApiClient.
        /// </summary>
        /// <remarks>
        /// The <see cref="IHttpResponseResolver"/> implementation is responsible for deserialising the <see cref="System.Net.Http.HttpResponseMessage"/>
        /// into the required result object.
        /// 
        /// When no other value is supplied the <see cref="WebApiClient"/> by default uses the <see cref="SimpleJsonResponseResolver"/>. This resolver will
        /// assumes the response is a JSON message and tries to deserialize it into the required result object.
        /// </remarks>
        public IHttpResponseResolver HttpResponseResolver
        {
            get { return _httpResponseResolver ?? (_httpResponseResolver = new SimpleJsonResponseResolver(_serializer)); }
            set { _httpResponseResolver = value; }
        }

        /// <summary>
        /// Gets or sets the accept header of the HTTP request. Default the accept header is set to "appliction/json".
        /// </summary>
        public string AcceptHeader { get; set; } = "application/json";

        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public async Task<TResult> GetAsync<TResult>(Priority priority, string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpClient = GetWebApiClient(priority);

            SetHttpRequestHeaders(httpClient);

            //TODO: httpClient.GetAsync may throw NetworkOnMainThreadException
            // see: https://bugzilla.xamarin.com/show_bug.cgi?id=44961
            //var response = await httpClient.GetAsync(path, cancellationToken).ConfigureAwait(false);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);

            var response = await httpClient
                .SendAsync(httpRequest, cancellationToken)
                .ConfigureAwait(false);

            if(!await response.EnsureSuccessStatusCodeAsync())
                return default(TResult);

            return await HttpResponseResolver.ResolveHttpResponseAsync<TResult>(response);
        }

        public async Task<HttpResponseMessage> GetAsync(Priority priority, string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpClient = GetWebApiClient(priority);

            SetHttpRequestHeaders(httpClient);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);

            var response = await httpClient
                .SendAsync(httpRequest, cancellationToken)
                .ConfigureAwait(false);

            if (!await response.EnsureSuccessStatusCodeAsync())
                return default(HttpResponseMessage);

            return response;
        }

        public async Task<TResult> PostAsync<TContent, TResult>(Priority priority, string path, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpClient = GetWebApiClient(priority);

            SetHttpRequestHeaders(httpClient);

            var httpContent = ResolveHttpContent(content, contentResolver);
            var response = await httpClient
                .PostAsync(path, httpContent, cancellationToken)
                .ConfigureAwait(false);

            if (!await response.EnsureSuccessStatusCodeAsync())
                return default(TResult);

            return await HttpResponseResolver.ResolveHttpResponseAsync<TResult>(response);
        }

        public async Task<TResult> PutAsync<TContent, TResult>(Priority priority, string path, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpClient = GetWebApiClient(priority);

            SetHttpRequestHeaders(httpClient);

            var httpContent = ResolveHttpContent(content, contentResolver);
            var response = await httpClient
                .PutAsync(path, httpContent, cancellationToken)
                .ConfigureAwait(false);

            if (!await response.EnsureSuccessStatusCodeAsync())
                return default(TResult);

            return await HttpResponseResolver.ResolveHttpResponseAsync<TResult>(response);
        }

        public async Task<TResult> DeleteAsync<TResult>(Priority priority, string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpClient = GetWebApiClient(priority);

            SetHttpRequestHeaders(httpClient);

            var response = await httpClient.DeleteAsync(path, cancellationToken).ConfigureAwait(false);

            if (!await response.EnsureSuccessStatusCodeAsync())
                return default(TResult);

            return await HttpResponseResolver.ResolveHttpResponseAsync<TResult>(response);
        }

        internal HttpContent ResolveHttpContent<TContent>(TContent content, IHttpContentResolver contentResolver = null)
        {
        	HttpContent httpContent = null;

        	if (content != null)
        	{
        		if (content is HttpContent)
        		{
        			httpContent = content as HttpContent;
        		}
        		else
        		{
        			if (contentResolver != null)
        			{
        				httpContent = contentResolver.ResolveHttpContent(content);
        			}
        			else
        			{
                        var contentAsDictionary = content as Dictionary<string, string>;

                        httpContent = contentAsDictionary != null
                            ? new DictionaryContentResolver().ResolveHttpContent(content as Dictionary<string, string>)
                            : HttpContentResolver.ResolveHttpContent(content);
        			}
        		}
        	}
            return httpContent;
        }

        internal HttpClient GetWebApiClient(Priority prioriy)
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

        internal void SetHttpRequestHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptHeader));

            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            foreach (var header in Headers)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
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
