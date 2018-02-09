using Sample.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sample
{
	public partial class MainPage : ContentPage
	{
        private readonly TodoViewModel _viewModel;

		public MainPage()
		{
			InitializeComponent();

            _viewModel = new TodoViewModel();
            BindingContext = _viewModel;
		}

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.LoadData();
        }
    }
}
