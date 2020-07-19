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
    // the class that webscrapes facebook.com/marketplace
    public class FbMarketPlace
    {
        // the urls for scraping
        private const string site = "https://www.facebook.com/marketplace/sydney/search";
        private const string args = "&category_id=vehicles&exact=false";
        private static readonly string[] trans = { "/?category_id=vehicles&query=", "?transmissionType=manual&query=", "?transmissionType=automatic&query=" };

        // webscrape ebay for all the listings
        public async static Task<List<Car>> ScrapeFbMarketPlace(string search, uint page, uint transNum)
        {
            // need headless browsers to do mulitable pages this is bs
            if (page > 1)
                return new List<Car>();

            // Initializing the html doc
            HtmlDocument htmlDocument = new HtmlDocument();

            using (HttpClient httpClient = new HttpClient())
            {
                // get the HTML doc of website
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                HttpResponseMessage response = await httpClient.GetAsync($"{site}{trans[transNum]}{search}{args}");
                string html = await response.Content.ReadAsStringAsync();

                // Load HTML doc
                htmlDocument.LoadHtml(html);
            }

            // get all the listing
            var scripts = htmlDocument.DocumentNode.Descendants("script").ToArray();

            // if failed to get the listing return a emtpy list
            if (scripts.Length == 0)
                return new List<Car>();

            // set the third last item in the array
            var script = scripts[scripts.Length - 3].InnerText;

            // arrays to store the text
            string[] keyWords = { "uri:", "text:", "marketplace_listing_title:", "custom_sub_titles:[", "url:" };
            string[] texts = { "", "", "", "", "" };

            var carItems = new List<Car>();

            // exstract the usefull text from the script
            int idx = 0;
            while (idx != -1)
            {
                for (int i = 0; i < keyWords.Length; i++)
                {
                    // find the next sub string in the string
                    idx = script.IndexOf(keyWords[i], idx);

                    // if reach the end of the string return
                    if (idx == -1)
                        return carItems;

                    // walk forwards through the string to construct the text
                    idx += keyWords[i].Length;
                    string text = "";
                    while (script[idx + 1] != '"')
                    {
                        text += script[++idx];
                    }

                    // add the text to the array
                    texts[i] = text;
                }

                // add them all to the list
                carItems.Add(new Car(texts[2], texts[4], texts[0], texts[1].toUInt(), texts[3].toUInt() * 1000, "FbMarketPlace"));
            }

            return carItems;
        }
    }
}
