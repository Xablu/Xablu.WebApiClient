using System;
using BooksQL.API.GraphQL.Types;
using BooksQL.API.Repositories;
using GraphQL.Types;

namespace BooksQL.API.GraphQL
{
    public class BooksQuery : ObjectGraphType
    {
        public BooksQuery(BooksRepository booksRepository)
        {
            Field<ListGraphType<BookType>>(
                "books",
                resolve: context => booksRepository.GetBooks()
            );

            Field<BookType>(
                "book",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "iSBN" }),
                resolve: context =>
                {
                    var isbn = context.GetArgument<string>("iSBN");
                    return booksRepository.GetBook(isbn);
                }
            );
        }
    }
}
