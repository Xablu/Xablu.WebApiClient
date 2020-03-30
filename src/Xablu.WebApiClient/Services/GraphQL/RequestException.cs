using System;
using System.Diagnostics;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class RequestException : Exception
    {
        public RequestException(string statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            Debug.WriteLine($"Statuscode:{StatusCode}: {Environment.NewLine} {errorMessage}");
        }

        public RequestException(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Debug.WriteLine(errorMessage);
        }

        public string StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
