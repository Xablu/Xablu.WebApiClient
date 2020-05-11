using System;
using BooksQL.API.Repositories;
using BooksQL.API.Entities;
using GraphQL.Types;
using GraphQL.DataLoader;

namespace BooksQL.API.GraphQL.Types
{
    public class BookType : ObjectGraphType<Book>
    {
        public BookType(
            BookReviewsRepository bookReviewsRepository)
            //IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            Field(t => t.ISBN).Description("Unique identifier for a book");
            Field(t => t.Title);
            Field(t => t.Authors);
            Field(t => t.PublishDate);
            Field<GenreEnumType>("genre", "Genre of book");

            Field<ListGraphType<BookReviewType>>(
                "reviews",
                resolve: context => bookReviewsRepository.GetBookReviews(context.Source.ISBN));

                //resolve: context =>
                //{
                //    var loader = dataLoaderContextAccessor.Context.GetOrAddCollectionBatchLoader<string, BookReview>(
                //        "GetBookReviewByBookISBNs", (isbns) => bookReviewsRepository.GetBookReviewsByMultipleISBNs(isbns));

                //    return loader.LoadAsync(context.Source.ISBN);
                //}
        }
    }
}
