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
        public int SortType { get; set; }
        public int TransType { get; private set; }

        public FilterOptions(int sorttype, int transtype)
        {
            SortType = sorttype;
            TransType = transtype;
        }
    }
}
