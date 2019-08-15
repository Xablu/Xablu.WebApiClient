using System;
namespace Xablu.WebApiClient.Services
{
    public interface IRefitService<T>
    {
        T Speculative { get; }
        T UserInitiated { get; }
        T Background { get; }
    }
}
