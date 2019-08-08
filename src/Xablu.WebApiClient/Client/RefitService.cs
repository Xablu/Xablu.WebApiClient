using System;
using System.Net.Http;
using Fusillade;
using Refit; 

namespace Xablu.WebApiClient.Client
{
    public class RefitService<T> : IRefitService<T>
    {
        public const string ApiBaseAddress = "";

        private readonly Lazy<T> _background;
        private readonly Lazy<T> _userInitiated;
        private readonly Lazy<T> _speculative;

        public RefitService(string apiBaseAddress = null)
        {
            Func<HttpMessageHandler, T> createClient = messageHandler =>
            {
                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(apiBaseAddress ?? ApiBaseAddress)
                };

                return RestService.For<T>(client);
            };

            _background = new Lazy<T>(() => createClient(NetCache.Background));

            _userInitiated = new Lazy<T>(() => createClient(NetCache.UserInitiated));

            _speculative = new Lazy<T>(() => createClient(NetCache.Speculative));
        }

        public T Background => _background.Value;

        public T UserInitiated => _userInitiated.Value;

        public T Speculative => _speculative.Value;
    }
}
