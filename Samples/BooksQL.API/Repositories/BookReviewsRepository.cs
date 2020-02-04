using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksQL.API.Entities;

namespace BooksQL.API.Repositories
{
    public class BookReviewsRepository
    {
        private static List<BookReview> Reviews { get; } = new List<BookReview>
        {
            new BookReview
            {
                BookISBN = "0544272994",
                Id = 1,
                Review = "Amusing, highly recommendable!"
            },
            new BookReview
            {
                BookISBN = "0544272994",
                Id = 2,
                Review = "I still look forward to a second part"
            },
            new BookReview
            {
                BookISBN = "9780553386790",
                Id = 3,
                Review = "Much better than the series"
            },
            new BookReview
            {
                BookISBN = "9780553386790",
                Id = 4,
                Review = "Everyone dies"
            },
            new BookReview
            {
                BookISBN = "9780553386790",
                Id = 5,
                Review = "I just can't stop reading it!"
            },
            new BookReview
            {
                BookISBN = "9789173488297",
                Id = 6,
                Review = "The throne belongs to Joffrey - no doubts!"
            },
            new BookReview
            {
                BookISBN = "9780553801477",
                Id = 7,
                Review = "Best book ever"
            },
            new BookReview
            {
                BookISBN = "0446677450",
                Id = 8,
                Review = "This should be read at schools"
            },
            new BookReview
            {
                BookISBN = "0446677450",
                Id = 9,
                Review = "Interesting but not very practical"
            },
        };

        public BookReviewsRepository()
        {
        }

        public Task<IEnumerable<BookReview>> GetBookReviews(string isbn)
        {
            return Task.FromResult(Reviews.Where(r => r.BookISBN == isbn));
        }

        public Task<BookReview> CreateBookReview(BookReview bookReview)
        {
            var last = Reviews.Last();
            bookReview.Id = last.Id + 1;
            Reviews.Add(bookReview);
            return Task.FromResult(bookReview);
        }
    }
}
