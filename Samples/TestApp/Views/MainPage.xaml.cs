using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using Newtonsoft.Json;
using TestApp.Models;
using TestApp.Services;
using Xablu.WebApiClient;
using Xablu.WebApiClient.Services.GraphQL;
using Xamarin.Forms;

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

        public async Task<string> GraphqlAsync()
        {
            var defaultHeaders = new Dictionary<string, string>
            {
                ["User-Agent"] = "LukasThijs",
                ["Authorization"] = "bearer insertbearer"
            };
            var webApiClient = WebApiClientFactory.Get<IGitHubApi>("https://api.github.com/", false, default, defaultHeaders);

            var requestForSingleUser = new Request<UserResponseModel>("(login: \"LukasThijs\")");
            var test = new GraphQLHttpClient("https://api.github.com/graphql");
            test.DefaultRequestHeaders.Add("User-Agent", "LukasThijs");
            test.DefaultRequestHeaders.Add("Authorization", "Bearer insertbearer");

            //todo call below works
            //    var a = await test.SendQueryAsync(requestForSingleUser);
            //todo call below does not work
            await webApiClient.SendQueryAsync<UserResponseModel>(requestForSingleUser);

            return null;
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

    public class UsersResponseModel
    {
        [JsonProperty("users")]
        public List<User> Users { get; set; }
    }
    public class UserResponseModel
    {
        [JsonProperty("user")]
        public User User { get; set; }
    }
    public class User
    {
        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("followers")]
        public Followers Followers { get; set; }
    }
    public class Followers
    {
        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }
    }
}
