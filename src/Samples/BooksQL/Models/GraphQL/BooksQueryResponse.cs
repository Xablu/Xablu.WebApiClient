using System;
using System.Collections.Generic;

namespace BooksQL.Models.GraphQL
{
    public class BooksQueryResponse
    {
        public List<Book> Books { get; set; }
    }
}
