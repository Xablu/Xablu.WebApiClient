using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TestApp.Models;
using Xablu.WebApiClient.Services.GraphQL;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Newtonsoft.Json;
using System.Linq;
using Xablu.WebApiClient;
using TestApp.Services;

namespace TestApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);




            Task.Run(async () => await GraphqlAsync());


        }

        private async Task GraphqlAsync()
        {
            var defaultHeaders = new Dictionary<string, string>
            {
                ["User-Agent"] = "LukasThijs",
                ["Authorization"] = "Bearer "
            };
            var webApiClient = WebApiClientFactory.Get<IGitHubApi>("https://api.github.com", false, () => new SampleHttpClientHandler(), defaultHeaders);
            var request = new Request<UserResponseModel>(null, new[] { "(login: LukasThijs)" });
              
            // TODO: Handle the result!
            await webApiClient.SendQueryAsync(request);
        }


        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Browse:
                        MenuPages.Add(id, new NavigationPage(new ItemsPage()));
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}
