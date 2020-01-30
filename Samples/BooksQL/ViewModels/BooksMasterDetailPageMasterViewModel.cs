using System;
using System.Collections.ObjectModel;
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
                    new MasterMenuItem { Id = 0, Title = "Page 1" },
                    new MasterMenuItem { Id = 1, Title = "Page 2" },
                    new MasterMenuItem { Id = 2, Title = "Page 3" },
                    new MasterMenuItem { Id = 3, Title = "Page 4" },
                    new MasterMenuItem { Id = 4, Title = "Page 5" },
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
