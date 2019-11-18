using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Android.Net;

namespace Xablu.WebApiClient.Native
{
    public class NativeHttpClientHandler : AndroidClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}
