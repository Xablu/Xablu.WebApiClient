using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TestApp.Models;
using TestApp.Models.StarWarsAPI;
using TestApp.Services;
using Xablu.WebApiClient;
using Xablu.WebApiClient.Enums;
using Xablu.WebApiClient.Native;

namespace TestConsoleApp
{
    public class RefitExampleCalls
    {
        // 2 ways to check an object. One from json printed, the other an object from the api
        // the first way is more modular and works for any json, the second more adjusted for SWAPI
        public static async Task<IEnumerable<Starships>> GetStarShipItemsAsync(bool forceRefresh = false)
        {
            IWebApiClient<IStarwarsApi> _webApiClient = WebApiClientFactory.Get<IStarwarsApi>("https://swapi.co/api", true);
            var jsonresult = await _webApiClient.Call(
                    (service) => service.GetTask(),
                    Priority.UserInitiated,
                    2,
                    (ex) => true,
                    60);

            var result = await _webApiClient.Call(
                    (service) => service.GetStarships(),
                    Priority.UserInitiated,
                    2,
                    (ex) => true,
                    60);

            // checking the get call with a quick timeout
            try
            {
                var timeoutresult = await _webApiClient.Call(
                    (service) => service.GetTask(),
                    Priority.UserInitiated,
                    2,
                    (ex) => true,
                    3);
            }
            catch (Exception e)
            {
                Console.WriteLine("This call is longer then 3 seconds, but will display anyway of working correctly");
            }

            ConvertJson(jsonresult);
            return result.Results;
        }

        // test post call
        public static async Task<string> PostRawPostmanEcho(bool forceRefresh = false)
        {
            IWebApiClient<IPostmanEcho> webApiClient = WebApiClientFactory.Get<IPostmanEcho>("https://postman-echo.com/", false);

            PostObject postObject = new PostObject();
            postObject.testName = "hello world";
            postObject.testDate = DateTime.Now.ToString();

            var postObjectResult = await webApiClient.Call(
                (service) => service.PostObject(postObject),
                Priority.UserInitiated,
                2,
                (ex) => true,
                60);

            var postResult = await webApiClient.Call(
                    (service) => service.Post("test"),
                    UserInitiated,
                    2,
                    (ex) => true,
                    60);

            Console.WriteLine("\t Post string: ");
            ConvertJson(postResult);
            Console.WriteLine("\t Post object: ");
            ConvertJson(postObjectResult);
            return postResult;
        }

        // test login and authorization methods
        public static async Task<string> AuthenticatePostmanEcho(bool forceRefresh = false)
        {
            // this is how you do a basic auth, todo: how to convert to dictionary headers, i would use value?

            //var authData = string.Format("{0}:{1}", "postman", "password");
            //var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            //AuthenticationHeaderValue value = new AuthenticationHeaderValue(authHeaderValue, authData); 

            // this basic auth way is working now but less clear imo
            IDictionary<string, string> authValues = new Dictionary<string, string>
            {
                {"postman", "password" }
            };

            // OAuth isn't working, how do you add the headers for these
            //IDictionary<string, string> oAuthValues = new Dictionary<string, string>
            //{
            //    {"oauth_consumer_key", "D+EdQ-RKCGzna7bv9YD57c-%@2Nu7" },
            //    {"oauth_signature_method", "HMAC-SHA1" },
            //    {"oauth_timestamp", "1472121261"},
            //    {"oauth_nonce", "oauth_nonce" },
            //    {"oauth_version", "1.0" },
            //    {"oauth_signature", "s0rK92Myxx7ceUBVzlMaxiiXU00"}
            //};

            IWebApiClient<IPostmanEcho> basicAuthWebApiClient = WebApiClientFactory.Get<IPostmanEcho>("https://postman-echo.com/", false,  defaultHeaders: authValues);
            //IWebApiClient<IPostmanEcho> oAuthWebApiClient = WebApiClientFactory.Get<IPostmanEcho>("https://postman-echo.com/", false, () => new SampleHttpClientHandler(), oAuthValues);

            var basicAuthResult = await basicAuthWebApiClient.Call(
                (service) => service.BasicAuth(),
                Xablu.WebApiClient.Enums.Priority.UserInitiated,
                2,
                (ex) => true,
                60);

            //var oAuthResult = await oAuthWebApiClient.Call(
            //    (service) => service.OAuth(),
            //    Xablu.WebApiClient.Enums.Priority.UserInitiated,
            //    2,
            //    (ex) => true,
            //    60);

            Console.WriteLine("\t basic auth: ");
            ConvertJson(basicAuthResult);
            //Console.WriteLine("\t oauth: ");
            //ConvertJson(oAuthResult);
            return basicAuthResult;
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

Down here is an example call for connecting with a Web API service through Refit:

Task<TResult> Call<TResult>(Func<T, Task<TResult>> operation, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout);

async Task<IEnumerable<MyModel>> GetStarShipItemsAsync(bool forceRefresh = false)
{
    IWebApiClient<IRefitInteraface> webApiClient = WebApiClientFactory.Get<IRefitInteraface>("baseURL", bool defaultHeaders: true);
    var jsonresult = await webApiClient.Call(
            (IRefitInterafaceService) => IRefitInterafaceService.GetTask(),
            (Polly.Priority) Priority.UserInitiated,
            (retryCount) 2,
            (shouldRetry) => true,
            (timeout) 60);
}