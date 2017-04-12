using System;
using System.Threading.Tasks;
using Polly;
using System.Net;

namespace Xablu.WebApiClient
{
	public class BaseClient
	{
		protected readonly IWebApiClient apiClient;

		public BaseClient(IWebApiClient apiClient)
		{
			if (apiClient == null)
				throw new ArgumentNullException(nameof(apiClient));

			this.apiClient = apiClient;
		}

		protected async Task<TResult> ExecuteRemoteRequest<TResult>(Func<Task<TResult>> action)
		{
			TResult result = default(TResult);

			try
			{
				result = await Policy
					.Handle<WebException>()
					.WaitAndRetryAsync
					(
						retryCount: 5,
						sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
					)
					.ExecuteAsync(action).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				if (e.GetType().Namespace == "Java.Net")
				{
					throw new HttpExtensions.HttpResponseException(HttpStatusCode.RequestTimeout, e.Message, e.StackTrace);
				}
				throw;
			}

			return result;
		}
	}
}
