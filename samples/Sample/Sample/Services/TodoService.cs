using Fusillade;
using Newtonsoft.Json;
using Sample.Models;
using Sample.Resolvers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xablu.WebApiClient;

namespace Sample.Services
{
    public class TodoService
    {
        private readonly IRestApiClient _restApiClient;

        public TodoService()
        {
            _restApiClient = CrossRestApiClient.Current;
        }

        public async Task<IEnumerable<TodoItem>> GetTodoItems()
        {
            var url = "todos";

            var result = await _restApiClient.GetAsync<IEnumerable<TodoItem>>(
                Priority.UserInitiated,
                url,
                httpResponseResolver: new TestResolver(new JsonSerializer())).ConfigureAwait(false);

            return result.Content;
        }
    }
}
