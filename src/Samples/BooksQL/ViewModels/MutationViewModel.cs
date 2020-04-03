using System.Threading.Tasks;
using BooksQL.Models;
using BooksQL.Models.GraphQL;
using BooksQL.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xablu.WebApiClient.Services.GraphQL;
using Xamarin.Forms;

namespace BooksQL.ViewModels
{
    public class MutationViewModel : BaseViewModel
    {
        private BooksService _booksService;
        private MutationRequest<BookReviewMutationResponse> _request;
        private string _query;
        private string _result;

        public MutationViewModel()
        {
            _booksService = DependencyService.Resolve<BooksService>();

            RefreshCommand = new Command(() => CreateMutation());
            
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

        private void SetQuery()
        {
            var bookReview = new BookReview
            {
                BookISBN = "0544272994",
                Review = "This is a mutation test"
            };
            _request = new MutationRequest<BookReviewMutationResponse>("createReview", "review", bookReview);
            Query = _request.Query;
        }

        private async Task CreateMutation()
        {
            var review = await _booksService.CreateReview(_request);

            var json = JsonConvert.SerializeObject(review);
            var formattedJson = JValue.Parse(json).ToString(Formatting.Indented);

            Result = formattedJson;
        }
    }
}
