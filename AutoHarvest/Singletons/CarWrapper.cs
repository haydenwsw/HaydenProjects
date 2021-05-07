using AutoHarvest.Models;
using AutoHarvest.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Singletons
{
    public class CarWrapper
    {
        // headless browser for facebook
        private CefSharpHeadless HeadlessBrowser = new CefSharpHeadless();

        // get all the car listing from the websties asynchronously
        public async Task<List<Car>> getCarsAsync(string searchTerm, int page, FilterOptions filterOptions)
        {
            // scrape all the websites
            var gumtreeCars = Gumtree.ScrapeGumtree(searchTerm, page, filterOptions);
            var FbMarketplaceCars = FbMarketplace.ScrapeFbMarketplace(HeadlessBrowser, searchTerm, page, filterOptions.TransType);

            // await all the taks in parallel at once
            await Task.WhenAll(gumtreeCars, FbMarketplaceCars);

            // create the list to the lenght of all the lists
            var Cars = new List<Car>(gumtreeCars.Result.Count + FbMarketplaceCars.Result.Count);

            // combine all the lists
            Cars.AddRange(gumtreeCars.Result);
            Cars.AddRange(FbMarketplaceCars.Result);

            // sort the list
            Sort(ref Cars, filterOptions.SortType);

            return Cars;
        }

        // sorts the list
        private void Sort(ref List<Car> Cars, int SortNum)
        {
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
