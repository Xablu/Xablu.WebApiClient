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
		private IHttpContentResolver _httpContentResolver;
		private IHttpResponseResolver _httpResponseResolver;
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

		    HttpClient CreateClient(HttpMessageHandler messageHandler) => new HttpClient(messageHandler) {BaseAddress = new Uri(apiBaseAddress)};

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
		/// Gets or sets the implementation of the <see cref="IHttpContentResolver"/> interface associated with the WebApiClient.
		/// </summary>
		/// <remarks>
		/// The <see cref="IHttpContentResolver"/> implementation is responsible for serializing content which needs to be send to the server
		/// using a HTTP POST or PUT request.
		/// 
		/// When no other value is supplied the <see cref="RestApiClient"/> by default uses the <see cref="SimpleJsonContentResolver"/>. This resolver will
		/// try to serialize the content to a JSON message and returns the proper <see cref="System.Net.Http.HttpContent"/> instance.
		/// </remarks>
		public virtual IHttpContentResolver HttpContentResolver
		{
			get => _httpContentResolver ?? (_httpContentResolver = new SimpleJsonContentResolver(_serializer));
		    set => _httpContentResolver = value;
		}

		/// <summary>
		/// Gets or sets the implementation of the <see cref="IHttpResponseResolver"/> interface associated with the WebApiClient.
		/// </summary>
		/// <remarks>
		/// The <see cref="IHttpResponseResolver"/> implementation is responsible for deserialising the <see cref="System.Net.Http.HttpResponseMessage"/>
		/// into the required result object.
		/// 
		/// When no other value is supplied the <see cref="RestApiClient"/> by default uses the <see cref="SimpleJsonResponseResolver"/>. This resolver will
		/// assumes the response is a JSON message and tries to deserialize it into the required result object.
		/// </remarks>
		public virtual IHttpResponseResolver HttpResponseResolver
		{
			get => _httpResponseResolver ?? (_httpResponseResolver = new SimpleJsonResponseResolver(_serializer));
		    set => _httpResponseResolver = value;
		}

		/// <summary>
		/// Gets or sets the accept header of the HTTP request. Default the accept header is set to "appliction/json".
		/// </summary>
		public virtual string AcceptHeader { get; set; } = "application/json";

		public virtual IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

		public virtual async Task<IRestApiResult<TResult>> GetAsync<TResult>(Priority priority, string path, CancellationToken cancellationToken = default(CancellationToken))
		{
			var httpClient = GetRestApiClient(priority);

			SetHttpRequestHeaders(httpClient);

			var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);

			var response = await httpClient
				.SendAsync(httpRequest, cancellationToken)
				.ConfigureAwait(false);

            return await response.BuildRestApiResult<TResult>(HttpResponseResolver);
		}

		public virtual async Task<IRestApiResult<TResult>> PatchAsync<TContent, TResult>(Priority priority, string path, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var httpClient = GetRestApiClient(priority);

			SetHttpRequestHeaders(httpClient);

			var httpContent = ResolveHttpContent(content, contentResolver);
			var httpRequestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), path)
			{
				Content = httpContent
			};

			var response = await httpClient
				.SendAsync(httpRequestMessage, cancellationToken)
				.ConfigureAwait(false);
            
			return await response.BuildRestApiResult<TResult>(HttpResponseResolver);
		}

		public virtual async Task<IRestApiResult<TResult>> PostAsync<TContent, TResult>(Priority priority, string path, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var httpClient = GetRestApiClient(priority);

			SetHttpRequestHeaders(httpClient);

			var httpContent = ResolveHttpContent(content, contentResolver);
			var response = await httpClient
				.PostAsync(path, httpContent, cancellationToken)
				.ConfigureAwait(false);

			return await response.BuildRestApiResult<TResult>(HttpResponseResolver);
		}

		public virtual async Task<IRestApiResult<TResult>> PutAsync<TContent, TResult>(Priority priority, string path, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var httpClient = GetRestApiClient(priority);

			SetHttpRequestHeaders(httpClient);

			var httpContent = ResolveHttpContent(content, contentResolver);
			var response = await httpClient
				.PutAsync(path, httpContent, cancellationToken)
				.ConfigureAwait(false);

			return await response.BuildRestApiResult<TResult>(HttpResponseResolver);
		}

		public virtual async Task<IRestApiResult<TResult>> DeleteAsync<TResult>(Priority priority, string path, CancellationToken cancellationToken = default(CancellationToken))
		{
			var httpClient = GetRestApiClient(priority);

			SetHttpRequestHeaders(httpClient);

			var response = await httpClient.DeleteAsync(path, cancellationToken).ConfigureAwait(false);

			return await response.BuildRestApiResult<TResult>(HttpResponseResolver);
		}

		internal HttpContent ResolveHttpContent<TContent>(TContent content, IHttpContentResolver contentResolver = null)
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

		internal HttpClient GetRestApiClient(Priority prioriy)
		{
			if (_apiBaseAddress == null)
				throw new ArgumentNullException(nameof(_apiBaseAddress), "Api base adress is not set. Call SetBaseAddress() to initialize.");

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
