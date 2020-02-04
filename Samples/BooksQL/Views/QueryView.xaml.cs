using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BooksQL.Models;
using BooksQL.Services;
using BooksQL.ViewModels;
using Xablu.WebApiClient.Services.GraphQL;
using Xamarin.Forms;

namespace BooksQL.Views
{
    public partial class QueryView : ContentPage
    {
        private BooksService _booksService;

        public QueryView()
        {
            InitializeComponent();
            BindingContext = new QueryViewModel();
        }



    }
}
