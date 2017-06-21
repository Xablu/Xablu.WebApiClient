using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Xablu.WebApiClient.UnitTests.Fakes
{
    internal static class HttpResponseMessageExtensions
    {
        public static HttpResponseMessage Respond(this HttpResponseMessage responseMessage,
            HttpStatusCode statusCode)
        {
            return responseMessage.Respond(statusCode, null, null);
        }

        public static HttpResponseMessage Respond(this HttpResponseMessage responseMessage,
            string mediaType,
            string response)
        {
            return responseMessage.Respond(HttpStatusCode.OK, mediaType, response);
        }

        public static HttpResponseMessage Respond(this HttpResponseMessage responseMessage, 
            HttpStatusCode statusCode,
            string mediaType,
            string response)
        {
            if (!string.IsNullOrEmpty(response))
            {
                var content = new StringContent(response);

                if (!string.IsNullOrEmpty(mediaType))
                {
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse(mediaType);
                }

                responseMessage.Content = content;
            }

            responseMessage.StatusCode = statusCode;

            return responseMessage;
        }
    }
}
