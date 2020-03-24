using System;
using BooksQL.API.Repositories;
using BooksQL.API.Entities;
using GraphQL.Types;

namespace BooksQL.API.GraphQL.Types
{
    public class BookType : ObjectGraphType<Book>
    {
        public BookType(BookReviewsRepository bookReviewsRepository)
        {
            Field(t => t.ISBN);
            Field(t => t.Title);
            Field(t => t.Authors);
            Field(t => t.PublishDate);
            Field<GenreEnumType>("genre", "Genre of book");
             
            Field<ListGraphType<BookReviewType>>(
                "reviews",
                resolve: context => bookReviewsRepository.GetBookReviews(context.Source.ISBN)
            );
        }
    }
}
