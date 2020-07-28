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

        // get all the car listing from the websties
        public static List<Car> getCars(string searchTerm, uint page, uint SortNum, uint TransNum)
        {
            var Cars = new List<Car>();

            var gumtreeCars = Gumtree.ScrapeGumtree(searchTerm, page, SortNum, TransNum).Result;

            Cars.AddRange(gumtreeCars);

            // Sort(Cars, SortNum)

            return Cars;
        }

        // get all the car listing from the websties asynchronously
        public static async Task<List<Car>> getCarsAsync(string searchTerm, uint page, uint SortNum, uint TransNum)
        {
            var Cars = new List<Car>();

            return Cars;
        }

        private static List<Car> Sort(List<Car> Cars, uint SortNum)
        {
            // sort list
            switch (SortNum)
            {
                case (uint)SortTypes.PriceLowtoHigh:
                    Cars.Sort((x, y) => x.Price.CompareTo(y.Price));
                    return Cars;
                case (uint)SortTypes.PriceHightoLow:
                    Cars.Sort((y, x) => x.Price.CompareTo(y.Price));
                    return Cars;
                case (uint)SortTypes.KilometresLowtoHigh:
                    Cars.Sort((x, y) => x.KMs.CompareTo(y.KMs));
                    return Cars;
                case (uint)SortTypes.KilometresHightoLow:
                    Cars.Sort((y, x) => x.KMs.CompareTo(y.KMs));
                    return Cars;
            }

            return Cars;
        }
    }
}
