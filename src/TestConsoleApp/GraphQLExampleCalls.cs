using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestApp.Services;
using Xablu.WebApiClient;
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
                ["User-Agent"] = "LukasThijs",
                ["Authorization"] = "Bearer e9da1446711b5529b1aabe27a21ae205897bcbde"
            };
            var webApiClient = WebApiClientFactory.Get<IGitHubApi>("https://api.github.com/", false, default, defaultHeaders);
            //8f9b136d6d9d84817752218dd3e7c16480407bf0
            var requestForSingleUser = new Request<UserResponseModel>("(login: \"LukasThijs\")");

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
