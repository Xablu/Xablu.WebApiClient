using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksQL.API.Entities;

namespace BooksQL.API.Repositories
{
    public class BooksRepository
    {
        private static List<Book> Books { get; } = new List<Book>
        {
            new Book
            {
                ISBN = "0544272994",
                Title = "What If?: Serious Scientific Answers to Absurd Hypothetical Questions",
                Authors = new List<string> { "Randall Munroe" } ,
                PublishDate =  new DateTime(2014, 09, 02),
                Genre = Genre.Humour
            },
            new Book
            {
                ISBN = "9780553386790",
                Title = "Game of Thrones",
                Authors = new List<string> { "George R R Martin" },
                PublishDate = new DateTime(1996, 08, 01),
                Genre = Genre.Fiction
            },
            new Book
            {
                ISBN = "9789173488297",
                Title = "A Clash of Kings",
                Authors = new List<string> { "George R R Martin" },
                PublishDate = new DateTime(1998, 11, 16),
                Genre = Genre.Fiction
            },
            new Book
            {
                ISBN = "9780553801477",
                Title = "A Dance with Dragons",
                Authors = new List<string> { "George R R Martin" },
                PublishDate = new DateTime(2011, 07, 12),
                Genre = Genre.Fiction
            },
            new Book
            {
                ISBN = "0446677450",
                Title = "Rich Dad Poor Dad",
                Authors = new List<string> { "Robert Kiyosaki", "Sharon Lechter" },
                Genre = Genre.Finances
            }
        };

        public Task<List<Book>> GetBooks()
        {
            return Task.FromResult(Books);
        }

        public Task<Book> GetBook(string isbn)
        {
            return Task.FromResult(Books.First(b => b.ISBN == isbn));
        }
    }
}
