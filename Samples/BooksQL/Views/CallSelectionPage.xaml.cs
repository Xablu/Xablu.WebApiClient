using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BooksQL.ViewModels;
using BooksQL.Views.Items;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BooksQL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CallSelectionPage : ContentPage
    {
        public ObservableCollection<SelectionItem> SelectionItems { get; set; }

        public CallSelectionPage()
        {
            InitializeComponent();
            BindingContext = new CallSelectionPageViewModel();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as SelectionItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);

            page.Title = item.Title;

            Navigation.PushAsync(new NavigationPage(page) { });

            MyListView.SelectedItem = null;
        }


    }
}
