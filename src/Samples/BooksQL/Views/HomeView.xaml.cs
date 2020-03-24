using System;
using System.Collections.Generic;
using BooksQL.ViewModels;
using Xamarin.Forms;

namespace BooksQL.Views
{
    public partial class HomeView : ContentPage
    {
        public HomeView()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel();
        }
    }
}
