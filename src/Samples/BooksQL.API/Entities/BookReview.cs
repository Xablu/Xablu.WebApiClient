using System;
namespace BooksQL.API.Entities
{
    public class BookReview
    {
        public int Id { get; set; }
        public string BookISBN { get; set; }
        public string Review { get; set; }
    }
}
