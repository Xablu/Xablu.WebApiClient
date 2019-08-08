using System;
namespace Xablu.WebApiClient.Client
{
    public interface IRefitService<T>
    {
        T Speculative { get; }
        T UserInitiated { get; }
        T Background { get; }
    }
}
