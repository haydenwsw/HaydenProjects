using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoHarvest.HelperFunctions;

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

    /// <summary>
    /// model that holds the query from the user
    /// </summary>
    public class FilterOptions
    {
        [MaxLength(100)]
        public string SearchTerm { get; private set; }

        public int SortType { get; private set; }

        public int TransType { get; private set; }

        [MaxLength(10)]
        public string PriceMin { get; private set; }

        [MaxLength(10)]
        public string PriceMax { get; private set; }

        public bool ToggleCarsales { get; private set; }

        public bool ToggleFBMarketplace { get; private set; }

        public bool ToggleGumtree { get; private set; }

        public int PageNumber { get; private set; }

        public FilterOptions(string searchterm, int sorttype, int transtype, string pricemin, string pricemax, bool togglecarsales, bool togglefbmarketplace, bool togglegumtree, int pagenumber)
        {
            SearchTerm = searchterm;
            SortType = Math.Clamp(sorttype, 0, 4);
            TransType = Math.Clamp(transtype, 0, 2);
            PriceMin = pricemin.LeaveOnlyNumbers();
            PriceMax = pricemax.LeaveOnlyNumbers();
            ToggleCarsales = togglecarsales;
            ToggleFBMarketplace = togglefbmarketplace;
            ToggleGumtree = togglegumtree;
            PageNumber = Math.Clamp(pagenumber, 1, 10);
        }

        public override string ToString()
        {
            return $"Search: {SearchTerm}, Sort: {(SortTypes)SortType}, Trans: {(TransTypes)TransType}, Min: {PriceMin}, Max: {PriceMax}, Carsales: {ToggleCarsales}, FbMarketplace: {ToggleFBMarketplace}, Gumtree: {ToggleGumtree}, Page: {PageNumber}";
        }
    }
}
