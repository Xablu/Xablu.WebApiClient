using System;
using BooksQL.API.Entities;
using BooksQL.API.GraphQL.Types;
using BooksQL.API.Repositories;
using GraphQL.Types;

namespace BooksQL.API.GraphQL
{
    public class BooksMutation : ObjectGraphType
    {
        public BooksMutation(BookReviewsRepository bookReviewsRepository)
        {
            FieldAsync<BookReviewType>(
                "createReview",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<BookReviewInputType>> { Name = "review" }),
                resolve: async context =>
                {
                    var review = context.GetArgument<BookReview>("review");

                    // try or catch exception and add it to GraphQL errors
                    return await context.TryAsyncResolve(
                        async c => await bookReviewsRepository.CreateBookReview(review));
                }
            );
        }
    }
}
