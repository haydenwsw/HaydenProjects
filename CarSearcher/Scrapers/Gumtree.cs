using CarSearcher.Models;
using CarSearcher.ExtensionFunctions;
using CarSearcher.Exceptions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CarSearcher.Scrapers
{
    /// <summary>
    /// gumtree.com webscrape
    /// </summary>
    public partial class Gumtree
    {
        // the urls for scraping
        private readonly string Site = "https://www.gumtree.com.au";
        private readonly string[] Sortby = { "price_asc", "price_desc", "date", "carmileageinkms_a", "carmileageinkms_d" };
        private readonly string[] Transmission = { null, "m", "a" };

        private readonly HttpClient HttpClient;
        private readonly CarLookup CarLookup;
        private readonly CarSearcherConfig CarSearcherConfig;

        public Gumtree(HttpClient httpclient, CarLookup carlookup, CarSearcherConfig carsearcherconfig)
        {
            HttpClient = httpclient;
            CarSearcherConfig = carsearcherconfig;
            CarLookup = carlookup;
        }

        public async Task<List<Car>> ScrapeCars(FilterOptions filterOptions)
        {
            string url = null;

            try
            {
                // get the make model keys
                string make = CarLookup.MakeModel[filterOptions.Make].Item1.GumtreeKey;
                string model = CarLookup.MakeModel[filterOptions.Make].Item2[filterOptions.Model].GumtreeKey;

                // get the search params for make model or text
                string search;
                if (filterOptions.SearchTerm == null)
                    search = (make != null ? $"attributeMap[cars.carmake_s]={make}&" : "") +
                    (model != null ? $"attributeMap[cars.carmodel_s]={model}&" : "");
                else
                    search = $"keywords={filterOptions.SearchTerm}&";

                // build the url
                url = $"{Site}/ws/search.json?{search}" +
                    (filterOptions.GetTrans(Transmission) != null ? $"attributeMap[cars.cartransmission_s]={filterOptions.GetTrans(Transmission)}&" : "") +
                    $"categoryId=18320&categoryName=Cars%2C%20Vans%20%26%20Utes&defaultRadius=10&" +
                    $"locationId=3008839&locationStr=New%20South%20Wales&" +
                    (filterOptions.MaxPrice != "" ? $"maxPrice={filterOptions.MaxPrice}&" : "") +
                    (filterOptions.MinPrice != "" ? $"minPrice={filterOptions.MinPrice}&" : "") +
                    $"offerType=OFFER&" +
                    $"pageNum={filterOptions.PageNumber}&" +
                    $"pageSize=24&" +
                    $"previousCategoryId=18320&radius=0&searchView=GALLERY&" +
                    $"sortByName={filterOptions.GetSort(Sortby)}";

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.Add("User-Agent", CarSearcherConfig.UserAgent);
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Site", "same-origin");
                request.Headers.Add("TE", "trailers");

                HttpResponseMessage response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();

                GumtreeResponce fbMarketplaceResponse = JsonConvert.DeserializeObject<GumtreeResponce>(json);

                ResultList[] resultLists = fbMarketplaceResponse.Data.Results.ResultList;

                //if search comes up empty return
                if (resultLists.Length == 0)
                    return new List<Car>();

                List<Car> carItems = new List<Car>();
                for (int i = 0; i < resultLists.Length; i++)
                {
                    if (resultLists[i].MainImageUrl == null)
                        continue;

                    // add item
                    carItems.Add(new Car(
                        resultLists[i].Title, 
                        Site + resultLists[i].Url, 
                        resultLists[i].MainImageUrl.AbsoluteUri, 
                        resultLists[i].PriceText.ToInt(), 
                        resultLists[i].MainAttributes.Data.Carmileageinkms.ToInt(), 
                        Source.Gumtree, 
                        new string[3] { resultLists[i].MainAttributes.Data.Carbodytype, resultLists[i].MainAttributes.Data.Cartransmission,
                            Join(resultLists[i].MainAttributes.Data.CylinderConfiguration, resultLists[i].MainAttributes.Data.EngineCapacityLitres) }
                        ));
                }

                return carItems;
            }
            catch (Exception ex)
            {
                return await Task.FromException<List<Car>>(new GumtreeException($"Gumtree GetCars has failed. Url: {url}", ex));
            }
        }

        /// <summary>
        /// Gets all the makes and model from Gumtree
        /// </summary>
        /// <param name="allMakeModel"></param>
        /// <returns></returns>
        public async Task GetMakeModel(Dictionary<string, (string, List<(string, string)>)> allMakeModel)
        {
            // first get all the makes
            HtmlDocument htmlDocument = new HtmlDocument();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://www.gumtree.com.au/cars");

            request.Headers.Add("User-Agent", CarSearcherConfig.UserAgent);
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("Referer", "https://www.gumtree.com.au/");
            request.Headers.Add("DNT", "1");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");

            HttpResponseMessage response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string html = await response.Content.ReadAsStringAsync();

            // Load HTML doc
            htmlDocument.LoadHtml(html);

            HtmlNode selectNode = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"clp-as-make\"]");

            if (selectNode == null && selectNode.ChildNodes == null)
                throw new GumtreeException($"The makes select node is null or empty");

            // add make to the main list
            foreach (HtmlNode option in selectNode.SelectNodes("option"))
            {
                string make = option.GetAttributeValue("value", "");
                if (make == "")
                    continue;

                allMakeModel.TryAdd(option.InnerText.Trim(), (make, new List<(string, string)>()));
            }

            // then loop through all the makes to get all the models
            foreach (KeyValuePair<string, (string, List<(string, string)>)> item in allMakeModel)
            {
                request = new HttpRequestMessage(HttpMethod.Get, $"https://www.gumtree.com.au/p-select-attribute.json?categoryId=18320&attributeId=cars.carmake_s&rootAttribute=cars.carmake_s&optionPath={item.Value.Item1}");

                request.Headers.Add("User-Agent", CarSearcherConfig.UserAgent);
                request.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("Referer", "https://www.gumtree.com.au/cars");
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Site", "same-origin");
                request.Headers.Add("TE", "trailers");

                // wait a bit so we don't spam the server
                await Task.Delay(500);

                response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();

                GumtreeModels gumtreeModels = JsonConvert.DeserializeObject<GumtreeModels[]>(json).FirstOrDefault();

                if (gumtreeModels == null)
                    continue;

                for (int i = 1; i < gumtreeModels.Items.Length; i++)
                {
                    allMakeModel[item.Key].Item2.Add((gumtreeModels.Items[i].Id, gumtreeModels.Items[i].Value));
                }
            }
        }

        private string Join(string a, string b)
        {
            if (a != null && b != null)
                return $"{a} {b}";
            else if (a == null ^ b == null)
                return $"{a}{b}";
            else
                return null;
        }

        #region model for the car models
        private partial class GumtreeModels
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public Item[] Items { get; set; }
            public bool GumtreeModelRequired { get; set; }
        }

        private partial class Item
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }
        #endregion
    }
}
