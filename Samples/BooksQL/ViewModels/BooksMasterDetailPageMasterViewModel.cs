using System;
using System.Collections.ObjectModel;
using BooksQL.Views;
using BooksQL.Views.Items;

namespace BooksQL.ViewModels
{
    public class BooksMasterDetailPageMasterViewModel
    {

        public ObservableCollection<MasterMenuItem> MenuItems { get; set; }

        public BooksMasterDetailPageMasterViewModel()
        {
            MenuItems = new ObservableCollection<MasterMenuItem>(new[]
            {
                    new MasterMenuItem { Id = 0, Title = "GraphQL", TargetType = typeof(CallSelectionPage) },
                    new MasterMenuItem { Id = 1, Title = "Refit" },
                });
        }

        //#region INotifyPropertyChanged Implementation
        //public event PropertyChangedEventHandler PropertyChanged;
        //void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    if (PropertyChanged == null)
        //        return;

        //    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        //#endregion
        // }
    }
}
