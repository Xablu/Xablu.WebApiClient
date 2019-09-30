using System;
using Xablu.WebApiClient.Enums;

namespace Xablu.WebApiClient.Client
{
    public interface IRefitService<T>
    {
        T GetByPriority(Priority priority);
    }
}
