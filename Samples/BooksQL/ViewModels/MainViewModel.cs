using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BooksQL.Models;
using BooksQL.Services;
using Xamarin.Forms;

namespace BooksQL.ViewModels
{
    public class MainViewModel
    {
        private BooksService _booksService;

        public MainViewModel()
        {
            _booksService = DependencyService.Resolve<BooksService>();

            RefreshCommand = new Command(() => GetBooks());

            RefreshCommand.Execute(null);
        }

        public Command RefreshCommand { get; private set; }

        public bool IsRefreshing { get; private set; }

        public ObservableCollection<Book> Books { get; private set; } = new ObservableCollection<Book>();

        private async Task GetBooks()
        {
            try
            {
                IsRefreshing = true;

                await _booksService.CreateReview();

                var books = await _booksService.GetBooks();

                Books.Clear();

                foreach (var book in books)
                {
                    Books.Add(book);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}
