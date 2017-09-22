using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Xablu.WebApiClient.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace Xablu.WebApiClient.UnitTests
{
    public class RestApiClientTests
    {
        private class RestApiClientAccessor : RestApiClient
        {
            public void SetHttpRequestHeadersAccessor(HttpRequestMessage message, IList<KeyValuePair<string, string>> headers)
            {
                base.SetHttpRequestHeaders(message, headers);
            }
        }

        [Fact]
        public void SetHttpRequestHeaders_DefaultAcceptHeader_ApplicationJson()
        {
            var restClient = new RestApiClientAccessor();
            var httpRequestMessage = new HttpRequestMessage();

            restClient.SetHttpRequestHeadersAccessor(httpRequestMessage, null);

            Assert.Equal(1, httpRequestMessage.Headers.Accept.Count);
            Assert.True(httpRequestMessage.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")));
        }

        [Fact]
        public void SetHttpRequestHeaders_OverrideAcceptHeader_AcceptHeaderMatchedWithTheValueSet()
        {
            const string acceptHeader = "plain/text";
            var restClient = new RestApiClientAccessor { DefaultAcceptHeader = acceptHeader };
            var httpRequestMessage = new HttpRequestMessage();

            restClient.SetHttpRequestHeadersAccessor(httpRequestMessage, null);

            Assert.Equal(1, httpRequestMessage.Headers.Accept.Count);
            Assert.True(httpRequestMessage.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue(acceptHeader)));
        }

        [Fact]
        public void SetHttpRequestHeaders_MultipleAcceptHeaders_AcceptHeaderMatchedWithTheValuesSet()
        {
            const string acceptHeader = "plain/text";
            var restClient = new RestApiClientAccessor();
            var headers = new List<KeyValuePair<string, string>> { { "Accept", acceptHeader } };
            var httpRequestMessage = new HttpRequestMessage();

            restClient.SetHttpRequestHeadersAccessor(httpRequestMessage, headers);

            Assert.Equal(2, httpRequestMessage.Headers.Accept.Count);
            Assert.True(httpRequestMessage.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")));
            Assert.True(httpRequestMessage.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue("plain/text")));
        }

        [Fact]
        public void SetHttpRequestHeaders_DefaultAcceptEncoding_Gzip()
        {
            var restClient = new RestApiClientAccessor();
            var httpRequestMessage = new HttpRequestMessage();

            restClient.SetHttpRequestHeadersAccessor(httpRequestMessage, null);

            Assert.Equal(1, httpRequestMessage.Headers.AcceptEncoding.Count);
            Assert.True(httpRequestMessage.Headers.AcceptEncoding.Contains(new StringWithQualityHeaderValue("gzip")));
        }

        [Fact]
        public void SetHttpRequestHeaders_CustomHeadersConfigure_CustomHeadersAreAddedToHttpClient()
        {
            var restClient = new RestApiClientAccessor();
            var headers = new List<KeyValuePair<string, string>> { { "X-CustomHeader-1", "Value 1" }, { "X-CustomHeader-2", "Value 2" } };
            var httpRequestMessage = new HttpRequestMessage();

            restClient.SetHttpRequestHeadersAccessor(httpRequestMessage, headers);

            Assert.True(httpRequestMessage.Headers.Contains("X-CustomHeader-1"));
            Assert.True(httpRequestMessage.Headers.Contains("X-CustomHeader-2"));
        }
    }
}
