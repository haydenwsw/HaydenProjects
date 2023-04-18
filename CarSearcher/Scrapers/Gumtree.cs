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
        private readonly string[] Sortby = { "price_asc", "price_desc", "date", "carmileageinkms_a", "carmileageinkms_d",
                                            "rank", "caryear_a", "caryear_d" };
        private readonly string[] Transmission = { null, "m", "a" };

        private readonly HttpClient HttpClient;
        private readonly CarLookup CarLookup;
        private readonly CarSearcherConfig CarSearcherConfig;

        // get all the makes for car guesser
        private readonly string[] RngMakes;

        public Gumtree(HttpClient httpclient, CarLookup carlookup, CarSearcherConfig carsearcherconfig)
        {
            HttpClient = httpclient;
            CarSearcherConfig = carsearcherconfig;
            CarLookup = carlookup;

            RngMakes = CarLookup.MakeModel.Where(x => x.Value.Item2.Values.Count > 2).Select(x => x.Key).ToArray();
        }

        public async Task<List<Car>> ScrapeCars(FilterOptions filterOptions)
        {
            string url = null;

            try
            {
                // get the search params for make model or text
                string search;
                if (filterOptions.SearchTerm == null)
                {
                    // get the make model keys
                    string make = CarLookup.MakeModel[filterOptions.Make].Item1.GumtreeKey;
                    string model = CarLookup.MakeModel[filterOptions.Make].Item2[filterOptions.Model].GumtreeKey;

                    search = (make != null ? $"attributeMap[cars.carmake_s]={make}&" : "") +
                    (model != null ? $"attributeMap[cars.carmodel_s]={model}&" : "");
                }
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

                ResultList[] resultList = fbMarketplaceResponse.Data.Results.ResultList;

                //if search comes up empty return
                if (resultList.Length == 0)
                    return new List<Car>();

                List<Car> carItems = new List<Car>();
                for (int i = 0; i < resultList.Length; i++)
                {
                    if (resultList[i].MainImageUrl == null)
                        continue;

                    // add item
                    carItems.Add(new Car(
                        resultList[i].Title, 
                        Site + resultList[i].Url, 
                        resultList[i].MainImageUrl.AbsoluteUri, 
                        resultList[i].PriceText.ToInt(), 
                        resultList[i].MainAttributes.Data.Carmileageinkms.ToInt(), 
                        Source.Gumtree, 
                        new string[3] { resultList[i].MainAttributes.Data.Carbodytype, resultList[i].MainAttributes.Data.Cartransmission,
                            Join(resultList[i].MainAttributes.Data.CylinderConfiguration, resultList[i].MainAttributes.Data.EngineCapacityLitres) }
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
        /// Get's a random Car for the user to guess what it is
        /// </summary>
        /// <param name="body"></param>
        /// <param name="company"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public async Task<GuessCar> GetRandomCar(string body, string company, bool isEasyMode, Random rng)
        {
            string url = null;
            string make = "";

            try
            {
                if (body != null)
                {
                    // use switch
                    string bodyType = "";
                    switch (body)
                    {
                        case "Convertible":
                            bodyType = "conv";
                            break;
                        case "Coupe":
                            bodyType = "coup";
                            break;
                        case "Hatchback":
                            bodyType = "htchbck";
                            break;
                        case "Sedan":
                            bodyType = "sedan";
                            break;
                        case "Wagon":
                            bodyType = "wagon";
                            break;
                        case "SUV":
                            bodyType = "suv";
                            break;
                        default: // All
                            bodyType = "";
                            break;
                    }

                    int pageNumber = rng.Next(1, 30);

                    url = $"{Site}/ws/search.json?" +
                        (bodyType != "" ? $"attributeMap[cars.carbodytype_s]={bodyType}&" : "") +
                        "categoryId=18320&" +
                        "categoryName=Cars%2C%20Vans%20%26%20Utes&" +
                        "defaultRadius=10&" +
                        "locationId=0&" +
                        "locationStr=Australia&" +
                        $"pageNum={pageNumber}&" +
                        "pageSize=24&" +
                        "previousCategoryId=18320&" +
                        "radius=0&" +
                        "searchView=GALLERY&" +
                        $"sortByName={Sortby[rng.Next(0, Sortby.Length)]}";
                }
                else
                {
                    // get the make keys
                    make = "toyota";
                    if (CarLookup.MakeModel.ContainsKey(company))
                        make = CarLookup.MakeModel[company].Item1.GumtreeKey;

                    int maxPage = await GetMaxPages(make);
                    int pageNumber = rng.Next(1, maxPage);

                    url = $"{Site}/ws/search.json?" +
                        $"attributeMap[cars.carmake_s]={make}&" +
                        "categoryId=18320&" +
                        "categoryName=Cars%2C%20Vans%20%26%20Utes&" +
                        "defaultRadius=10&" +
                        "locationId=0&" +
                        "locationStr=Australia&" +
                        $"pageNum={pageNumber}&" +
                        "pageSize=24&" +
                        "previousCategoryId=18320&" +
                        "radius=0&" +
                        "searchView=GALLERY&" +
                        $"sortByName={Sortby[rng.Next(0, Sortby.Length)]}";
                }

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

                ResultList[] resultList = fbMarketplaceResponse.Data.Results.ResultList;

                int removed = 0;
                while (true)
                {
                    int lenght = resultList.Length - removed;
                    // if failed to get the listing return
                    if (lenght == 0)
                        throw new GumtreeException("List empty");

                    int idx = rng.Next(0, lenght);
                    string name = resultList[idx].Title;

                    // if the img url is null
                    if (resultList[idx].MainImageUrl == null)
                    {
                        Swap(resultList, idx, lenght - 1);
                        removed++;
                        continue;
                    }

                    string imgUrl = resultList[idx].MainImageUrl.AbsoluteUri;

                    // get max res image
                    imgUrl = imgUrl.Replace("t_$_35", "t_$_s-l800");

                    // get the listings make
                    if (!CarLookup.MakeModel.ContainsKey(make))
                        make = CarLookup.MakeModel.Keys.FirstOrDefault(x => name.Contains(x));

                    // check if it's on record
                    if (make == null)
                    {
                        make = "";
                        // move the failed listing to the end then cap the lenght so it can't be reached
                        Swap(resultList, idx, lenght - 1);
                        removed++;
                        continue;
                    }

                    // get all the models of the listings make
                    var models = CarLookup.MakeModel[make].Item2.Keys.Skip(1).ToList();

                    // get the listings model
                    string model = models.FirstOrDefault(x => name.Contains(x));

                    // check if it's on record
                    if (model == null)
                    {
                        make = "";
                        Swap(resultList, idx, lenght - 1);
                        removed++;
                        continue;
                    }

                    // create the choices
                    string[] choices = new string[4];
                    models.Remove(model);

                    if (isEasyMode || body == null)
                    {
                        // get easy mode false choices
                        for (int i = 1; i < choices.Length; i++)
                        {
                            string model2 = models[rng.Next(0, models.Count)];
                            choices[i] = $"{make} {model2}";
                            if (models.Count > 1)
                                models.Remove(model2);
                        }
                    }
                    else
                    {
                        // get hard mode false choices
                        int count = RngMakes.Length;
                        for (int i = 1; i < choices.Length; i++)
                        {
                            string make2 = RngMakes.ElementAt(rng.Next(0, count));
                            count = CarLookup.MakeModel[make2].Item2.Keys.Count - 1;
                            string model2 = CarLookup.MakeModel[make2].Item2.Keys.ElementAt(rng.Next(1, count));
                            choices[i] = $"{make2} {model2}";

                        }
                    }

                    // randomly set the answer
                    int ans = rng.Next(0, choices.Length);
                    choices[0] = choices[ans];
                    choices[ans] = $"{make} {model}";

                    return new GuessCar(imgUrl, ans, choices);
                }
            }
            catch (Exception ex)
            {
                return await Task.FromException<GuessCar>(new CarsalesException($"Gumtree GuessCar has failed. Url: {url}", ex));
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

        private async Task<int> GetMaxPages(string make)
        {
            string url = $"{Site}/j-cars-search.json?attributeMap[cars.carmake_s]={make}&attributeMap[cars.carmodel_s]=&minPrice=&maxPrice=&locationId=0&keywords=";

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

            Max numberFound = JsonConvert.DeserializeObject<Max>(json);
            int maxPage = numberFound.numberFound / 24 + 1;
            maxPage = Math.Clamp(maxPage, 1, 50);
            return maxPage;
        }

        private void Swap<T>(T[] arry, int a, int b)
        {
            var temp = arry[a];
            arry[a] = arry[b];
            arry[b] = temp;
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

        private class Max
        {
            public int numberFound { get; set; }
        }
    }
}
