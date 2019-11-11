using System;
using Xablu.WebApiClient.Enums;

namespace Xablu.WebApiClient.Services.Rest
{
    public interface IRefitService<T>
    {
        T GetByPriority(Priority priority);
    }
}
