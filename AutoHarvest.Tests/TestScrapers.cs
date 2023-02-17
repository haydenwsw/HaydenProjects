using System;
using System.Collections.Generic;
using Xunit;
using AutoHarvest.Scrapers;
using AutoHarvest.Models;
using System.Net.Http;

namespace AutoHarvest.Tests
{
    public class TestScrapers
    {
        private IHttpClientFactory httpClient;

        public TestScrapers()
        {
            var httpClient = new DefaultHttpClientFactory();
        }

        [Theory]
        [InlineData("celica", 1, (int)SortTypes.PriceLowtoHigh, (int)TransTypes.All)]
        public async void ScrapeGumtree(string search, int page, int sortType, int transType)
        {
            //Gumtree gumtree = new Gumtree(new HttpClient(), );

            FilterOptions filterOptions = new FilterOptions(search, sortType, transType, "", "", true, true, true, page);
            //List<Car> GumtreeCars = await Gumtree.ScrapeGumtree(filterOptions);

            //Assert.NotEmpty(GumtreeCars);
        }

        //[Theory]
        //[InlineData("celica", 1, (int)TransTypes.All)]
        //public async void ScrapeFbMarketPlace(string search, int page, int transNum)
        //{
        //    List<Car> FbMarketPlaceCars = await FbMarketplace.ScrapeFbMarketplace(null, search, page, transNum);

        //    Assert.NotEmpty(FbMarketPlaceCars);
        //}

        //[Theory]
        //[InlineData("celica", 1, 0)]
        //public async void ScrapeCarsales(string search, int page, int transNum)
        //{
        //    List<Car> CarsalesCars = await Carsales.ScrapeCarsales(search, page, transNum);

        //    Assert.NotEmpty(CarsalesCars);
        //}
    }

    public sealed class DefaultHttpClientFactory : IHttpClientFactory, IDisposable
    {
        private readonly Lazy<HttpMessageHandler> _handlerLazy = new(() => new HttpClientHandler());

        public HttpClient CreateClient(string name) => new(_handlerLazy.Value, disposeHandler: false);

        public DefaultHttpClientFactory()
        {

        }

        public void Dispose()
        {
            if (_handlerLazy.IsValueCreated)
            {
                _handlerLazy.Value.Dispose();
            }
        }
    }
}
