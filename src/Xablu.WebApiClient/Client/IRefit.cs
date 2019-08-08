using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fusillade;
using Refit;
using Xablu.WebApiClient.HttpExtensions;

namespace Xablu.WebApiClient.Client
{
    public interface IRefit
    {
        [Get("")]
        Task GetTask();

        [Post("")]
        Task PostTask();

        [Put("")]
        Task PutTask();

        [Patch("")]
        Task PatchTask();

        [Delete("")]
        Task DeleteTask();
    }
}
