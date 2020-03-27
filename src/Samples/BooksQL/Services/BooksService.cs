using System.Collections.Generic;
using System.Threading.Tasks;
using BooksQL.ApiEndpoints;
using BooksQL.Models;
using BooksQL.Models.GraphQL;
using Xablu.WebApiClient;
using Xablu.WebApiClient.Services.GraphQL;
using Xamarin.Essentials;

namespace BooksQL.Services
{
    public class BooksService
    {
        private readonly IWebApiClient<IBooksApi> _webApiClient;

        public BooksService()
        {
            _webApiClient = WebApiClientFactory.Get<IBooksApi>(GetEndPoint());
        }

        public async Task<IEnumerable<Book>> GetBooks(Request<BooksQueryResponse> request)
        {
            var booksResponse = await _webApiClient.SendQueryAsync(request);
            return booksResponse.Books;
        }

        public async Task<BookReviewMutationResponse> CreateReview(MutationRequest<BookReviewMutationResponse> mutationRequest)
        {
            var review = await _webApiClient.SendMutationAsync(mutationRequest);
            return review;
        }

        private string GetEndPoint()
        {
            switch (DeviceInfo.Platform.ToString())
            {
                case "iOS":
                    return "http://localhost:5000";
                case "Android":
                    return "http://10.0.2.2:5000";
                default:
                    return "http://localhost:5000";
            }
        }
    }
}
