using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Xablu.WebApiClient.Native
{
    public class NativeHttpClientHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}
