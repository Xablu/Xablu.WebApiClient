using System;
using System.Net.Http;
using Fusillade;
using Refit;
using Splat;

namespace Xablu.WebApiClient.Client
{
    public class RefitService<T> : IRefitService<T>
    {
        public const string ApiBaseAddress = "";

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

            _background = new Lazy<T>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.Background)));

            _userInitiated = new Lazy<T>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.UserInitiated)));

            _speculative = new Lazy<T>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.Speculative)));
        }

        private readonly Lazy<T> _background;
        private readonly Lazy<T> _userInitiated;
        private readonly Lazy<T> _speculative;

        public T Background
        {
            get { return _background.Value; }
        }

        public T UserInitiated
        {
            get { return _userInitiated.Value; }
        }

        public T Speculative
        {
            get { return _speculative.Value; }
        }
    }
}
