using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TestApp.Models.StarWarsAPI;
using TestApp.Services;
using TestApp.ViewModels;
using Xablu.WebApiClient;

namespace TestConsoleApp
{
    class Program
    {
        static IWebApiClient<IStarwarsApi> _webApiClient;
        static List<Starships> items = new List<Starships>();

        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            _webApiClient = WebApiClientFactory.Get<IStarwarsApi>("https://swapi.co/api", false, () => new SampleHttpClientHandler());
            await ServiceMenu();
        }

        static async Task ServiceMenu()
        {
            Console.WriteLine("WebApiClient test console");
            Console.WriteLine("------------------------\n");
            Console.WriteLine("Type a number to choose the API service, and then press Enter");
            Console.WriteLine("\t1 - Refit");
            Console.WriteLine("\t2 - GraphQL");
            Console.Write("Your option? ");

            await CallMenu();
        }
        static async Task CallMenu()
        {
            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Type a number to choose a randomised example Refitcall, and then press Enter");
                    Console.WriteLine("\t1 - GET");
                    Console.WriteLine("\t2 - PUT");
                    Console.WriteLine("\t2 - POST");
                    Console.WriteLine("\t2 - DELETE");
                    Console.Write("Your option? ");
                    await PrintCall();
                    break;
                case "2":
                    Console.WriteLine("Type a number to choose the randomised exmaple call, and then press Enter");
                    Console.WriteLine("\t1 - GET");
                    Console.WriteLine("\t2 - PUT");
                    Console.WriteLine("\t2 - POST");
                    Console.WriteLine("\t2 - DELETE");
                    Console.Write("Your option? ");
                    await PrintCall();
                    break;
                default:
                    await ServiceMenu();
                    break;
            }
        }

        static async Task PrintCall()
        {
            BaseViewModel model = new BaseViewModel();
            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Wait for your result...");
                    try
                    {
                        var items = await GetStarShipItemsAsync(true);
                        Convert(items.ToString());
                    }
                    catch (ArgumentException aex)
                    {
                        Console.WriteLine($"Caught ArgumentException: {aex.Message}");
                    }
                    break;
                default:
                    await ServiceMenu();
                    break;
            }
        }

        static async Task<IEnumerable<Starships>> GetStarShipItemsAsync(bool forceRefresh = false)
        {
            await _webApiClient.Call(
                    (service) => service.GetTask());

            await _webApiClient.Call(
                    (service) => service.GetTask(),
                    Xablu.WebApiClient.Enums.Priority.UserInitiated,
                    2,
                    (ex) => true,
                    60);

            var result = await _webApiClient.Call<ApiResponse<List<Starships>>>(
                    (service) => service.GetStarships(),
                    Xablu.WebApiClient.Enums.Priority.UserInitiated,
                    2,
                    (ex) => true,
                    60);

            return await Task.FromResult(items);
        }

        static void Convert(string json)
        {
            JObject parsed = JObject.Parse(json);

            foreach (var pair in parsed)
            {
                Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }
        }
    }
}
