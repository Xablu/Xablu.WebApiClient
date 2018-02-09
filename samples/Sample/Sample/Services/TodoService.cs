using Fusillade;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xablu.WebApiClient;
using Xablu.WebApiClient.Abstractions;

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

            var result = await _restApiClient.GetAsync<IEnumerable<TodoItem>>(Priority.UserInitiated, url).ConfigureAwait(false);

            return result.Content;
        }
    }
}
