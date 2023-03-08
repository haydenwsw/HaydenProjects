using CarSearcher.Models;
using CarSearcher.Models.Json;
using CarSearcher.ExtensionFunctions;
using CarSearcher.Exceptions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CarSearcher.Scrapers
{
    /// <summary>
    /// carsales.com webscrape
    /// </summary>
    public class Carsales
    {
        private readonly string Site = "https://www.carsales.com.au/";
        private readonly string[] SortBy = { "~Price", "Price", "LastUpdated", "~Odometer", "Odometer" };
        private readonly string[] Transmission = { "", "._.GenericGearType.Manual", "._.GenericGearType.Automatic" };

        private readonly HttpClient HttpClient;
        private readonly CarLookup CarLookup;
        private readonly CarSearcherConfig CarSearcherConfig;

        public Carsales(HttpClient httpclient, CarLookup carLookup, CarSearcherConfig carsearcherconfig)
        {
            HttpClient = httpclient;
            CarSearcherConfig = carsearcherconfig;
            CarLookup = carLookup;
        }

        /// <summary>
        /// webscrape Carsales for all the listings
        /// </summary>
        /// <param name="filterOptions"></param>
        /// <param name="carLookup"></param>
        /// <returns></returns>
        public async Task<List<Car>> ScrapeCars(FilterOptions filterOptions)
        {
            string url = null;

            try
            {
                // format the price range input field
                string priceRange = filterOptions.MinPrice == "" && filterOptions.MaxPrice == "" ? "" : $"._.Price.range({filterOptions.MinPrice}..{filterOptions.MaxPrice})";

                // format pages
                string pages = filterOptions.PageNumber > 1 ? $"&offset={(filterOptions.PageNumber - 1) * 12}" : "";

                if (filterOptions.SearchTerm == null)
                {
                    // get the make model keys
                    string make = CarLookup.MakeModel[filterOptions.Make].Item1.CarsalesKey;
                    string model = CarLookup.MakeModel[filterOptions.Make].Item2[filterOptions.Model].CarsalesKey;

                    // constuct makemodel
                    string makemodel;
                    if (make == null)
                        makemodel = "(And.";
                    else if (model == null)
                        makemodel = $"(And.{make}_.";
                    else
                        makemodel = $"(And.(C.{make}_.{model}.)_.";

                    // build the url with make model
                    url = $"{Site}cars/?q={makemodel}State.New+South+Wales{filterOptions.GetTrans(Transmission)}{priceRange}.)&sort={filterOptions.GetSort(SortBy)}{pages}";
                }
                else
                {
                    // build the url with search term
                    url = $"{Site}cars/?q=(And.Service.carsales._.CarAll.keyword({filterOptions.SearchTerm})._.State.New+South+Wales{filterOptions.GetTrans(Transmission)}{priceRange}.)&sort={filterOptions.GetSort(SortBy)}{pages}";
                }

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.Add("User-Agent", CarSearcherConfig.UserAgent);
                request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                request.Headers.Add("Referer", "https://www.carsales.com.au/");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Alt-Used", "www.carsales.com.au");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.Headers.Add("Sec-Fetch-Dest", "document");
                request.Headers.Add("Sec-Fetch-Mode", "navigate");
                request.Headers.Add("Sec-Fetch-Site", "same-origin");
                request.Headers.Add("TE", "trailers");

                HttpResponseMessage response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();

                // Load HTML doc
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                // get the listing's parent node
                var nodes = htmlDocument.DocumentNode.Descendants("div")
                      .Where(node => node.GetAttributeValue("class", "")
                      .Equals("listing-items"));

                // if failed return empty
                if (!nodes.Any())
                    return new List<Car>();

                // get all the listing
                var items = nodes.FirstOrDefault().SelectNodes("div").ToArray();

                // if failed to get the listing return a emtpy list
                if (items.Length == 0)
                    return new List<Car>();

                var carItems = new List<Car>();
                for (int i = 0; i < items.Length; i++)
                {
                    // div for the urls
                    HtmlNode cardheader = items[i].SelectSingleNode(".//div[@class='card-header']");

                    // if the listing isn't a listing skip it
                    if (cardheader == null)
                        continue;

                    // get listing url
                    string link = cardheader.ChildNodes[1].GetAttributeValue("href", "");

                    // get listing thumbnails
                    HtmlNodeCollection imgs = cardheader.SelectNodes("./a/div/div/img");
                    string imgurl = imgs[0].GetAttributeValue("src", ""); // i just do one for now

                    // div for the texts
                    HtmlNode cardbody = items[i].SelectSingleNode("./div[@class='card-body']");

                    // get listing title
                    string name = cardbody.SelectSingleNode("./div/div/h3/a").InnerText.HtmlDecode();

                    // get listing price
                    int price = cardbody.SelectSingleNode("./div/div/div/div/a").InnerText.ToInt();

                    // get all the extra info in the listing
                    var extrainfo = cardbody.SelectNodes("./div[2]/div[1]/ul/li");

                    // get listing odomitor
                    int kms = extrainfo[0].InnerText.ToInt();

                    // get body type, transmission type and engine cyl
                    string[] info = new string[3];
                    for (int j = 1; j < extrainfo.Count; j++)
                    {
                        info[j - 1] = extrainfo[j].InnerText;
                    }

                    // add them all to the list
                    carItems.Add(new Car(name, Site + link, imgurl, price, kms, Source.Carsales, info));
                }

                return carItems;
            }
            catch (Exception ex)
            {
                return await Task.FromException<List<Car>>(new CarsalesException($"Carsales GetCars has failed. Url: {url}", ex));
            }
        }

        /// <summary>
        /// Gets all the makes and model from Carsales
        /// </summary>
        /// <param name="allMakeModel"></param>
        /// <returns></returns>
        public async Task GetMakeModel(Dictionary<string, (string, List<(string, string)>)> allMakeModel)
        {
            string url = "https://www.carsales.com.au/_homepage/v1/search/?tenantName=carsales";

            // first get all the makes
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Add("User-Agent", CarSearcherConfig.UserAgent);
            request.Headers.Add("Accept", "*/*");

            request.Content = new StringContent("{\"expression\":\"\"}");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();

            CarsalesMakeModel carsalesmakemodel = JsonConvert.DeserializeObject<CarsalesMakeModel>(json);

            if (carsalesmakemodel.PrimaryFields == null && carsalesmakemodel.PrimaryFields.Length == 0)
                throw new CarsalesException($"The makes PrimaryFields is null or empty");
            if (carsalesmakemodel.PrimaryFields[0].Values == null && carsalesmakemodel.PrimaryFields[0].Values.Length == 0)
                throw new CarsalesException($"The makes is null or empty");

            Value[] values = carsalesmakemodel.PrimaryFields[0].Values;

            // add make to the main list
            int start = Array.FindIndex(values, x => x.DisplayValue == null);
            for (int i = start + 1; i < values.Length; i++)
            {
                allMakeModel.TryAdd(values[i].DisplayValue, (values[i].ValueValue, new List<(string, string)>()));
            }

            // then loop through all the makes to get all the models
            foreach (KeyValuePair<string, (string, List<(string, string)>)> item in allMakeModel)
            {
                request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Headers.Add("User-Agent", CarSearcherConfig.UserAgent);
                request.Headers.Add("Accept", "*/*");

                request.Content = new StringContent($"{{\"expression\":\"Make.{item.Key}.\"}}");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // wait a bit so we don't spam the server
                await Task.Delay(500);

                response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                json = await response.Content.ReadAsStringAsync();

                carsalesmakemodel = JsonConvert.DeserializeObject<CarsalesMakeModel>(json);

                if (carsalesmakemodel.PrimaryFields == null && carsalesmakemodel.PrimaryFields.Length == 0)
                    throw new CarsalesException($"The returned PrimaryFields is null or empty on {item.Value.Item1}");
                if (carsalesmakemodel.PrimaryFields[1].Values == null && carsalesmakemodel.PrimaryFields[1].Values.Length == 0)
                    throw new CarsalesException($"The models is null or empty on {item.Value.Item1}");

                values = carsalesmakemodel.PrimaryFields[1].Values;

                // add model to the main list
                start = Array.FindIndex(values, x => x.DisplayValue == null);
                for (int i = start + 1; i < values.Length; i++)
                {
                    allMakeModel[item.Key].Item2.Add((values[i].ValueValue.Split('_')[1][1..^2], values[i].DisplayValue));
                }
            }
        }

        #region model for makemodel
        private class CarsalesMakeModel
        {
            [JsonProperty("version")]
            public long Version { get; set; }

            [JsonProperty("heading")]
            public string Heading { get; set; }

            [JsonProperty("subHeading")]
            public string SubHeading { get; set; }

            [JsonProperty("searchButtonUrl")]
            public string SearchButtonUrl { get; set; }

            [JsonProperty("searchButtonText")]
            public string SearchButtonText { get; set; }

            [JsonProperty("disableSearch")]
            public bool DisableSearch { get; set; }

            [JsonProperty("expression")]
            public string Expression { get; set; }

            [JsonProperty("tenant")]
            public string Tenant { get; set; }

            [JsonProperty("searchUrl")]
            public string SearchUrl { get; set; }

            [JsonProperty("formReset")]
            public bool FormReset { get; set; }

            [JsonProperty("primaryFields")]
            public PrimaryField[] PrimaryFields { get; set; }

            [JsonProperty("secondaryFields")]
            public SecondaryField[] SecondaryFields { get; set; }
        }

        private class PrimaryField
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("defaultValue")]
            public string DefaultValue { get; set; }

            [JsonProperty("values")]
            public Value[] Values { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("expression")]
            public string Expression { get; set; }

            [JsonProperty("removeAction")]
            public string RemoveAction { get; set; }

            [JsonProperty("noValueText")]
            public string NoValueText { get; set; }

            [JsonProperty("emphasis")]
            public bool Emphasis { get; set; }
        }

        private class Value
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("displayValue", NullValueHandling = NullValueHandling.Ignore)]
            public string DisplayValue { get; set; }

            [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
            public string ValueValue { get; set; }
        }

        private class SecondaryField
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore)]
            public string DefaultValue { get; set; }

            [JsonProperty("fromTitle", NullValueHandling = NullValueHandling.Ignore)]
            public string FromTitle { get; set; }

            [JsonProperty("toTitle", NullValueHandling = NullValueHandling.Ignore)]
            public string ToTitle { get; set; }

            [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
            public Value[] Values { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("expression")]
            public string Expression { get; set; }

            [JsonProperty("removeAction")]
            public string RemoveAction { get; set; }

            [JsonProperty("emphasis")]
            public bool Emphasis { get; set; }

            [JsonProperty("noValueText", NullValueHandling = NullValueHandling.Ignore)]
            public string NoValueText { get; set; }
        }
        #endregion
    }
}
