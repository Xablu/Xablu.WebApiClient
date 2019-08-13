using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fusillade;
using Newtonsoft.Json;
using Refit;
using Xablu.WebApiClient.HttpExtensions;

namespace Xablu.WebApiClient.Client
{
    public interface IRefit
    {
        [Get("{path}")]
        Task GetTask(string path);

        [Post("{path}")]
        Task PostTask(string path);

        [Put("{path}")]
        Task PutTask(string path);

        [Patch("{path}")]
        Task PatchTask(string path);

        [Delete("{path}")]
        Task DeleteTask(string path);
    }

    // below wip

    public class Response
    {
        [JsonProperty("container")]
        public DataContainer Container { get; set; }
    }

    public class DataContainer
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("ids")]
        public IList<string> Ids { get; set; }
    }
}