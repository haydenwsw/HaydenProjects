using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Models
{
    public enum SortTypes
    {
        PriceLowtoHigh,
        PriceHightoLow,
        Recent,
        KilometresLowtoHigh,
        KilometresHightoLow
    }

    public enum TransTypes
    {
        All,
        Manuel,
        Automatic
    }

    public struct FilterOptions
    {
        public string SearchTerm { get; private set; }
        public int SortType { get; set; }
        public int TransType { get; private set; }

        public FilterOptions(string searchterm, int sorttype, int transtype)
        {
            SearchTerm = searchterm;
            SortType = sorttype;
            TransType = transtype;
        }
    }
}
