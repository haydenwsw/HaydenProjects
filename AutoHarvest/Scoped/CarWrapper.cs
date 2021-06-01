using AutoHarvest.Models;
using AutoHarvest.Scrapers;
using AutoHarvest.Singleton;
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
        // headless browser for webscraping
        private readonly CefSharpHeadless HeadlessBrowser;

        public CarWrapper(CefSharpHeadless headlessbrowser)
        {
            HeadlessBrowser = headlessbrowser;
        }

        // get all the car listing from the websties asynchronously
        public async Task<List<Car>> getCarsAsync(FilterOptions filterOptions, Toggles toggles, int page)
        {
            // scrape carsales based on toggle
            Task<List<Car>> CarsalesCars;
            if (toggles.ToggleCarsales)
                CarsalesCars = Carsales.ScrapeCarsales(HeadlessBrowser.GetHtmlAsync, filterOptions, page);
            else
                CarsalesCars = Task.FromResult(new List<Car>());

            // scrape facebook marketplace based on toggle
            Task<List<Car>> FbMarketplaceCars;
            if (toggles.ToggleFBMarketplace)
                FbMarketplaceCars = FbMarketplace.ScrapeFbMarketplace(HeadlessBrowser.GetHtmlAsync, filterOptions, page);
            else
                FbMarketplaceCars = Task.FromResult(new List<Car>());

            // scrape gumtree based on toggle
            Task<List<Car>> GumtreeCars;
            if (toggles.ToggleGumtree)
                GumtreeCars = Gumtree.ScrapeGumtree(filterOptions, page);
            else
                GumtreeCars = Task.FromResult(new List<Car>());

            // await all the taks in parallel at once
            await Task.WhenAll(FbMarketplaceCars, CarsalesCars, GumtreeCars);

            // create the list to the lenght of all the lists
            var Cars = new List<Car>(CarsalesCars.Result.Count + GumtreeCars.Result.Count + FbMarketplaceCars.Result.Count);

            // combine all the lists
            Cars.AddRange(CarsalesCars.Result);
            Cars.AddRange(FbMarketplaceCars.Result);
            Cars.AddRange(GumtreeCars.Result);

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
