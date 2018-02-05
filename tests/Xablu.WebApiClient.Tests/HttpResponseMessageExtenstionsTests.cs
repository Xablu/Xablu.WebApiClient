using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xablu.WebApiClient.HttpExtensions;
using Xablu.WebApiClient.UnitTests.Mocks;
using Xunit;

namespace Xablu.WebApiClient.UnitTests
{
    public class HttpResponseMessageExtenstionsTests
    {
        [Fact]
        public async Task BuildRestApiResult_SuccessResponse_CallResolveOnHttpResponseResolverOnce()
        {
            var mockResolver = new MockHttpResponseResolver();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var result = await responseMessage.BuildRestApiResult<string>(mockResolver);

            Assert.Equal(1, mockResolver.CallsToResolveHttpResponse);
        }

        [Fact]
        public async Task BuildRestApiResult_SuccessResponse_ResultStatusCodeMatchesResponseStatusCode()
        {
            var mockResolver = new MockHttpResponseResolver();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var result = await responseMessage.BuildRestApiResult<string>(mockResolver);

            Assert.Equal(responseMessage.StatusCode, result.HttpStatusCode);
        }

        [Fact]
        public async Task BuildRestApiResult_SuccessResponse_ResultReasonPhraseMatchesResponseReasonPhrase()
        {
            var mockResolver = new MockHttpResponseResolver();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var result = await responseMessage.BuildRestApiResult<string>(mockResolver);

            Assert.Equal(responseMessage.ReasonPhrase, result.ReasonPhrase);
        }

        [Fact]
        public async Task BuildRestApiResult_FailedResponse_NoCallToResolveOnHttpResponseResolver()
        {
            var mockResolver = new MockHttpResponseResolver();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            var result = await responseMessage.BuildRestApiResult<string>(mockResolver);

            Assert.Equal(0, mockResolver.CallsToResolveHttpResponse);
        }

        [Fact]
        public async Task BuildRestApiResult_FailedResponse_ResultStatusCodeMatchesResponseStatusCode()
        {
            var mockResolver = new MockHttpResponseResolver();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            var result = await responseMessage.BuildRestApiResult<string>(mockResolver);

            Assert.Equal(result.HttpStatusCode, responseMessage.StatusCode);
        }

        [Fact]
        public async Task BuildRestApiResult_FailedResponse_ResultReasonPhraseMatchesResponseReasonPhrase()
        {
            var mockResolver = new MockHttpResponseResolver();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            var result = await responseMessage.BuildRestApiResult<string>(mockResolver);

            Assert.Equal(result.ReasonPhrase, responseMessage.ReasonPhrase);
        }

        [Fact]
        public async Task BuildRestApiResult_FailedResponseWithNoContent_ResultDetailedErrorMessageIsEmpty()
        {
            var mockResolver = new MockHttpResponseResolver();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            var result = await responseMessage.BuildRestApiResult<string>(mockResolver);

            Assert.Equal(string.Empty, result.DetailedErrorMessage);
        }

        [Fact]
        public async Task BuildRestApiResult_FailedResponseWithContent_ResultDetailedErrorMessageMatchesContent()
        {
            const string expectedErrorMessage = "Very detailed error message";
            var mockResolver = new MockHttpResponseResolver();
            var responseMessage =
                new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(expectedErrorMessage)
                };

            var result = await responseMessage.BuildRestApiResult<string>(mockResolver);

            Assert.Equal(expectedErrorMessage, result.DetailedErrorMessage);
        }
    }
}
