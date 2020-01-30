using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksQL.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace BooksQL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BooksMasterDetailPageDetail : ContentPage
    {
        public BooksMasterDetailPageDetail()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }


    }
}
