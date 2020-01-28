using System;
using BooksQL.API.Entities;
using GraphQL.Types;

namespace BooksQL.API.GraphQL.Types
{
    public class BookReviewType : ObjectGraphType<BookReview>
    {
        public BookReviewType()
        {
            Field(t => t.BookISBN);
            Field(t => t.Id);
            Field(t => t.Review);
        }
    }
}
