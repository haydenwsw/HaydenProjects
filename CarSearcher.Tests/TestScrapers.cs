using System;
using System.Collections.Generic;
using Xunit;
using System.Net.Http;
using CarSearcher.Models;
using System.Threading.Tasks;

namespace HaydenProjects.Tests
{
    public class TestScrapers
    {
        private readonly HttpClient HttpClient;

        public TestScrapers()
        {
            HttpClient = new HttpClient();
        }

        [Theory]
        [InlineData("Toyoda", "Camery", (int)SortBy.PriceLowtoHigh, (int)Transmission.All, "", "", 1)]
        public async void ScrapeCarsales(string make, string model, int sortType, int transType, string priceMin, string priceMax, int page)
        {
            await Task.Delay(500);

            FilterOptions filterOptions = new FilterOptions(make, model, "", sortType, transType, priceMin, priceMax, true, true, true, page);

            //List<Car> CarsalesCars = await Carsales.ScrapeCarsales(search, page, transNum);

            //Assert.NotEmpty(CarsalesCars);
        }

        [Theory]
        [InlineData("Toyoda", "Camery", (int)SortBy.PriceLowtoHigh, (int)Transmission.All, "", "", 1)]
        public async void ScrapeFbMarketPlace(string make, string model, int sortType, int transType, string priceMin, string priceMax, int page)
        {
            await Task.Delay(500);

            FilterOptions filterOptions = new FilterOptions(make, model, "", sortType, transType, priceMin, priceMax, true, true, true, page);

            //List<Car> FbMarketPlaceCars = await FbMarketplace.ScrapeFbMarketplace(null, search, page, transNum);

            //Assert.NotEmpty(FbMarketPlaceCars);
        }

        [Theory]
        [InlineData("Toyoda", "Camery", (int)SortBy.PriceLowtoHigh, (int)Transmission.All, "", "", 1)]
        public async void ScrapeGumtree(string make, string model, int sortType, int transType, string priceMin, string priceMax, int page)
        {
            await Task.Delay(500);

            FilterOptions filterOptions = new FilterOptions(make, model, "", sortType, transType, priceMin, priceMax, true, true, true, page);

            //Gumtree gumtree = new Gumtree(new HttpClient(), );

            //List<Car> GumtreeCars = await Gumtree.ScrapeGumtree(filterOptions);

            //Assert.NotEmpty(GumtreeCars);
        }
    }
}
