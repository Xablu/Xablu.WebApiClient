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
        string postResult;

        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            await CallMenu();
        }

        static async Task CallMenu()
        {
            Console.WriteLine("WebApiClient test console");
            Console.WriteLine("------------------------\n");
            Console.WriteLine("Type a number to choose the API service, and then press Enter");
            Console.WriteLine("\t1 - Refit");
            Console.WriteLine("\t2 - GraphQL");
            Console.WriteLine("\t3 - Quit Program");
            Console.Write("Your option? ");

            await ServiceMenu();
        }

        static async Task ServiceMenu()
        {
            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Type a number to choose a randomised example Refitcall, and then press Enter");
                    Console.WriteLine("\t1 - GET");
                    Console.WriteLine("\t2 - POST");
                    Console.WriteLine("\t3 - AUTHENTICATE");
                    Console.WriteLine("\t4 - Back to Home Menu");
                    Console.Write("Your option? ");
                    await PrintRefitCall();
                    break;
                case "2":
                    Console.WriteLine("Type a number to choose the randomised exmaple call, and then press Enter");
                    Console.WriteLine("\t1 - GET");
                    // Console.WriteLine("\t2 - PUT");
                    // Console.WriteLine("\t2 - POST");
                    //  Console.WriteLine("\t2 - DELETE");
                    Console.WriteLine("\t2 - Back to Home Menu");
                    Console.Write("Your option? ");
                    await PrintGraphqlCall();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    await CallMenu();
                    break;
            }
        }

        static async Task PrintRefitCall()
        {
            Console.WriteLine("Wait for your result...");
            switch (Console.ReadLine())
            {
                case "1":
                    try
                    {
                        var _items = await RefitExampleCalls.GetStarShipItemsAsync(true);
                        items = _items as List<Starships>;
                    }
                    catch (ArgumentException aex)
                    {
                        Console.WriteLine($"Caught ArgumentException: {aex.Message}");
                    }
                    break;
                case "2":
                    try
                    {
                        string postResult = await RefitExampleCalls.PostRawPostmanEcho(true);
                    }
                    catch (ArgumentException aex)
                    {
                        Console.WriteLine($"Caught ArgumentException: {aex.Message}");
                    }
                    break;
                case "3":
                    try
                    {
                        string postResult = await RefitExampleCalls.AuthenticatePostmanEcho(true);
                    }
                    catch (ArgumentException aex)
                    {
                        Console.WriteLine($"Caught ArgumentException: {aex.Message}");
                    }
                    break;
                default:
                    await CallMenu();
                    break;
            }
            await ServiceMenu();
        }

        static async Task PrintGraphqlCall()
        {
            switch (Console.ReadLine())
            {
                case "1":
                    try
                    {
                        var model = await GraphQLExampleCalls.GraphqlAsync();
                        Console.WriteLine(model);
                    }
                    catch (ArgumentException aex)
                    {
                        Console.WriteLine($"Caught ArgumentException: {aex.Message}");
                    }
                    break;
                default:
                    await CallMenu();
                    break;
            }
        }
    }
}