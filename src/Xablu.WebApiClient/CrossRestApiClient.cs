using System;
using Xablu.WebApiClient.Abstractions;
using Xablu.WebApiClient.Abstractions.Exceptions;

namespace Xablu.WebApiClient
{
    public class CrossRestApiClient
    {
        private static Action<RestApiClientOptions> _configureRestApiClient;
        private static Lazy<IRestApiClient> _restApiClientImplementation = new Lazy<IRestApiClient>(() => CreateRestApiClient(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static bool IsSupported => _restApiClientImplementation.Value == null ? false : true;

        public static void Configure(Action<RestApiClientOptions> options)
        {
            _configureRestApiClient = options;
        }

        public static IRestApiClient Current
        {
            get
            {
                var ret = _restApiClientImplementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        private static IRestApiClient CreateRestApiClient()
        {
#if NETSTANDARD2_0
			return null;
#else
            if(_configureRestApiClient == null)
                throw NotConfiguredException();

            var options = new RestApiClientOptions();
            _configureRestApiClient.Invoke(options);
            return new RestApiClientImplementation(options);
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

        internal static Exception NotConfiguredException() =>
            new NotConfiguredException("The `CrossRestApiClient` has not been configured. Make sure you call the `CrossRestApiClient.Configure` method before accessing the `CrossRestApiClient.Current` property.");

    }
}
