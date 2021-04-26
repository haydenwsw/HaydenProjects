using AutoHarvest.Models;
using AutoHarvest.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.HelperFunctions
{
    public static class CarWrapper
    {
        // get all the car listing from the websties
        public static List<Car> getCars(string searchTerm, int page, FilterOptions filterOptions)
        {
            var gumtreeCars = Gumtree.ScrapeGumtree(searchTerm, page, filterOptions).Result;
            //var v = FbMarketPlace.ScrapeFbMarketPlace(searchTerm, page, TransNum).Result;

            // create the list to the lenght of all the lists
            var Cars = new List<Car>(gumtreeCars.Count);
            
            // combine all the lists
            Cars.AddRange(gumtreeCars);

            // sort the list
            Sort(ref Cars, filterOptions.SortType);

            return Cars;
        }

        // get all the car listing from the websties asynchronously
        public static async Task<List<Car>> getCarsAsync(string searchTerm, int page, FilterOptions filterOptions)
        {
            var Cars = new List<Car>();

            var gumtreeCars = await Gumtree.ScrapeGumtree(searchTerm, page, filterOptions);

            Cars.AddRange(gumtreeCars);

            Sort(ref Cars, filterOptions.SortType);

            return Cars;
        }

        private static void Sort(ref List<Car> Cars, int SortNum)
        {
            // sort list
            switch (SortNum)
            {
                case (int)SortTypes.PriceLowtoHigh:
                    Cars.Sort((x, y) => x.Price.CompareTo(y.Price));
                    return;
                case (int)SortTypes.PriceHightoLow:
                    Cars.Sort((y, x) => x.Price.CompareTo(y.Price));
                    return;
                case (int)SortTypes.KilometresLowtoHigh:
                    Cars.Sort((x, y) => x.KMs.CompareTo(y.KMs));
                    return;
                case (int)SortTypes.KilometresHightoLow:
                    Cars.Sort((y, x) => x.KMs.CompareTo(y.KMs));
                    return;
            }
        }
    }
}
