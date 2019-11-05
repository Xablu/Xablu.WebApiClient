using System;
using GraphQL.Common.Response;
namespace Xablu.WebApiClient.Services.GraphQL
{
    public class RequestException : Exception
    {
        public RequestException(string statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            Console.WriteLine($"Statuscode:{StatusCode}:" + Environment.NewLine +
                              $"{errorMessage}");
        }

        public RequestException(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Console.WriteLine(errorMessage);
        }

        public string StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
