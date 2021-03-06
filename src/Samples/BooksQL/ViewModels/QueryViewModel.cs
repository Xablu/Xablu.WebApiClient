using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BooksQL.Models;
using BooksQL.Models.GraphQL;
using BooksQL.Services;
using Xablu.WebApiClient.Services.GraphQL;
using Xamarin.Forms;

namespace BooksQL.ViewModels
{
    public class QueryViewModel : BaseViewModel
    {
        private readonly BooksService _booksService;

        private QueryRequest<BooksQueryResponse> _request;
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
            _request = new QueryRequest<BooksQueryResponse>();
            Query = _request.Query;
        }

        private async Task GetBooks()
        {
            try
            {
                IsRefreshing = true;

                var books = await _booksService.GetBooks(_request);

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