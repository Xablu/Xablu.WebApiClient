using System;

namespace BooksQL.Views.Items
{
    public class MasterMenuItem
    {
        public MasterMenuItem()
        {
            TargetType = typeof(BooksMasterDetailPageDetail);
        }



        public int Id { get; set; }
        public string Title { get; set; }


        public Type TargetType { get; set; }
    }
}

