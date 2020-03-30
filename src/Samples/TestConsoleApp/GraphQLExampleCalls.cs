using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TestApp.Services;
using Xablu.WebApiClient;
using Xablu.WebApiClient.Attributes;
using Xablu.WebApiClient.Services.GraphQL;

namespace TestConsoleApp
{
    public class GraphQLExampleCalls
    {
        public GraphQLExampleCalls()
        {

        }

        public static async Task<string> GraphQLQueryAsync()
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

            var requestForSingleUser = new QueryRequest<UserResponseModel>("(login: \"exampleuserxablu\")");

            var result = await webApiClient.SendQueryAsync(requestForSingleUser);
            var serializedObject = JsonConvert.SerializeObject(result);

            return serializedObject;
        }

        public static async Task<string> GraphQLMutationAsync()
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

            var testModel = new ChangeUserInputVariables
            {
                ClientMutationId = "101010101",
                Message = "The mutation has succeeded"
            };

            // Example of a written query:
            // var mutationQuery = @" mutation ($changeUserStatusInputModel: ChangeUserStatusInput!) { changeUserStatus(input: $changeUserStatusInputModel){ clientMutationId, status { message }}}";
            var mutationRequest = new MutationRequest<ChangeStatusMutationResponseModel>("changeUserStatus", "input", testModel);

            var result = await webApiClient.SendMutationAsync(mutationRequest);
            var serializedObject = JsonConvert.SerializeObject(result);

            return serializedObject;
        }
    }

    public class UsersResponseModel
    {
        public List<User> Users { get; set; }
    }

    public class UserResponseModel
    {
        public User User { get; set; }
    }

    public class User
    {
        public DateTimeOffset CreatedAt { get; set; }

        public string Location { get; set; }

        public Followers Followers { get; set; }
    }

    public class Followers
    {
        public long TotalCount { get; set; }
    }
    
    [VariableInputType("ChangeUserStatusInput")]
    public class ChangeUserInputVariables
    {
        public string ClientMutationId { get; set; }

        public string Message { get; set; } 
    }

    public class ChangeStatusMutationResponseModel
    {
        public ChangeUserStatusPayload ChangeUserStatus { get; set; }
    }

    public class ChangeUserStatusPayload
    {
        public string ClientMutationId { get; set; }

        public Status Status { get; set; }
    }

    public class Status
    {
        public string Message { get; set; }
    }
}
