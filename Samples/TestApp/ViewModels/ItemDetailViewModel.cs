using TestApp.Models.StarWarsAPI;

namespace TestApp.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Starships Item { get; set; }
        public ItemDetailViewModel(Starships item = null)
        {
            Title = item?.name;
            Item = item;
        }
    }
}
