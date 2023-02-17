using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;
using AutoHarvest.Pages;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoHarvest.Scrapers
{
    /// <summary>
    /// carsales.com webscrape
    /// </summary>
    public class Carsales
    {
        private readonly string site = "https://www.carsales.com.au/";
        private readonly string[] SortType = { "~Price", "Price", "LastUpdated", "~Odometer", "Odometer" };
        private readonly string[] TransType = { "", "._.GenericGearType.Manual", "._.GenericGearType.Automatic" };
        private readonly string priceRangeText = "._.Price.range({0}..{1})";

        private readonly HttpClient HttpClient;
        private readonly ILogger<Carsales> Logger;

        public Carsales(IHttpClientFactory httpclientfactory, ILogger<Carsales> logger)
        {
            HttpClient = httpclientfactory.CreateClient();
            Logger = logger;
        }

        // webscrape Carsales for all the listings
        public async Task<List<Car>> ScrapeCarsales(FilterOptions FilterOptions)
        {
            string url = null;

            try
            {
                // Initializing the html doc
                HtmlDocument htmlDocument = new HtmlDocument();

                // format the price range input field
                string priceRange = FilterOptions.PriceMin == "" && FilterOptions.PriceMax == "" ? "" : string.Format(priceRangeText, FilterOptions.PriceMin, FilterOptions.PriceMax);

                // get the HTML doc of website
                url = $"{site}cars/?sort={SortType[FilterOptions.SortType]}&q=(And.Service.CARSALES._.CarAll.keyword({FilterOptions.SearchTerm})._.State.New South Wales{TransType[FilterOptions.TransType]}{priceRange}.)&offset={(FilterOptions.PageNumber - 1) * 12}";

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/109.0");
                request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                request.Headers.Add("Host", "www.carsales.com.au");
                request.Headers.Add("Referer", "https://carsid.carsales.com.au/");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.Headers.Add("Sec-Fetch-Dest", "document");
                request.Headers.Add("Sec-Fetch-Mode", "navigate");
                request.Headers.Add("Sec-Fetch-Site", "same-site");
                request.Headers.Add("TE", "trailers");

                HttpResponseMessage response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();

                // Load HTML doc
                htmlDocument.LoadHtml(html);

                // get the listing's parent node
                var nodes = htmlDocument.DocumentNode.Descendants("div")
                      .Where(node => node.GetAttributeValue("class", "")
                      .Equals("listing-items"));

                // if failed return empty
                if (!nodes.Any())
                {
                    Logger.LogInformation("Carsales scraper the listings parent node has no children. CarsalesUrl: {url}. FilterOptions: {FilterOptions}", url, FilterOptions);
                    return new List<Car>();
                }

                // get all the listing
                var items = nodes.FirstOrDefault().SelectNodes("div").ToArray();

                // if failed to get the listing return a emtpy list
                if (items.Length == 0)
                {
                    Logger.LogInformation("Carsales scraper the listings are empty. CarsalesUrl: {url}. FilterOptions: {FilterOptions}", url, FilterOptions);
                    return new List<Car>();
                }

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
                    carItems.Add(new Car(name, site + link, imgurl, price, kms, Source.Carsales, info));
                }

                return carItems;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Carsales scraper has failed. CarsalesUrl: {url}. FilterOptions: {FilterOptions}", url, FilterOptions);
                return new List<Car>();
            }
        }
    }
}
