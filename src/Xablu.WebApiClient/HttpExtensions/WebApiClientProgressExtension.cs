using System.Threading;
using System.Threading.Tasks;
using Fusillade;
using Xablu.WebApiClient.HttpExtensions;

namespace Xablu.WebApiClient
{
    public static class WebApiClientProgressExtension
    {
        public static async Task<TResult> PostAsync<TContent, TResult>(this WebApiClient webApiClient, Priority priority, string path, TContent content = default(TContent), IHttpContentResolver contentResolver = null, ProgressDelegate progressDelegate)
        {
            var httpClient = webApiClient.GetWebApiClient(priority);

            webApiClient.SetHttpRequestHeaders(httpClient);

            var httpContent = webApiClient.ResolveHttpContent(content);

            var stream = await httpContent.ReadAsStreamAsync();
            var progressContent = new ProgressStreamContent(httpContent.Headers, stream, CancellationToken.None);
            progressContent.Progress = progressDelegate;

            var response = await httpClient
                .PostAsync(path, progressContent)
                .ConfigureAwait(false);

            if (!await response.EnsureSuccessStatusCodeAsync())
                return default(TResult);

            return await webApiClient.HttpResponseResolver.ResolveHttpResponseAsync<TResult>(response);
        }

        public static async Task<TResult> PostAsync<TContent, TResult>(this WebApiClient webApiClient, Priority priority, string path, TContent content = default(TContent), IHttpContentResolver contentResolver = null,  CancellationToken cancellationToken = null, ProgressDelegate progressDelegate)
        {
            var httpClient = webApiClient.GetWebApiClient(priority);

            webApiClient.SetHttpRequestHeaders(httpClient);

            var httpContent = webApiClient.ResolveHttpContent(content);

            var stream = await httpContent.ReadAsStreamAsync();
            var progressContent = new ProgressStreamContent(httpContent.Headers, stream, cancellationToken);
            progressContent.Progress = progressDelegate;

            var response = await httpClient
                .PostAsync(path, progressContent)
                .ConfigureAwait(false);

            if (!await response.EnsureSuccessStatusCodeAsync())
                return default(TResult);

            return await webApiClient.HttpResponseResolver.ResolveHttpResponseAsync<TResult>(response);
        }
    }
}
