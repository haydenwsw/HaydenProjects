using System;
using System.Collections.Generic;
using Xunit;
using AutoHarvest.Scrapers;
using AutoHarvest.Models;

namespace AutoHarvest.Tests
{
    public class TestScrapers
    {
        [Theory]
        [InlineData("celica", 1, (int)SortTypes.PriceLowtoHigh, 0)]
        public async void ScrapeGumtree(string search, int page, int sortType, int transType)
        {
            FilterOptions filterOptions = new FilterOptions(sortType, transType);
            List<Car> GumtreeCars = await Gumtree.ScrapeGumtree(search, page, filterOptions);

            Assert.NotEmpty(GumtreeCars);
        }

        [Theory]
        [InlineData("celica", 1, 0)]
        public async void ScrapeFbMarketPlace(string search, int page, int transNum)
        {
            List<Car> FbMarketPlaceCars = await FbMarketPlace.ScrapeFbMarketPlace(search, page, transNum);

            Assert.NotEmpty(FbMarketPlaceCars);
        }

        [Theory]
        [InlineData("celica", 1, 0)]
        public async void ScrapeCarsales(string search, int page, int transNum)
        {
            List<Car> CarsalesCars = await Carsales.ScrapeCarsales(search, page, transNum);

            Assert.NotEmpty(CarsalesCars);
        }
    }
}
