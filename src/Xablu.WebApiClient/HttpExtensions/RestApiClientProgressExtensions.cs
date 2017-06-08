using System.Threading;
using System.Threading.Tasks;
using Fusillade;
using Xablu.WebApiClient.HttpContentExtensions;

namespace Xablu.WebApiClient.HttpExtensions
{
    public static class RestApiClientProgressExtension
    {
        public static async Task<IRestApiResult<TResult>> PostAsync<TContent, TResult>(this RestApiClient apiClient, Priority priority, string path, TContent content = default(TContent), ProgressDelegate progressDelegate = null, IHttpContentResolver contentResolver = null)
        {
            var httpClient = apiClient.GetRestApiClient(priority);

            apiClient.SetHttpRequestHeaders(httpClient);

            var httpContent = apiClient.ResolveHttpContent(content);

            var stream = await httpContent.ReadAsStreamAsync();
            var progressContent = new ProgressStreamContent(httpContent.Headers, stream, CancellationToken.None);
            progressContent.Progress = progressDelegate;

            var response = await httpClient
                .PostAsync(path, progressContent)
                .ConfigureAwait(false);

            return await response.BuildRestApiResult<TResult>(apiClient.HttpResponseResolver);
        }
    }
}
