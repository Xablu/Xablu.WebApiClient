using System.Collections.ObjectModel;
using BooksQL.Views;
using BooksQL.Views.Items;

namespace BooksQL.ViewModels
{
    public class BooksMasterDetailPageMasterViewModel : BaseViewModel
    { 
        public BooksMasterDetailPageMasterViewModel()
        {
            MenuItems = new ObservableCollection<MasterMenuItem>(new[]
            {
                new MasterMenuItem { Id = 0, Title = "GraphQL", IconSource ="graphql_icon.png", TargetType = typeof(CallSelectionPage) },
                new MasterMenuItem { Id = 1, Title = "Refit", IconSource="refit_icon_white.png" },
                new MasterMenuItem { Id = 2, Title = "Home", IconSource="home.png", TargetType = typeof(HomeView) }
            });
        }

        public ObservableCollection<MasterMenuItem> MenuItems { get; set; }
    }
}
