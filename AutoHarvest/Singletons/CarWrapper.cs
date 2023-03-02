using AutoHarvest.Models;
using AutoHarvest.Models.Json;
using AutoHarvest.Pages;
using AutoHarvest.Scrapers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Singletons
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

        // data classes
        private readonly CarLookup CarLookup;
        private readonly CarFinder CarFinder;

        public CarWrapper(Carsales carsales, FbMarketplace fbmarketplace, Gumtree gumtree, CarLookup carlookup, IOptions<CarFinder> carfinder)
        {
            Carsales = carsales;
            FbMarketplace = fbmarketplace;
            Gumtree = gumtree;
            CarLookup = carlookup;
            CarFinder = carfinder.Value;
        }

        // get all the car listing from the websties asynchronously
        public async Task<List<Car>> GetCarsAsync(FilterOptions filterOptions)
        {
            // scrape carsales based on toggle
            Task<List<Car>> carsalesCars;
            if (filterOptions.ToggleCarsales && CarFinder.EnableCarsales)
                carsalesCars = Carsales.ScrapeCars(filterOptions, CarLookup);
            else
                carsalesCars = Task.FromResult(new List<Car>());

            // scrape facebook marketplace based on toggle
            Task<List<Car>> fbMarketplaceCars;
            if (filterOptions.ToggleFBMarketplace && CarFinder.EnableFbMarketplace)
                fbMarketplaceCars = FbMarketplace.ScrapeCars(filterOptions, CarLookup);
            else
                fbMarketplaceCars = Task.FromResult(new List<Car>());

            // scrape gumtree based on toggle
            Task<List<Car>> gumtreeCars;
            if (filterOptions.ToggleGumtree && CarFinder.EnableGumtree)
                gumtreeCars = Gumtree.ScrapeCars(filterOptions, CarLookup);
            else
                gumtreeCars = Task.FromResult(new List<Car>());

            // await all the tasks in parallel at once
            List<Car>[] returnCars = await Task.WhenAll(carsalesCars, fbMarketplaceCars, gumtreeCars);

            // combine all the lists
            List<Car> cars = returnCars.SelectMany(x => x).ToList();

            // sort the list on the backend because no frontend rendering engine
            Sort(cars, filterOptions.SortBy);

            return cars;
        }

        // sorts the list
        private static void Sort(List<Car> cars, int sortNum)
        {
            switch (sortNum)
            {
                case (int)SortBy.PriceLowtoHigh:
                    cars.Sort((x, y) => x.Price.CompareTo(y.Price));
                    return;
                case (int)SortBy.PriceHightoLow:
                    cars.Sort((y, x) => x.Price.CompareTo(y.Price));
                    return;
                case (int)SortBy.KilometresLowtoHigh:
                    cars.Sort((x, y) => x.KMs.CompareTo(y.KMs));
                    return;
                case (int)SortBy.KilometresHightoLow:
                    cars.Sort((y, x) => x.KMs.CompareTo(y.KMs));
                    return;
            }
        }
    }
}
