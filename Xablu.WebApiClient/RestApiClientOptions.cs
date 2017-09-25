using Newtonsoft.Json;
using System.Collections.Generic;
using Xablu.WebApiClient.Resolvers;
using System.Net.Http;
using System;

namespace Xablu.WebApiClient
{
    public class RestApiClientOptions
    {
        public RestApiClientOptions(string apiBaseAddress)
        {
            ApiBaseAddress = apiBaseAddress;
        }

        public string ApiBaseAddress { get; }
        public Func<HttpMessageHandler> DefaultHttpMessageHandler { get; set; } = () => new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.GZip };
        public IList<KeyValuePair<string, string>> DefaultHeaders { get; set; } = new List<KeyValuePair<string, string>> { { "Accept", "application/json" }, { "Accept-Enconding", "gzip" } };

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
        public IHttpContentResolver DefaultContentResolver { get; set; } = new SimpleJsonContentResolver(new JsonSerializer());

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
        public IHttpResponseResolver DefaultResponseResolver { get; set; } = new SimpleJsonResponseResolver(new JsonSerializer());
    }
}
