using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoHarvest.Scoped;

namespace AutoHarvest.Scrapers
{
    // the class that webscrapes facebook.com/marketplace
    public static class FbMarketplace
    {
        // the urls for scraping
        private const string site = "https://www.facebook.com/marketplace/sydney/search";
        private const string args = "&category_id=vehicles&exact=false";
        private static readonly string[] trans = { "/?category_id=vehicles&query=", "?transmissionType=manual&query=", "?transmissionType=automatic&query=" };

        // webscrape FbMarketplace for all the listings
        public async static Task<List<Car>> ScrapeFbMarketplace(CefSharpHeadless HeadlessBrowser, string search, int page, int transNum)
        {
            // need headless browsers to do mulitable pages this is bs
            if (page > 1)
                return new List<Car>();
            
            // Initializing the html doc
            HtmlDocument htmlDocument = new HtmlDocument();

            // get the HTML doc of website
            string url = $"{site}{trans[transNum]}{search}{args}";
            string html = await HeadlessBrowser.GetHtmlAsync(url);

            // Load HTML doc
            htmlDocument.LoadHtml(html);

            // gets the longest string in the list
            string script = htmlDocument.DocumentNode.Descendants("script").Aggregate("", (max, cur) => max.Length > cur.InnerText.Length ? max : cur.InnerText);

            // arrays to find and store the data, 0: url, 1: imgUrl, 2: price, 3: title, 4: kms
            string[] keyWords = { "GroupCommerceProductItem\",\"id\":", "image\":{\"uri\":", "amount\":\"", "listing_title\":", "subtitle\":" };
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
                        if (script[++idx] != '\\')
                            text += script[idx];
                    }

                    // add the text to the array
                    texts[i] = text;
                }

                // add them all to the list
                carItems.Add(new Car(texts[3], "https://www.facebook.com/marketplace/" + texts[0], texts[1], texts[2].ToInt(), texts[4].ToInt() * 1000, "FbMarketplace"));
            }

            return carItems;
        }
    }
}
