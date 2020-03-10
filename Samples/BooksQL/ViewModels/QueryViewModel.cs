using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BooksQL.Models;
using BooksQL.Services;
using Xablu.WebApiClient.Services.GraphQL;
using Xamarin.Forms;

namespace BooksQL.ViewModels
{
    public class QueryViewModel : INotifyPropertyChanged
    {
        private BooksService _booksService;
        private string _query = "Query result";

        public QueryViewModel()
        {
            _booksService = DependencyService.Resolve<BooksService>();

            RefreshCommand = new Command(() => GetBooks());
            SetQuery();
        }

        public Command RefreshCommand { get; private set; }

        public bool IsRefreshing { get; private set; }

        public string Query
        {
            get { return _query; }
            set
            {
                if (_query != value)
                {
                    _query = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Book> Books { get; private set; } = new ObservableCollection<Book>();

        private void SetQuery()
        {
            var result = new Request<BooksResponseModel>();
            Query = result.Query;
        }

        private async Task GetBooks()
        {
            try
            {
                IsRefreshing = true;

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

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}