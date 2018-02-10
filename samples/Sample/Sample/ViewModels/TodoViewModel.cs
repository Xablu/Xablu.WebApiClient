using Sample.Helpers;
using Sample.Models;
using Sample.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
    public class TodoViewModel : NotifyPropertyChanged
    {
        private readonly TodoService _todoService = new TodoService();

        private IEnumerable<TodoItem> _todoItems;
        
        public IEnumerable<TodoItem> TodoItems
        {
            get => _todoItems;
            set => SetProperty(ref _todoItems, value);
        }

        public async Task LoadData()
        {
            TodoItems = await _todoService.GetTodoItems();
        }
    }
}
