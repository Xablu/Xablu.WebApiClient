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
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            await CallMenu();
        }

        static async Task CallMenu()
        {
            string option = string.Empty;
            do
            {
                try
                {
                    Console.WriteLine("WebApiClient test console");
                    Console.WriteLine("------------------------\n");
                    Console.WriteLine("Type a number to choose the API service, and then press Enter");
                    Console.WriteLine("\t1 - Refit");
                    Console.WriteLine("\t2 - GraphQL");
                    Console.WriteLine("\t3 - Quit Program");
                    Console.Write("Your option? ");

                    option = Console.ReadLine();

                    await ExecuteOption(option);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } while (option != "3");
        }

        static async Task ExecuteOption(string option)
        {
            switch (option)
            {
                case "1":
                    Console.WriteLine("Type a number to choose a randomised example Refitcall, and then press Enter");
                    Console.WriteLine("\t1 - GET");
                    Console.WriteLine("\t2 - POST");
                    Console.WriteLine("\t3 - AUTHENTICATE");
                    Console.WriteLine("\t4 - Back to Home Menu");
                    Console.Write("Your option? ");

                    var refitOption = Console.ReadLine();
                    await ExecuteRefitOption(refitOption);
                    break;

                case "2":
                    Console.WriteLine("Type a number to choose the randomised exmaple call, and then press Enter");
                    Console.WriteLine("\t1 - GET");
                    Console.WriteLine("\t2 - SendMutation");
                    // Console.WriteLine("\t2 - POST");
                    //  Console.WriteLine("\t2 - DELETE");
                    Console.WriteLine("\t3 - Back to Home Menu");
                    Console.Write("Your option? ");

                    var graphQLOption = Console.ReadLine();
                    await ExecuteGraphQLOption(graphQLOption); 
                    break;
                case "3":
                    Console.Write("Have a nice day!");
                    break;
                default:
                    Console.Write("Please enter a valid option");
                    break;
            }
        }

        static async Task ExecuteRefitOption(string refitOption)
        {
            Console.WriteLine("Wait for your result...");

            switch (refitOption)
            {
                case "1":
                    try
                    {
                        var _items = await RefitExampleCalls.GetStarShipItemsAsync(true);
                        List<Starships> items = _items as List<Starships>;
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
            } 
        }

        static async Task ExecuteGraphQLOption(string graphQLOption)
        {
            switch (graphQLOption)
            {
                case "1":
                    try
                    {
                        var model = await GraphQLExampleCalls.GraphQLQueryAsync();
                        Console.WriteLine(model);
                    }
                    catch (ArgumentException aex)
                    {
                        Console.WriteLine($"Caught ArgumentException: {aex.Message}");
                    }
                    break;

                case "2":
                    try
                    {
                        var model = await GraphQLExampleCalls.GraphQLMutationAsync();
                        Console.WriteLine(model);
                    }
                    catch (ArgumentException aex)
                    {
                        Console.WriteLine($"Caught ArgumentException: {aex.Message}");
                    }
                    break;
            }
        }
    }
}