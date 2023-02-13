using AutoHarvest.Models;
using AutoHarvest.Pages;
using AutoHarvest.Scrapers;
using AutoHarvest.Singleton;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Singleton
{
    /// <summary>
    /// Wrappes all the scrapers listings into one list and sorts accordingly
    /// all listings are from NSW area
    /// </summary>
    public class CarWrapper
    {
        // the webscraper classes
        private readonly Carsales Carsales;
        private readonly FbMarketplace FbMarketplace;
        private readonly Gumtree Gumtree;

        // headless browser for webscraping
        private readonly CefSharpHeadless HeadlessBrowser;

        public CarWrapper(Carsales carsales, FbMarketplace fbmarketplace, Gumtree gumtree, CefSharpHeadless headlessbrowser)
        {
            Carsales = carsales;
            FbMarketplace = fbmarketplace;
            Gumtree = gumtree;
            HeadlessBrowser = headlessbrowser;
        }

        // get all the car listing from the websties asynchronously
        public async Task<List<Car>> GetCarsAsync(FilterOptions FilterOptions)
        {
            // scrape carsales based on toggle
            Task<List<Car>> CarsalesCars;
            if (FilterOptions.ToggleCarsales)
                CarsalesCars = Carsales.ScrapeCarsales(HeadlessBrowser.CreateNewTab(), FilterOptions);
            else
                CarsalesCars = Task.FromResult(new List<Car>());

            // scrape facebook marketplace based on toggle
            Task<List<Car>> FbMarketplaceCars;
            if (FilterOptions.ToggleFBMarketplace)
                FbMarketplaceCars = FbMarketplace.ScrapeFbMarketplace(HeadlessBrowser.CreateNewTab(), FilterOptions);
            else
                FbMarketplaceCars = Task.FromResult(new List<Car>());

            // scrape gumtree based on toggle
            Task<List<Car>> GumtreeCars;
            if (FilterOptions.ToggleGumtree)
                GumtreeCars = Gumtree.ScrapeGumtree(FilterOptions);
            else
                GumtreeCars = Task.FromResult(new List<Car>());

            // await all the tasks in parallel at once
            await Task.WhenAll(CarsalesCars, FbMarketplaceCars, GumtreeCars);

            // create the list to the lenght of all the lists
            var Cars = new List<Car>(CarsalesCars.Result.Count + FbMarketplaceCars.Result.Count + GumtreeCars.Result.Count);

            // combine all the lists
            Cars.AddRange(CarsalesCars.Result);
            Cars.AddRange(FbMarketplaceCars.Result);
            Cars.AddRange(GumtreeCars.Result);

            // sort the list
            Sort(ref Cars, FilterOptions.SortType);

            return Cars;
        }

        // sorts the list
        private static void Sort(ref List<Car> Cars, int SortNum)
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
