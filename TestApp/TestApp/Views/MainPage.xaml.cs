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
            var dict = new Dictionary<string, string>() {
                { "Authorization","Bearer " }
            };

            // dict.Add("Authorization", "99f736dd263bc004ae9b11c99bf42ccb547e7311");

            var graphqlTest = new GraphQLService("https://api.github.com/graphql", null);
            graphqlTest.Client.DefaultRequestHeaders.Add("User-Agent", "LukasThijs");
            graphqlTest.Client.DefaultRequestHeaders.Add("Authorization", "Bearer ");


            var query = "{ user {  createdAt   location   followers {  totalCount } }}";
            var query2 = "{ viewer { login } }";

            var responseModel = new ResponseModel() { User = new User() };
            //todo need help with request class
            var request = new Request<ResponseModel>(query, responseModel);
            var test = new GraphQLRequest();
            //var response = await graphqlTest.Client.SendQueryAsync();


            var response2 = await graphqlTest.Client.SendQueryAsync(query2);


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
