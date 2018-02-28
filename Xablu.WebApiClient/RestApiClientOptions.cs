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

        /// <summary>
        /// Gets the base address of the API to which the <see cref="RestApiClient"/> will send the HTTP requests.
        /// </summary>
        /// <remarks>
        /// The base address must only contain the HTTP-scheme (http or https) and the domain / ip-address to connect to. 
        /// An example could be "https://www.xablu.com". The base address will be appended in front of the 'path' value which
        /// is supplied with every HTTP request. 
        /// </remarks>
        public string ApiBaseAddress { get; set; }

        /// <summary>
        /// Gets or sets a delegate which will instantiate an instance of <see cref="HttpMessageHandler"/> class used to process the HTTP requests.
        /// </summary>
        /// <remarks>
        /// The <see cref="RestApiClient"/> will (lazily) create an instance of the <see cref="HttpClient"/> object for each of the different priorities.
        /// The delegate supplied to the <see cref="DefaultHttpMessageHandler"/> property is called to supply the <see cref="HttpClient"/> with an implementation
        /// of the <see cref="HttpMessageHandler"/> class. 
        /// 
        /// You can supply a different delegate to create a platform specific implementation of the <see cref="HttpMessageHander"/> class. For example you might want 
        /// to supply one of the following:
        /// 
        /// <code>
        /// // To use the NSUrlSessionHandler (on iOS devices) use:
        /// () => new NSUrlSessionHandler();
        /// 
        /// // To use the AndroidClientHandler (on Android devices) use:
        /// () => new AndroidClientHandler();
        /// </code> 
        /// 
        /// By default the delegate will create an instance of the <see cref="HttpClientHandler"/> (standard .NET implementation).   
        /// </remarks>
        public Func<HttpMessageHandler> DefaultHttpMessageHandler { get; set; } = () => new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.GZip };

        /// <summary>
        /// Gets or sets a list of default headers which will be used with every HTTP request made by the <see cref="RestApiClient"/>.
        /// </summary>
        /// <remarks>
        /// The headers specified in this list will be added to every HTTP request made by the <see cref="RestApiClient"/>. The following headers
        /// are already pre-configured:
        /// <list type="table">  
        ///    <listheader>  
        ///        <term>Key</term>  
        ///        <description>Value</description>  
        ///    </listheader>  
        ///    <item>  
        ///        <term>Accept</term>  
        ///        <description>application/json</description>  
        ///    </item>  
        ///    <item>  
        ///        <term>Accept-Encoding</term>  
        ///        <description>gzip</description>  
        ///    </item>  
        /// </list>  
        /// </remarks>
        public IList<KeyValuePair<string, string>> DefaultHeaders { get; set; } = new List<KeyValuePair<string, string>> { { "Accept", "application/json" }, { "Accept-Encoding", "gzip" } };

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
