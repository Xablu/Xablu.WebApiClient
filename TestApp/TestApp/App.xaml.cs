using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TestApp.Services;
using TestApp.Views;
using Xablu.WebApiClient;

namespace TestApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            RequestOptions.DefaultRequestOptions = new RequestOptions
            {
                Timeout = 5
            };
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
