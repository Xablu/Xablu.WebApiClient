using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;


namespace Sample.Models
{
    public class TodoItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("userId")]
        public int UserId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("completed")]
        public bool Completed { get; set; }
    }
}
