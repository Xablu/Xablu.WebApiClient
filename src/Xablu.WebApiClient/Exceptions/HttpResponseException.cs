using System;
using System.Net;
            
namespace Xablu.WebApiClient.Exceptions
{
	public class HttpResponseException : Exception
	{
		public HttpStatusCode StatusCode { get; private set; }
		public string Content { get; private set; }

		public HttpResponseException(HttpStatusCode statusCode, string reasonPhrase, string content)
			: base(reasonPhrase)
		{
			StatusCode = statusCode;
			Content = content;
		}
	}
}
