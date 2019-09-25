using GraphQL.Client.Http;
using Xablu.WebApiClient.Enums;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public interface IGraphQLService
    {
        string BaseUrl { get; }

        GraphQLHttpClient GetByPriority(Priority priority);        
    }
}
