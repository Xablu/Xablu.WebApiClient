using System;
using Xablu.WebApiClient.Attributes;

namespace BooksQL.Models
{
    [VariableInputType("reviewInput")]
    public class BookReview
    {
        public string BookISBN { get; set; }
        public string Review { get; set; }
        //public string Id { get; set; }
    }
}
