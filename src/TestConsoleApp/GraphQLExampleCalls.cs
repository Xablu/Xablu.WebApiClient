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
            var first = "411eecb9b8fc9f5ac0a";
            var second = "ff63e8135cb68b4f10116";
            var combinedB = first + second;
            var defaultHeaders = new Dictionary<string, string>
            {
                ["User-Agent"] = "xabluexampleuser",
                ["Authorization"] = $"Bearer {combinedB}"
            };

            var webApiClient = WebApiClientFactory.Get<IGitHubApi>("https://api.github.com", false, default, defaultHeaders);

            var requestForSingleUser = new Request<UserResponseModel>("(login: \"exampleuserxablu\")");

            var result = await webApiClient.SendQueryAsync(requestForSingleUser);
            var serializedObject = JsonConvert.SerializeObject(result);

            return serializedObject;
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
