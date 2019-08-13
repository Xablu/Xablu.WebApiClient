using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Models.StarWarsAPI;
using Xablu.WebApiClient;
using Xablu.WebApiClient.Client;

namespace TestApp.Services
{
    public class MockDataStore : IDataStore<Starships>
    {
        List<Starships> items;
        public IRefitClient refitClient = new RefitClient("https://swapi.co/api/");
        public IRefitService<IStarwarsApi> refitService = new RefitService<IStarwarsApi>("https://swapi.co/api/");

        public MockDataStore()
        {
            items = new List<Starships>();
            var mockItems = new List<Starships>
            {
                new Starships { Id = Guid.NewGuid().ToString(), name = "First starship", model="This is an item description." },
                new Starships { Id = Guid.NewGuid().ToString(), name = "Second starship", model="This is an item description." },
                new Starships { Id = Guid.NewGuid().ToString(), name = "Third starship", model="This is an item description." },
                new Starships { Id = Guid.NewGuid().ToString(), name = "Fourth starship", model="This is an item description." },
                new Starships { Id = Guid.NewGuid().ToString(), name = "Fifth starship", model="This is an item description." },
                new Starships { Id = Guid.NewGuid().ToString(), name = "Sixth starship", model="This is an item description." }
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(Starships item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Starships item)
        {
            var oldItem = items.Where((Starships arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Starships arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Starships> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Starships>> GetItemsAsync(bool forceRefresh = false)
        {
            var clientResult = refitClient.RefitService.UserInitiated.GetTask("starships/");
            var serviceResult = refitService.UserInitiated.GetTask();

            //give either result back with own refit interface and endpoints or either give endpoint and let our package handle it for you

            return await Task.FromResult(items);
        }
    }
}