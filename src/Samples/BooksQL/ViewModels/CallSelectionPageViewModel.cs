using System;
using System.Collections.ObjectModel;
using BooksQL.Views;
using BooksQL.Views.Items;

namespace BooksQL.ViewModels
{
    public class CallSelectionPageViewModel
    {

        public CallSelectionPageViewModel()
        {
            SelectionItems = new ObservableCollection<SelectionItem>
            {
                new SelectionItem() { Id = 0, Title = "Query", TargetType = typeof(QueryView)},
                new SelectionItem() { Id = 0, Title = "Mutation", TargetType = typeof(MutationView)}
            };
        }

        public ObservableCollection<SelectionItem> SelectionItems { get; set; }
    }
}
