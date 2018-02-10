using System;

namespace Xablu.WebApiClient.Abstractions.Exceptions
{
    public class NotConfiguredException : Exception
    {
        public NotConfiguredException()
        {
        }

        public NotConfiguredException(string message)
            : base(message)
        {
        }

        public NotConfiguredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
