using System;
using Xablu.WebApiClient.Attributes;

namespace TestApp.Services
{
    [GraphQLEndpoint("/graphql")]
    public interface IGitHubApi
    {
        // we could of course have some REST endpoints here and use Refit!
    }
}
