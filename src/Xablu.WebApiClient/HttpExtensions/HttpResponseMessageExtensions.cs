//using System.Threading.Tasks;
//using System.Net.Http;
//using System.Net;
//using Xablu.WebApiClient.Exceptions;

//namespace Xablu.WebApiClient.HttpExtensions
//{
//    public static class HttpResponseMessageExtensions
//    {
//        public static async Task<bool> EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
//        {
//            if (response.IsSuccessStatusCode)
//            {
//                return true;
//            }

//            var content = await response.Content.ReadAsStringAsync();

//            if (response.Content != null)
//                response.Content.Dispose();

//            var exception = new HttpResponseException(response.StatusCode, response.ReasonPhrase, content);

//            switch (response.StatusCode)
//            {
//                case HttpStatusCode.Gone: // session expired
//                    throw new NoSessionException(response.ReasonPhrase, exception);
//                default:
//                    throw exception;
//            }
//        }

//        public static async Task<IRestApiResult<TResult>> BuildRestApiResult<TResult>(this HttpResponseMessage response,
//            IHttpResponseResolver resolver)
//        {
//            if (response.IsSuccessStatusCode)
//            {
//                var result = await resolver.ResolveHttpResponseAsync<TResult>(response);
//                return new RestApiResult<TResult>(response.StatusCode, result, response.ReasonPhrase);
//            }

//            var errorMessage = string.Empty;
//            if (response.Content != null)
//            {
//                errorMessage = await response.Content.ReadAsStringAsync();
//            }

//            return new RestApiResult<TResult>(response.StatusCode, default(TResult), response.ReasonPhrase,
//                errorMessage);
//        }
//    }
//}