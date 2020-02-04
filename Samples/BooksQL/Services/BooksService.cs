using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksQL.ApiEndpoints;
using BooksQL.Models;
using Newtonsoft.Json;
using Xablu.WebApiClient;
using Xablu.WebApiClient.Services.GraphQL;

namespace BooksQL.Services
{
    public class BooksService
    {
        private readonly IWebApiClient<IBooksApi> _webApiClient;

        public BooksService()
        {
            _webApiClient = WebApiClientFactory.Get<IBooksApi>("http://localhost:5000");
        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            var booksResponse = await _webApiClient.SendQueryAsync(new Request<BooksResponseModel>());

            return booksResponse.Books;
        }

        public async Task<BookReview> CreateReview(BookReview bookreview)
        { 
            var review = await _webApiClient.SendMutationAsync(new MutationRequest<BookReview>(new MutationDetail("createReview", "review"), bookreview));
            return review; 
        }
    }

    public class BooksResponseModel
    {
        public List<Book> Books { get; set; }
    }
}
