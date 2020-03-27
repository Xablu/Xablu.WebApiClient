using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fusillade;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Xablu.WebApiClient.Native;

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
            if (string.IsNullOrEmpty(apiBaseAddress))
                throw new ArgumentNullException(nameof(apiBaseAddress));

            Debug.WriteLine($"GraphQL base url set to: {apiBaseAddress}, autoRedirects: {autoRedirectRequests} and delegatingHandler: {delegatingHandler != null}");
            
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

                var client = new GraphQLHttpClient(new GraphQLHttpClientOptions
                {
                    HttpMessageHandler = handler,
                    EndPoint = new Uri(apiBaseAddress)
                }, new NewtonsoftJsonSerializer());

                if (defaultHeaders != default)
                {
                    foreach (var header in defaultHeaders)
                    {
                        client.HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
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
                Debug.WriteLine("Disabling AutoRedirects");

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



    //public class Nico : IGraphQLWebsocketJsonSerializer
    //{
    //    public static JsonSerializerSettings DefaultJsonSerializerSettings => new JsonSerializerSettings
    //    {
    //        ContractResolver = new CamelCasePropertyNamesContractResolver { IgnoreIsSpecifiedMembers = true },
    //        MissingMemberHandling = MissingMemberHandling.Ignore,
    //    };

    //    public JsonSerializerSettings JsonSerializerSettings { get; }

    //    public Nico() : this(DefaultJsonSerializerSettings) { }

    //    public Nico(Action<JsonSerializerSettings> configure) : this(configure.AndReturn(DefaultJsonSerializerSettings)) { }

    //    public Nico(JsonSerializerSettings jsonSerializerSettings)
    //    {
    //        JsonSerializerSettings = jsonSerializerSettings;
    //        ConfigureMandatorySerializerOptions();
    //    }

    //    // deserialize extensions to Dictionary<string, object>
    //    private void ConfigureMandatorySerializerOptions() => JsonSerializerSettings.Converters.Insert(0, new MapConverter());

    //    public string SerializeToString(GraphQLRequest request) => JsonConvert.SerializeObject(request, JsonSerializerSettings);

    //    public byte[] SerializeToBytes(GraphQLWebSocketRequest request)
    //    {
    //        var json = JsonConvert.SerializeObject(request, JsonSerializerSettings);
    //        return Encoding.UTF8.GetBytes(json);
    //    }

    //    public Task<WebsocketMessageWrapper> DeserializeToWebsocketResponseWrapperAsync(Stream stream) => DeserializeFromUtf8Stream<WebsocketMessageWrapper>(stream);

    //    public GraphQLWebSocketResponse<GraphQLResponse<TResponse>> DeserializeToWebsocketResponse<TResponse>(byte[] bytes) =>
    //        JsonConvert.DeserializeObject<GraphQLWebSocketResponse<GraphQLResponse<TResponse>>>(Encoding.UTF8.GetString(bytes),
    //            JsonSerializerSettings);

    //    public Task<GraphQLResponse<TResponse>> DeserializeFromUtf8StreamAsync<TResponse>(Stream stream, CancellationToken cancellationToken) => DeserializeFromUtf8Stream<GraphQLResponse<TResponse>>(stream);


    //    private Task<T> DeserializeFromUtf8Stream<T>(Stream stream)
    //    {
    //        Stream toTest = new MemoryStream();
    //        stream.CopyTo(toTest);
    //        var asd = StreamToString(toTest);
            
    //        var dess = JsonConvert.DeserializeObject<T>(asd);



    //        using var sr = new StreamReader(stream);


    //        using JsonReader reader = new JsonTextReader(sr);
    //        var serializer = JsonSerializer.Create(JsonSerializerSettings);

    //        var des = serializer.Deserialize<T>(reader);
    //        return Task.FromResult(des);
    //    }

    //    public string StreamToString(Stream stream)
    //    {
    //        stream.Position = 0;
    //        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
    //        {
    //            return reader.ReadToEnd();
    //        }
    //    }

    //}

    //public class MapConverter : JsonConverter<Map>
    //{
    //    public override void WriteJson(JsonWriter writer, Map value, JsonSerializer serializer) =>
    //        throw new NotImplementedException(
    //            "This converter currently is only intended to be used to read a JSON object into a strongly-typed representation.");

    //    public override Map ReadJson(JsonReader reader, Type objectType, Map existingValue,
    //        bool hasExistingValue, JsonSerializer serializer)
    //    {
    //        var rootToken = JToken.ReadFrom(reader);
    //        if (rootToken is JObject)
    //        {
    //            return ReadDictionary<Map>(rootToken);
    //        }
    //        else
    //            throw new ArgumentException("This converter can only parse when the root element is a JSON Object.");
    //    }

    //    private object ReadToken(JToken? token) =>
    //        token switch
    //        {
    //            JObject jObject => ReadDictionary<Dictionary<string, object>>(jObject),
    //            JArray jArray => ReadArray(jArray),
    //            JValue jValue => jValue.Value,
    //            JConstructor _ => throw new ArgumentOutOfRangeException(nameof(token.Type),
    //                "cannot deserialize a JSON constructor"),
    //            JProperty _ => throw new ArgumentOutOfRangeException(nameof(token.Type),
    //                "cannot deserialize a JSON property"),
    //            JContainer _ => throw new ArgumentOutOfRangeException(nameof(token.Type),
    //                "cannot deserialize a JSON comment"),
    //            _ => throw new ArgumentOutOfRangeException(nameof(token.Type))
    //        };

    //    private TDictionary ReadDictionary<TDictionary>(JToken element) where TDictionary : Dictionary<string, object>
    //    {
    //        var result = Activator.CreateInstance<TDictionary>();
    //        foreach (var property in ((JObject)element).Properties())
    //        {
    //            if (IsUnsupportedJTokenType(property.Value.Type))
    //                continue;
    //            result[property.Name] = ReadToken(property.Value);
    //        }
    //        return result;
    //    }

    //    private IEnumerable<object> ReadArray(JToken element)
    //    {
    //        foreach (var item in element.Values())
    //        {
    //            if (IsUnsupportedJTokenType(item.Type))
    //                continue;
    //            yield return ReadToken(item);
    //        }
    //    }

    //    private bool IsUnsupportedJTokenType(JTokenType type) => type == JTokenType.Constructor || type == JTokenType.Property || type == JTokenType.Comment;
    //}
}




