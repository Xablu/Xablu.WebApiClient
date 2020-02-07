using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BooksQL.Models;
using BooksQL.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xablu.WebApiClient.Services.GraphQL;
using Xamarin.Forms;

namespace BooksQL.ViewModels
{
    public class MutationViewModel : INotifyPropertyChanged
    {
        private BooksService _booksService;
        private string _query;
        private string _result;

        public MutationViewModel()
        {
            _booksService = DependencyService.Resolve<BooksService>();
            RefreshCommand = new Command(() => CreateMutation());
            BookReview = new BookReview
            {
                BookISBN = "0544272994",
                Review = "This is a mutation test"
            };

            SetQuery();
        }

        public Command RefreshCommand { get; private set; }

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

        public string Result
        {
            get { return _result; }
            set
            {
                if (_result != value)
                {
                    _result = value;
                    OnPropertyChanged();
                }
            }
        }

        public BookReview BookReview { get; set; }

        private void SetQuery()
        {
            var result = new MutationRequest<BookReview>(new MutationDetail("createReview", "review"), BookReview);
            Query = result.Query;
        }

        private async Task CreateMutation()
        {
            var review = await _booksService.CreateReview(BookReview);

            var json = JsonConvert.SerializeObject(review);
            var formattedJson = JValue.Parse(json).ToString(Formatting.Indented);

            Result = formattedJson;
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
