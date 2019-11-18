using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TestApp.Models;
using TestApp.Services;
using Xablu.WebApiClient;
using Xablu.WebApiClient.Native;
using Xablu.WebApiClient.Services.GraphQL;

namespace TestConsoleApp
{
    public class GraphQLExampleCalls
    {
        public GraphQLExampleCalls()
        {

        }

        public static async Task<string> GraphqlAsync()
        {
            var defaultHeaders = new Dictionary<string, string>
            {
                ["User-Agent"] = "ExampleUser",
                ["Authorization"] = "Bearer "
            };
            var webApiClient = WebApiClientFactory.Get<IGitHubApi>("https://api.github.com", false, default, defaultHeaders);

            var requestForSingleUser = new Request<UserResponseModel>(null, "(login: ExampleUser)");

            var requestForUsersList = new Request<UsersResponseModel>();

            await webApiClient.SendQueryAsync(requestForSingleUser);

            return requestForSingleUser.ToString();
        }

        static void ConvertJson(string json)
        {
            JObject parsed = JObject.Parse(json);

            foreach (var pair in parsed)
            {
                Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }
        }
    }
}
