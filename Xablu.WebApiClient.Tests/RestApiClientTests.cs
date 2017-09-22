using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Fusillade;
using Xablu.WebApiClient.Resolvers;
using Xablu.WebApiClient.UnitTests.Builders;
using Xablu.WebApiClient.UnitTests.Fakes;
using Xunit;
using System.Linq;

namespace Xablu.WebApiClient.UnitTests
{
    public class RestApiClientTests
    {
        [Fact]
        public void SetHttpRequestHeaders_DefaultAcceptHeader_ApplicationJson()
        {
            var httpClient = new HttpClient();
            var restClient = new RestApiClient(() => new FakeMessageHandler());
            
            restClient.SetHttpRequestHeaders(httpClient);

            Assert.Equal(1, httpClient.DefaultRequestHeaders.Accept.Count);
            Assert.True(httpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")));
        }

        [Fact]
        public void SetHttpRequestHeaders_OverrideAcceptHeader_AcceptHeaderMatchedWithTheValueSet()
        {
            const string acceptHeader = "plain/text";
            var httpClient = new HttpClient();
            var restClient = new RestApiClient(() => new FakeMessageHandler()) { AcceptHeader =  acceptHeader };
            
            restClient.SetHttpRequestHeaders(httpClient);

            Assert.Equal(1, httpClient.DefaultRequestHeaders.Accept.Count);
            Assert.True(httpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue(acceptHeader)));
        }

        [Fact]
        public void SetHttpRequestHeaders_MultipleAcceptHeaders_AcceptHeaderMatchedWithTheValuesSet()
        {
            var restClient = new RestApiClient(() => new FakeMessageHandler());
            restClient.Headers.Add("Accept", "plain/text");
            var httpClient = new HttpClient();

            restClient.SetHttpRequestHeaders(httpClient);

            Assert.Equal(2, httpClient.DefaultRequestHeaders.Accept.Count);
            Assert.True(httpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")));
            Assert.True(httpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue("plain/text")));
        }

        [Fact]
        public void SetHttpRequestHeaders_DefaultAcceptEncoding_Gzip()
        {
            var httpClient = new HttpClient();
            var restClient = new RestApiClient(() => new FakeMessageHandler());
            
            restClient.SetHttpRequestHeaders(httpClient);

            Assert.Equal(1, httpClient.DefaultRequestHeaders.AcceptEncoding.Count);
            Assert.True(httpClient.DefaultRequestHeaders.AcceptEncoding.Contains(new StringWithQualityHeaderValue("gzip")));
        }

        [Fact]
        public void SetHttpRequestHeaders_CustomHeadersConfigure_CustomHeadersAreAddedToHttpClient()
        {
            var httpClient = new HttpClient();
            var restClient = new RestApiClient(() => new FakeMessageHandler());
            restClient.Headers.Add("X-CustomHeader-1", "Value 1");
            restClient.Headers.Add("X-CustomHeader-2", "Value 2");

            restClient.SetHttpRequestHeaders(httpClient);

            Assert.True(httpClient.DefaultRequestHeaders.Contains("X-CustomHeader-1"));
            Assert.True(httpClient.DefaultRequestHeaders.Contains("X-CustomHeader-2"));
        }

        [Fact]
        public void SetHttpRequestHeaders_HeadersAlreadySetAndCleared_OnlyDefaultHeadersAreSet()
        {
            var httpClient = new HttpClient();
            var restClient = new RestApiClient(() => new FakeMessageHandler());
            restClient.Headers.Add("X-CustomHeader-1", "Value 1");
            restClient.Headers.Add("X-CustomHeader-2", "Value 2");

            restClient.SetHttpRequestHeaders(httpClient);
            restClient.Headers.Clear();
            restClient.SetHttpRequestHeaders(httpClient);

            // Note that the default Accept and AcceptEncoding headers always set
            Assert.Equal(2, httpClient.DefaultRequestHeaders.Count());
            Assert.True(httpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")));
            Assert.True(httpClient.DefaultRequestHeaders.AcceptEncoding.Contains(new StringWithQualityHeaderValue("gzip")));
        }
    }
}
