using CarSearcher;
using CarSearcher.Models;
using CarSearcher.Scrapers;
using HaydenProjects.Pages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HaydenProjects.Singletons
{
    /// <summary>
    /// Wrappes all the scrapers listings into one list and sorts accordingly
    /// all listings are from NSW area
    /// </summary>
    public class CarWrapper
    {
        // data classes
        private readonly CarSearcherConfig CarSearcherConfig;

        // the webscraper classes
        private readonly Carsales Carsales;
        private readonly FbMarketplace FbMarketplace;
        private readonly Gumtree Gumtree;

        private readonly Random Rng;
        private readonly ILogger<CarWrapper> Logger;

        public CarWrapper(IHttpClientFactory httpclientfactory, STACefNetHeadless cefnetheadless, IOptions<CarSearcherConfig> carsearcherconfig, CarLookup carlookup, Random random, ILoggerFactory loggerFactory)
        {
            CarSearcherConfig = carsearcherconfig.Value;

            Carsales = new Carsales(cefnetheadless, carlookup, CarSearcherConfig, loggerFactory.CreateLogger<Carsales>());
            FbMarketplace = new FbMarketplace(httpclientfactory.CreateClient(), carlookup, CarSearcherConfig, loggerFactory.CreateLogger<FbMarketplace>());
            Gumtree = new Gumtree(httpclientfactory.CreateClient(), carlookup, CarSearcherConfig, loggerFactory.CreateLogger<Gumtree>());

            Rng = random;
            Logger = loggerFactory.CreateLogger<CarWrapper>();
        }

        /// <summary>
        /// Get all the car listing from the websties asynchronously
        /// </summary>
        /// <param name="filterOptions"></param>
        /// <returns></returns>
        public async Task<List<Car>> GetCarsAsync(FilterOptions filterOptions)
        {
            // scrape carsales based on toggle
            Task<List<Car>> carsalesCars;
            if (filterOptions.ToggleCarsales && CarSearcherConfig.EnableCarsales)
                carsalesCars = Carsales.ScrapeCars(filterOptions);
            else
                carsalesCars = Task.FromResult<List<Car>>(null);

            // scrape facebook marketplace based on toggle
            Task<List<Car>> fbMarketplaceCars;
            if (filterOptions.ToggleFBMarketplace && CarSearcherConfig.EnableFbMarketplace)
                fbMarketplaceCars = FbMarketplace.ScrapeCars(filterOptions);
            else
                fbMarketplaceCars = Task.FromResult<List<Car>>(null);

            // scrape gumtree based on toggle
            Task<List<Car>> gumtreeCars;
            if (filterOptions.ToggleGumtree && CarSearcherConfig.EnableGumtree)
                gumtreeCars = Gumtree.ScrapeCars(filterOptions);
            else
                gumtreeCars = Task.FromResult<List<Car>>(null);

            List<Car>[] returnCars = await Task.WhenAll(carsalesCars, fbMarketplaceCars, gumtreeCars);

            // combine all the lists
            List<Car> cars = returnCars.SelectMany(x => x ?? new List<Car>()).ToList();

            // sort the list on the backend because no frontend rendering engine
            Sort(cars, filterOptions.SortBy);

            return cars;
        }

        /// <summary>
        /// Get's a random car from Gumtree for the user to guess
        /// Carsales blocks you for bot activity (going to random pages)
        /// FbMarketplace is a pain to work with
        /// </summary>
        /// <param name="body"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<GuessCar> GuessCarAsync(string body, string company, bool isEasyMode)
        {
            try
            {
                // get a random car from gumtree
                return await Gumtree.GetRandomCar(body, company, isEasyMode, Rng);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "query body: {body}, company: {company} isEasyMode: {isEasyMode}", body, company, isEasyMode);
                return null;
            }
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
