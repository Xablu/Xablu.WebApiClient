using System;
using GraphQL;
using GraphQL.Types;

namespace BooksQL.API.GraphQL
{
    public class BooksSchema : Schema
    {
        public BooksSchema(IDependencyResolver dependencyResolver) : base(dependencyResolver)
        {
            Query = dependencyResolver.Resolve<BooksQuery>();

            Mutation = dependencyResolver.Resolve<BooksMutation>();

            //Subscription =
        }
    }
}
