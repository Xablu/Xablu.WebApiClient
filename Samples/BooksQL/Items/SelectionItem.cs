using System;
namespace BooksQL.Views.Items
{
    public class SelectionItem
    {
        public SelectionItem()
        {
            TargetType = typeof(CallSelectionPage);
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public Type TargetType { get; set; }

        // todo TEnum but can't use this yet?
        public object Call { get; set; }
    }
}
