using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoHarvest.Scrapers
{
    // the class that webscrapes gumtree.com
    public static class Gumtree
    {
        // the urls for scraping
        private const string site = "https://www.gumtree.com.au";
        private const string args1 = "/s-cars-vans-utes/nsw/";
        private const string args2 = "/page-";
        private const string args3 = "/k0c18320l3008839";
        private static readonly string[] sort = { "?sort=price_asc&ad=offering", "?sort=price_desc&ad=offering", "?ad=offering", "?sort=carmileageinkms_a&ad=offering", "?sort=carmileageinkms_d&ad=offering" };
        private static readonly string[] trans = { "", "/cartransmission-m", "/cartransmission-a" };

        // webscrape Gumtree for all the listings
        public async static Task<List<Car>> ScrapeGumtree(string search, int page, FilterOptions filterOptions)
        {
            // Initializing the html doc
            HtmlDocument htmlDocument = new HtmlDocument();

            using (HttpClient httpClient = new HttpClient())
            {
                // get the HTML doc of website with headers
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                //string e = $"{site}{args1}{search}{trans[filterOptions.TransType]}{args2}{page}{args3}{sort[filterOptions.SortType]}";
                HttpResponseMessage response = await httpClient.GetAsync($"{site}{args1}{search}{trans[filterOptions.TransType]}{args2}{page}{args3}{sort[filterOptions.SortType]}");
                string html = await response.Content.ReadAsStringAsync();

                // Load HTML doc
                htmlDocument.LoadHtml(html);
            }

            // get all the listing
            var items = htmlDocument.DocumentNode.Descendants("div")
                  .Where(node => node.GetAttributeValue("class", "")
                  .Equals("user-ad-collection-new-design__wrapper--row"))
                  .FirstOrDefault().SelectNodes("a").ToArray();

            // if failed to get the listing return a emtpy list
            if (items.Length == 0)
                return new List<Car>();

            // get the script that containts all the img urls
            var script = htmlDocument.DocumentNode.Descendants("script").ToArray()[9].InnerHtml;

            // exstract the image urls from the script
            var imgUrls = new List<string>();
            int idx = script.IndexOf("35.jpg", 0);
            while (idx != -1)
            {
                // walk backwards through the string to construct the url
                string imgUrl = "35.jpg";
                int i = idx;
                while (script[i - 1] != '"')
                {
                    imgUrl = $"{script[--i]}{imgUrl}";
                }
                imgUrls.Add(imgUrl);

                // break early when found all the images are found to save time
                if (imgUrls.Count == items.Length)
                    break;

                // find the next url in the string
                idx = script.IndexOf("35.jpg", idx + 1);
            }

            var carItems = new List<Car>();

            // get all the useful info
            int counter = 0;
            for (int i = 0; i < items.Length; i++)
            {
                // get the name, price and (location and time posted)
                string[] tags = items[i].GetAttributeValue("aria-label", "").Split('\n');

                // get kms
                int kms = items[i].ChildNodes[1].ChildNodes[2].ChildNodes[0].ChildNodes[0].InnerText.ToInt();

                // gets the listings image url TODO: check if it has one or not
                string imgUrl = imgUrls[counter++];

                // get the item url
                string link = site + items[i].GetAttributeValue("href", "");

                // add them all to the list
                carItems.Add(new Car(tags[0], link, imgUrl, tags[1].ToInt(), kms, "Gumtree"));
            }

            // return the list
            return carItems;
        }
    }
}
