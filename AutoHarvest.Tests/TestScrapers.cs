using System;
using System.Collections.Generic;
using Xunit;
using AutoHarvest.Scrapers;
using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;

namespace AutoHarvest.Tests
{
    public class TestScrapers
    {
        [Theory]
        [InlineData("celica", 1, (uint)CarWrapper.SortTypes.PriceLowtoHigh, 0)]
        public async void ScrapeGumtree(string search, uint page, uint sortNum, uint transNum)
        {
            List<Car> GumtreeCars = await Gumtree.ScrapeGumtree(search, page, sortNum, transNum);

            Assert.NotEmpty(GumtreeCars);
        }

        [Theory]
        [InlineData("celica", 1, 0)]
        public async void ScrapeFbMarketPlace(string search, uint page, uint transNum)
        {
            List<Car> FbMarketPlaceCars = await FbMarketPlace.ScrapeFbMarketPlace(search, page, transNum);

            Assert.NotEmpty(FbMarketPlaceCars);
        }

        [Theory]
        [InlineData("celica", 1, 0)]
        public async void ScrapeCarsales(string search, uint page, uint transNum)
        {
            List<Car> CarsalesCars = await Carsales.ScrapeCarsales(search, page, transNum);

            Assert.NotEmpty(CarsalesCars);
        }
    }
}
