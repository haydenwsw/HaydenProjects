using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;
using AutoHarvest.Singleton;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Scrapers
{
    // the class that webscrapes Carsales.com
    public static class Carsales
    {
        private const string site = "https://www.carsales.com.au/";
        private static readonly string[] SortType = { "~Price", "Price", "LastUpdated", "~Odometer", "Odometer" };
        private static readonly string[] TransType = { "", "._.GenericGearType.Manual", "._.GenericGearType.Automatic" };
        private const string priceRangeText = "._.Price.range({0}..{1})";

        // webscrape ebay for all the listings
        public static async Task<List<Car>> ScrapeCarsales(Func<string, Task<string>> GetHtmlAsync, FilterOptions filterOptions, int page)
        {
            // Initializing the html doc
            HtmlDocument htmlDocument = new HtmlDocument();

            // format the price range input field
            string priceRange = filterOptions.PriceMin == "" && filterOptions.PriceMax == "" ? "" : string.Format(priceRangeText, filterOptions.PriceMin, filterOptions.PriceMax);

            // get the HTML doc of website
            string url = $"{site}/cars/?sort={SortType[filterOptions.SortType]}&q=(And.Service.CARSALES._.CarAll.keyword({filterOptions.SearchTerm})._.State.New South Wales{TransType[filterOptions.TransType]}{priceRange}.)&offset={(page - 1) * 12}";
            string html = await GetHtmlAsync(url);

            // Load HTML doc
            htmlDocument.LoadHtml(html);

            // get all the listing
            var nodes = htmlDocument.DocumentNode.Descendants("div")
                  .Where(node => node.GetAttributeValue("class", "")
                  .Equals("listing-items"));

            // if failed return empty
            if (nodes.Count() == 0)
                return new List<Car>();

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
                string name = cardbody.SelectSingleNode("./div/div/h3/a").InnerText;

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
                carItems.Add(new Car(name, site + link, imgurl, price, kms, info, "Carsales"));
            }

            return carItems;
        }
    }
}
