using System;
using Xablu.WebApiClient.Enums;

namespace Xablu.WebApiClient
{
    public class RequestOptions
    {
        public int RetryCount { get; set; } = WebApiClient.DefaultRetryCount;

        public int Timeout { get; set; } = WebApiClient.DefaultTimeout;

        public Priority Priority { get; set; } = WebApiClient.DefaultPriority;

        public Func<Exception, bool> ShouldRetry { get; set; } = WebApiClient.DefaultShouldRetry;
    }
}
