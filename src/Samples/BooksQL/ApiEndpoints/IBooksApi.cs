using Xablu.WebApiClient.Attributes;

namespace BooksQL.ApiEndpoints
{
    [GraphQLEndpoint("/graphql")]
    public interface IBooksApi
    {
        // we could of course have some REST endpoints here and use Refit!
    }
}
