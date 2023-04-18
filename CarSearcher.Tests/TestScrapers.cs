using System;
using System.Collections.Generic;
using Xunit;
using System.Net.Http;
using CarSearcher.Models;
using System.Threading.Tasks;
using CarSearcher.Scrapers;
using CarSearcher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.IO;

namespace HaydenProjects.Tests
{
    public class TestScrapers
    {
        private readonly HttpClient HttpClient;
        private readonly STACefNetHeadless STACefNetHeadless;
        private readonly CarLookup CarLookup;
        private readonly CarSearcherConfig CarSearcherConfig;

        public TestScrapers()
        {
            HttpClient = new HttpClient();

            string startupPath = Environment.CurrentDirectory;
            string solutionPath = startupPath.Substring(0, startupPath.IndexOf("CarSearcher.Tests"));
            string projectPath = Path.Combine(solutionPath, "HaydenProjects");

            IConfigurationRoot root = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .Build();

            CarSearcherConfig = root.Get<CarSearcherConfig>();

            var options = Options.Create(CarSearcherConfig);

            STACefNetHeadless = new STACefNetHeadless(options, null);

            CarLookup = new CarLookup(options);
        }

        [Theory]
        [InlineData("Toyota", "Camery", (int)SortBy.PriceLowtoHigh, (int)Transmission.All, "", "", 1)]
        public async void ScrapeCarsales(string make, string model, int sortType, int transType, string priceMin, string priceMax, int page)
        {
            await Task.Delay(500);

            FilterOptions filterOptions = new FilterOptions(make, model, "", sortType, transType, priceMin, priceMax, true, true, true, page);

            Carsales carsales = new Carsales(STACefNetHeadless, CarLookup, CarSearcherConfig);

            List<Car> CarsalesCars = await carsales.ScrapeCars(filterOptions);

            Assert.NotEmpty(CarsalesCars);
        }

        [Theory]
        [InlineData("Toyota", "Camery", (int)SortBy.PriceLowtoHigh, (int)Transmission.All, "", "", 1)]
        public async void ScrapeFbMarketPlace(string make, string model, int sortType, int transType, string priceMin, string priceMax, int page)
        {
            await Task.Delay(500);

            FilterOptions filterOptions = new FilterOptions(make, model, "", sortType, transType, priceMin, priceMax, true, true, true, page);

            FbMarketplace fbMarketplace = new FbMarketplace(HttpClient, CarLookup, CarSearcherConfig);

            List<Car> FbMarketPlaceCars = await fbMarketplace.ScrapeCars(filterOptions);

            Assert.NotEmpty(FbMarketPlaceCars);
        }

        [Theory]
        [InlineData("Toyota", "Camery", (int)SortBy.PriceLowtoHigh, (int)Transmission.All, "", "", 1)]
        public async void ScrapeGumtree(string make, string model, int sortType, int transType, string priceMin, string priceMax, int page)
        {
            await Task.Delay(500);

            FilterOptions filterOptions = new FilterOptions(make, model, "", sortType, transType, priceMin, priceMax, true, true, true, page);

            Gumtree gumtree = new Gumtree(HttpClient, CarLookup, CarSearcherConfig);

            List<Car> GumtreeCars = await gumtree.ScrapeCars(filterOptions);

            Assert.NotEmpty(GumtreeCars);
        }
    }
}
