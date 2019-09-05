using System;
using Newtonsoft.Json;

namespace TestApp.Models
{

    public class ResponseModel
    {
        [JsonProperty("user")]
        public User User { get; set; }
    }

    public class User
    {
        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("location")]
        public object Location { get; set; }

        [JsonProperty("followers")]
        public Followers Followers { get; set; }
    }

    public class Followers
    {
        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }
    }
}
