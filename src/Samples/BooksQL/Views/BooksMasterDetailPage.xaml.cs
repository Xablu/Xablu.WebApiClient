using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksQL.Views.Items;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BooksQL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BooksMasterDetailPage : MasterDetailPage
    {
        public BooksMasterDetailPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;
            MasterPage.ListView.SelectedItem = null;
        }
    }
}
