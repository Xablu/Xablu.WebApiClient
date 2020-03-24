using System;
using BooksQL.Services;
using BooksQL.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BooksQL
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<BooksService>();

            MainPage = new BooksMasterDetailPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
