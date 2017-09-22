using System;
using System.Net.Http;
using Xablu.WebApiClient.UnitTests.Fakes;

namespace Xablu.WebApiClient.UnitTests.Builders
{
    public class RestApiClientBuilder
    {
        private readonly FakeMessageHandler _fakeHandler;

        private HttpResponseMessage _responseMessage;
        private IHttpContentResolver _contentResolver;
        private IHttpResponseResolver _responseResolver;
        private string _baseAddress;

        public RestApiClientBuilder()
        {
            _fakeHandler = new FakeMessageHandler();
        }

        public RestApiClientBuilder SetBaseAddress(string baseAddress)
        {
            _baseAddress = baseAddress;
            return this;
        }

        public RestApiClientBuilder SetHttpContentResolver(IHttpContentResolver contentResolver)
        {
            _contentResolver = contentResolver;
            return this;
        }

        public RestApiClientBuilder SetHttpResponseResolver(IHttpResponseResolver responseResolver)
        {
            _responseResolver = responseResolver;
            return this;
        }

        public HttpResponseMessage When(string path)
        {
            var uri = new Uri(_baseAddress);
            var uriBuilder = new UriBuilder(uri) {Path = path};

            _responseMessage = _fakeHandler.When(uriBuilder.Uri);

            return _responseMessage;
        }

        public IRestApiClient Build()
        {
            var restApiClient =
                new RestApiClient(_baseAddress, () => _fakeHandler)
                {
                    HttpContentResolver = _contentResolver,
                    HttpResponseResolver = _responseResolver
                };

            return restApiClient;
        }
    }
}
