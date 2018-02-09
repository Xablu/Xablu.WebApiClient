using System;

namespace Xablu.WebApiClient.Abstractions.Exceptions
{
    public class NoSessionException : Exception
    {
        public NoSessionException()
        {
        }

        public NoSessionException(string message)
            : base(message)
        {
        }

        public NoSessionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}