using System;
using Xablu.WebApiClient.Services;

namespace Xablu.WebApiClient.Client
{
    public interface IRefitClient
    {
        RefitService<IRefit> RefitService { get; }
    }
}
