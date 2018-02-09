using System.Net;
using Xablu.WebApiClient.Abstractions;

namespace Xablu.WebApiClient.Abstractions.HttpExtensions
{
    public delegate void OnSuccessAction<TResult>(TResult result);

    public delegate void OnFailureAction(HttpStatusCode httpStatusCode, string reasonPhrase,
        string detailedErrorMessage);

    public static class RestApiResultExtensions
    {
        public static IRestApiResult<TResult> OnSuccess<TResult>(this IRestApiResult<TResult> restApiResult,
            OnSuccessAction<TResult> action)
        {
            if (restApiResult.IsSuccessStatusCode)
                action(restApiResult.Content);

            return restApiResult;
        }

        public static IRestApiResult<TResult> OnFailure<TResult>(this IRestApiResult<TResult> restApiResult,
            OnFailureAction action)
        {
            if (!restApiResult.IsSuccessStatusCode)
                action(restApiResult.HttpStatusCode, restApiResult.ReasonPhrase, restApiResult.DetailedErrorMessage);

            return restApiResult;
        }
    }
}