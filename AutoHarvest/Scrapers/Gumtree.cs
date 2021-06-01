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
        private static readonly string[] sort = { "?sort=price_asc&ad=offering", "?sort=price_desc&ad=offering", "?ad=offering", "?sort=carmileageinkms_a&ad=offering", "?sort=carmileageinkms_d&ad=offering" };
        private static readonly string[] trans = { "", "/cartransmission-m", "/cartransmission-a" };

        // webscrape Gumtree for all the listings
        public async static Task<List<Car>> ScrapeGumtree(FilterOptions filterOptions, int page)
        {
            // Initializing the html doc
            HtmlDocument htmlDocument = new HtmlDocument();

            using (HttpClient httpClient = new HttpClient())
            {
                // get the HTML doc of website with headers
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                string url = $"{site}/s-cars-vans-utes/nsw/{filterOptions.SearchTerm}{trans[filterOptions.TransType]}/page-{page}/k0c18320l3008839{sort[filterOptions.SortType]}";
                HttpResponseMessage response = await httpClient.GetAsync(url);
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
            string key = "mainImageUrl\":";
            var imgUrls = new List<string>();
            int idx = script.IndexOf(key, 0);
            while (idx != -1)
            {
                // walk forwards through the string to construct the text
                idx += key.Length + 1;
                string imgUrl = "";
                while (script[idx] != '"')
                {
                    imgUrl += script[idx++];
                }
                imgUrls.Add(imgUrl);

                // break early when found all the images are found to save time
                if (imgUrls.Count == items.Length)
                    break;

                // find the next url in the string
                idx = script.IndexOf(key, idx + 1);
            }

            var carItems = new List<Car>();

            // get all the useful info
            int counter = 0;
            for (int i = 0; i < items.Length; i++)
            {
                // get the name, price and (location and time posted)
                string[] tags = items[i].GetAttributeValue("aria-label", "").Split('\n');

                // get all the extra info in the listing
                var extrainfo = items[i].SelectSingleNode("./div[2]/div/ul").ChildNodes;

                // get kms
                int kms = extrainfo[0].InnerText.ToInt();

                // get body type, transmission type and engine cyl
                string[] info = new string[3];
                for (int j = 1; j < extrainfo.Count; j++)
                {
                    info[j - 1] = extrainfo[j].InnerText;
                }

                // gets the listings image url
                string imgUrl = imgUrls[counter++];

                // get the item url
                string link = site + items[i].GetAttributeValue("href", "");

                // add them all to the list
                carItems.Add(new Car(tags[0], link, imgUrl, tags[1].ToInt(), kms, info, "Gumtree"));
            }

            // return the list
            return carItems;
        }
    }
}
