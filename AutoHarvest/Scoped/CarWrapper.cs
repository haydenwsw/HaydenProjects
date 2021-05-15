using AutoHarvest.Models;
using AutoHarvest.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Scoped
{
    /// <summary>
    /// Wrappes all the scrapers listings into one list a sort accordingly
    /// all listings are from NSW area
    /// </summary>
    public class CarWrapper
    {
        // headless browser for facebook
        private static CefSharpHeadless HeadlessBrowser = new CefSharpHeadless();

        // get all the car listing from the websties asynchronously
        public async Task<List<Car>> getCarsAsync(string searchTerm, int page, FilterOptions filterOptions)
        {
            // scrape all the websites
            //var CarsalesCars = Carsales.ScrapeCarsales(HeadlessBrowser, searchTerm, page, filterOptions.SortType);
            //var FbMarketplaceCars = FbMarketplace.ScrapeFbMarketplace(HeadlessBrowser, searchTerm, page, filterOptions.TransType);
            var GumtreeCars = Gumtree.ScrapeGumtree(searchTerm, page, filterOptions).Result;

            // await all the taks in parallel at once
            //await Task.WhenAll(GumtreeCars, FbMarketplaceCars, CarsalesCars);

            // create the list to the lenght of all the lists
            //var Cars = new List<Car>(GumtreeCars.Result.Count + FbMarketplaceCars.Result.Count + CarsalesCars.Result.Count);

            // combine all the lists
            //Cars.AddRange(GumtreeCars.Result);
            //Cars.AddRange(FbMarketplaceCars.Result);
            //Cars.AddRange(CarsalesCars.Result);

            // sort the list
            Sort(ref GumtreeCars, filterOptions.SortType);

            return GumtreeCars;
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
