using System;
using GraphQL.Common.Response;
namespace Xablu.WebApiClient.Services.GraphQL
{
    public class RequestException : Exception
    {
        public RequestException(string statusCode, string errorMessage, Action onFailure = null)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            Console.WriteLine($"Statuscode:{StatusCode}:" + Environment.NewLine +
                              $"{errorMessage}");
            OnFailure = onFailure;
            OnFailure?.Invoke();
        }

        public RequestException(string errorMessage, Action onFailure = null)
        {
            ErrorMessage = errorMessage;
            Console.WriteLine(errorMessage);
            OnFailure = onFailure;
            OnFailure?.Invoke();
        }

        public string StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public Action OnFailure { get; set; }
    }
}
