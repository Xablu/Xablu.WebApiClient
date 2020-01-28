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

        public async Task CreateReview()
        {
            var bookreview = new BookReview
            {
                BookISBN = "0544272994",
                Review = "This is a mutation test"
            };
            var review = await _webApiClient.SendMutationAsync(new MutationRequest<BookReview>(new MutationDetail("createReview", "review"), new { bookISBN = bookreview }));


            //var asd = new MutationRequest<BookReview>(new MutationDetail("createReview", "review"), bookreview);

            //JsonConvert.SerializeObject(new
            //{
            //    reviewNico = new
            //    {
            //        bookISBN = "0544272994",
            //        review = "This is a mutation test"
            //    }
            //});

            //var result = await _webApiClient.SendMutationAsync<BookReview>(
            //    @"mutation ($reviewNico: reviewInput!) {
            //        createReview(review: $reviewNico) {
            //            id
            //            review
            //        }
            //    }",
            //    //"{ review: { bookISBN: \"0544272994\", review:  \"cool!\"} }");
            //    new {
            //        reviewNico = new BookReview {
            //            BookISBN = "0544272994",
            //            Review = "This is a mutation test"
            //        }
            //    });
            //{
            //    reviewNico = new
            //    {
            //        bookISBN = "0544272994",
            //        review = "This is a mutation test"
            //    }
            //});
            //new {
            //    reviewNico = new {
            //        bookISBN = "0544272994",
            //        review = "This is a mutation test"
            //        }
            //    });
        }
    }

    public class BooksResponseModel
    {
        public List<Book> Books { get; set; }
    }
}
