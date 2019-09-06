using System;
using Newtonsoft.Json;

namespace TestApp.Models
{

    public class ResponseModel
    {
        [JsonProperty("viewer")]
        public Viewer Viewer { get; set; }
    }

    public class Viewer
    {
        [JsonProperty("login")]
        public string Login { get; set; }
    }
}
