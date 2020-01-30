using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BooksQL.ViewModels;
using BooksQL.Views.Items;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using ListView = Xamarin.Forms.ListView;

namespace BooksQL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BooksMasterDetailPageMaster : ContentPage
    {
        public ListView ListView;

        public BooksMasterDetailPageMaster()
        {
            InitializeComponent();

            BindingContext = new BooksMasterDetailPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //var safeInsets = On<iOS>().SafeAreaInsets();
            //safeInsets.Top = 20;
            //Padding = safeInsets;
        }

    }
}
