using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Xablu.WebApiClient.Exceptions;

namespace Xablu.WebApiClient.HttpExtensions
{

	public static class HttpResponseMessageExtensions
	{
		public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				return;
			}

			var content = await response.Content.ReadAsStringAsync();

			if (response.Content != null)
				response.Content.Dispose();

			var exception = new HttpResponseException(response.StatusCode, response.ReasonPhrase, content);

			switch (response.StatusCode)
			{
				case HttpStatusCode.Gone: // session expired
					throw new NoSessionException(response.ReasonPhrase, exception);
				default:
				throw exception;
			}
		}
	}

	public class HttpResponseException : Exception
	{
		public HttpStatusCode StatusCode { get; private set; }
		public string Content { get; private set; }

		public HttpResponseException (HttpStatusCode statusCode, string reasonPhrase, string content)
			: base (reasonPhrase)
		{
			StatusCode = statusCode;
			Content = content;
		}
	}
}

