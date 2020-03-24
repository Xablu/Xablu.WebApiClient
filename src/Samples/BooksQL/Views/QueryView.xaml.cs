using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BooksQL.Models;
using BooksQL.Services;
using BooksQL.ViewModels;
using BooksQL.Views.Items;
using Xablu.WebApiClient.Services.GraphQL;
using Xamarin.Forms;

namespace BooksQL.Views
{
    public partial class QueryView : ContentPage
    {
        public QueryView()
        {
            InitializeComponent();
            BindingContext = new QueryViewModel();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}
