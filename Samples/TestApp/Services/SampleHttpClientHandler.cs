using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp.Services
{
    public class SampleHttpClientHandler : DelegatingHandler
    {
        public SampleHttpClientHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
