using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CarSearcher.ExtensionFunctions;

namespace CarSearcher.Models
{
    public enum SortBy
    {
        PriceLowtoHigh,
        PriceHightoLow,
        Recent,
        KilometresLowtoHigh,
        KilometresHightoLow
    }

    public enum Transmission
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
        [MaxLength(25)]
        public string Make { get; private set; }

        [MaxLength(25)]
        public string Model { get; private set; }

        [MaxLength(100)]
        public string SearchTerm { get; private set; }

        public int SortBy { get; private set; }

        public int Transmission { get; private set; }

        [MaxLength(10)]
        public string MinPrice { get; private set; }

        [MaxLength(10)]
        public string MaxPrice { get; private set; }

        public bool ToggleCarsales { get; private set; }

        public bool ToggleFBMarketplace { get; private set; }

        public bool ToggleGumtree { get; private set; }

        public int PageNumber { get; private set; }

        public FilterOptions(string make, string model, string searchterm, int sorttype, int transtype, string pricemin, string pricemax, bool togglecarsales, bool togglefbmarketplace, bool togglegumtree, int pagenumber)
        {
            Make = make;
            Model = model;
            SearchTerm = searchterm;
            SortBy = Math.Clamp(sorttype, 0, 4);
            Transmission = Math.Clamp(transtype, 0, 2);
            MinPrice = pricemin.LeaveOnlyNumbers();
            MaxPrice = pricemax.LeaveOnlyNumbers();
            ToggleCarsales = togglecarsales;
            ToggleFBMarketplace = togglefbmarketplace;
            ToggleGumtree = togglegumtree;
            PageNumber = Math.Clamp(pagenumber, 1, 10);
        }

        public override string ToString()
        {
            return $"Make: {Make}, Model: {Model}, Search: {SearchTerm}, Sort: {(SortBy)SortBy}, Trans: {(Transmission)Transmission}, Min: {MinPrice}, Max: {MaxPrice}, Carsales: {ToggleCarsales}, FbMarketplace: {ToggleFBMarketplace}, Gumtree: {ToggleGumtree}, Page: {PageNumber}";
        }

        public T GetSort<T>(T[] sortArr)
        {
            return sortArr[SortBy];
        }

        public T GetTrans<T>(T[] transArr)
        {
            return transArr[Transmission];
        }
    }
}
