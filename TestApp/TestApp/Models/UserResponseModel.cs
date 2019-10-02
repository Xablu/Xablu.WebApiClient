using System;
using Newtonsoft.Json;
using Xablu.WebApiClient.Attributes;

namespace TestApp.Models
{
    public class UserResponseModel
    {
        [JsonProperty("users")]

        public System.Collections.Generic.List<User> Users { get; set; }

        [JsonProperty("user")]
        [QueryParameter(exclusiveWith: nameof(Users))]
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


