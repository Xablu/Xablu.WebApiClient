using System;
using Xablu.WebApiClient.Enums;

namespace Xablu.WebApiClient
{
    public class RequestOptions
    {
        public int RetryCount { get; set; } = 2;

        public int Timeout { get; set; } = 60;

        public Priority Priority { get; set; } = Priority.UserInitiated;

        public Func<Exception, bool> ShouldRetry { get; set; }
    }
}
