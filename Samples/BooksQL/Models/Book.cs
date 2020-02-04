using System;
using System.Collections.Generic;
using Xablu.WebApiClient.Attributes;

namespace BooksQL.Models
{
    public class Book
    {


        [NameOfField("iSBN")]
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public DateTime PublishDate { get; set; }
        public string Genre { get; set; }
    }
}
