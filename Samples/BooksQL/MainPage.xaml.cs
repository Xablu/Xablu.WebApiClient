using System.ComponentModel;
using BooksQL.ViewModels;
using Xamarin.Forms;

namespace BooksQL
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private MainViewModel ViewModel => (MainViewModel)BindingContext;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        } 
    }
}
