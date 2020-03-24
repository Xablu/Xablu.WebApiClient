using System;
using System.Collections.Generic;
using BooksQL.ViewModels;
using Xamarin.Forms;

namespace BooksQL.Views
{
    public partial class MutationView : ContentPage
    {
        public MutationView()
        {
            InitializeComponent();
            BindingContext = new MutationViewModel();
        }
    }
}
