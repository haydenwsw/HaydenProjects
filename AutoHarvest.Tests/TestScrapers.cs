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
        private IHttpClientFactory HttpClient;

        public TestScrapers()
        {
            HttpClient = new DefaultHttpClientFactory();
        }

        [Theory]
        [InlineData("celica", (int)SortBy.PriceLowtoHigh, (int)Transmission.All, "", "", 1)]
        public async void ScrapeCarsales(string search, int sortType, int transType, string priceMin, string priceMax, int page)
        {
            FilterOptions filterOptions = new FilterOptions(search, sortType, transType, priceMin, priceMax, true, true, true, page);

            //List<Car> CarsalesCars = await Carsales.ScrapeCarsales(search, page, transNum);

            //Assert.NotEmpty(CarsalesCars);
        }

        [Theory]
        [InlineData("celica", (int)SortBy.PriceLowtoHigh, (int)Transmission.All, "", "", 1)]
        public async void ScrapeFbMarketPlace(string search, int sortType, int transType, string priceMin, string priceMax, int page)
        {
            FilterOptions filterOptions = new FilterOptions(search, sortType, transType, priceMin, priceMax, true, true, true, page);

            //List<Car> FbMarketPlaceCars = await FbMarketplace.ScrapeFbMarketplace(null, search, page, transNum);

            //Assert.NotEmpty(FbMarketPlaceCars);
        }

        [Theory]
        [InlineData("celica", (int)SortBy.PriceLowtoHigh, (int)Transmission.All, "", "", 1)]
        public async void ScrapeGumtree(string search, int sortType, int transType, string priceMin, string priceMax, int page)
        {
            FilterOptions filterOptions = new FilterOptions(search, sortType, transType, priceMin, priceMax, true, true, true, page);

            //Gumtree gumtree = new Gumtree(new HttpClient(), );

            //List<Car> GumtreeCars = await Gumtree.ScrapeGumtree(filterOptions);

            //Assert.NotEmpty(GumtreeCars);
        }
    }

    public sealed class DefaultHttpClientFactory : IHttpClientFactory, IDisposable
    {
        private readonly Lazy<HttpMessageHandler> _handlerLazy = new Lazy<HttpMessageHandler>(() => new HttpClientHandler());

        public HttpClient CreateClient(string name) => new HttpClient(_handlerLazy.Value, disposeHandler: false);

        public void Dispose()
        {
            if (_handlerLazy.IsValueCreated)
            {
                _handlerLazy.Value.Dispose();
            }
        }
    }
}
