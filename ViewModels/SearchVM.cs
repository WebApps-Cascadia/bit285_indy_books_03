using System;
namespace IndyBooks.ViewModels
{
    public class SearchVM
    {
        //Properties needed for searching
        public String SKU { get; set; }
        public String Title { get; set; }
        public String Name { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }

        public Boolean Sale { get; set; }

    }
}
