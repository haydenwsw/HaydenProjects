using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;
using AutoHarvest.Scoped;
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
        private const string args = "cars/used/toyota/celica/new-south-wales-state/";
        private static readonly string[] SortType = { "?sort=~Price", "?sort=Price", "?sort=LastUpdated", "?sort=~Odometer", "?sort=Odometer" };

        // webscrape ebay for all the listings
        public static async Task<List<Car>> ScrapeCarsales(CefSharpHeadless HeadlessBrowser, string search, int page, int sortType)
        {
            // carsales has no "search bar" so im just gonna have to do this for now
            if (search != "celica")
                return new List<Car>();

            // Initializing the html doc
            HtmlDocument htmlDocument = new HtmlDocument();

            // get the HTML doc of website
            string url = $"{site}{args}{SortType[sortType]}";
            string html = await HeadlessBrowser.GetHtmlAsync(url);

            // Load HTML doc
            htmlDocument.LoadHtml(html);

            // get all the listing
            var items = htmlDocument.DocumentNode.Descendants("div")
                  .Where(node => node.GetAttributeValue("class", "")
                  .Equals("listing-items"))
                  .FirstOrDefault().SelectNodes("div").ToArray();

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
                HtmlNodeCollection imgs = cardheader.SelectNodes(".//a//div//div//img");
                string imgurl = imgs[0].GetAttributeValue("src", ""); // i just do one for now

                // div for the texts
                HtmlNode cardbody = items[i].SelectSingleNode(".//div[@class='card-body']");

                // get listing title
                string name = cardbody.SelectSingleNode(".//div//div//h3//a").InnerText;

                // get listing price
                int price = cardbody.SelectSingleNode(".//div//div//div//div//a").InnerText.ToInt();

                // get listing odomitor
                var kms = cardbody.SelectSingleNode(".//div//div//ul//li").InnerText.ToInt();

                // add them all to the list
                carItems.Add(new Car(name, "https://www.carsales.com.au/cars/" + link, imgurl, price, kms, "Carsales"));
            }

            return carItems;
        }
    }
}
