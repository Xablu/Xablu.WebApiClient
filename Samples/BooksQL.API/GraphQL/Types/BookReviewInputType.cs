using System;
using GraphQL.Types;

namespace BooksQL.API.GraphQL.Types
{
    public class BookReviewInputType : InputObjectGraphType
    {
        public BookReviewInputType()
        {
            Name = "reviewInput";
            Field<NonNullGraphType<StringGraphType>>("bookISBN");
            Field<NonNullGraphType<StringGraphType>>("review");
        }
    }
}
