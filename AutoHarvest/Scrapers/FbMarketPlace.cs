using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHarvest.Pages;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using AutoHarvest.Singletons;

namespace AutoHarvest.Scrapers
{
    /// <summary>
    /// facebook.com/marketplace webscrape
    /// </summary>
    public class FbMarketplace
    {
        // the urls for scraping
        private readonly string site = "https://www.facebook.com/marketplace/";
        private readonly string[] trans = { "", "&transmissionType=manual", "&transmissionType=automatic" };
        private readonly string[] sort = { "price_ascend", "price_descend", "", "", "" };

        private readonly HttpClient HttpClient;
        private readonly Cookies Cookies;
        private readonly ILogger<FbMarketplace> Logger;

        public FbMarketplace(IHttpClientFactory httpclientfactory, Cookies cookies, ILogger<FbMarketplace> logger)
        {
            HttpClient = httpclientfactory.CreateClient();
            Cookies = cookies;
            Logger = logger;
        }

        // webscrape FbMarketplace for all the listings
        public async Task<List<Car>> ScrapeFbMarketplace(FilterOptions FilterOptions)
        {
            string url = null;

            try
            {
                // requres to save the page per user and activate js a.k.a some bs
                if (FilterOptions.PageNumber > 1)
                    return new List<Car>();

                // Initializing the html doc
                HtmlDocument htmlDocument = new HtmlDocument();

                // format the price range input field
                string minPrice = FilterOptions.PriceMin == "" ? FilterOptions.PriceMin : $"&minPrice={FilterOptions.PriceMin}";
                string maxPrice = FilterOptions.PriceMax == "" ? FilterOptions.PriceMax : $"&maxPrice={FilterOptions.PriceMax}";

                // get the HTML doc of website
                url = $"{site}sydney/search?query={FilterOptions.SearchTerm}&sortBy={sort[FilterOptions.SortType]}{minPrice}{maxPrice}{trans[FilterOptions.TransType]}&category_id=vehicles&exact=false";

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/109.0");
                request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Alt-Used", "www.facebook.com");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("Cookie", Cookies.FbMarketplace);
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.Headers.Add("Sec-Fetch-Dest", "document");
                request.Headers.Add("Sec-Fetch-Mode", "navigate");
                request.Headers.Add("Sec-Fetch-Site", "none");
                request.Headers.Add("Sec-Fetch-User", "?1");
                request.Headers.Add("TE", "trailers");

                HttpResponseMessage response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();

                // Load HTML doc
                htmlDocument.LoadHtml(html);

                // gets the longest string in the list
                string script = htmlDocument.DocumentNode.Descendants("script").Select(s => s.InnerText).Where(x => x.Contains("MarketplaceFeedListingStoryObject")).FirstOrDefault();

                if (script == null)
                {
                    Logger.LogInformation("FbMarketplace scraper hasn't found a script to scrape from. FbMarketplaceUrl: {url}. FilterOptions: {FilterOptions}", url, FilterOptions);
                    return new List<Car>();
                }

                // trim down the json to makes things less of a headache
                int idx = script.IndexOf("{\"__bbox");
                idx = script.IndexOf("{\"__bbox", idx + "{\"__bbox".Length);
                string json = script.Substring(idx, script.Length - idx - 41); // hard code values for good luck

                // deserialize into the model
                FbMarketplaceJson fbMarketplaceModel = FbMarketplaceJson.FromJson(json);
                var items = fbMarketplaceModel.Bbox.Result.Data.MarketplaceSearch.FeedUnits.Edges;

                // if search comes up empty return
                if (items.Length == 0)
                {
                    Logger.LogInformation("FbMarketplace scraper the listings are empty. FbMarketplaceUrl: {url}. FilterOptions: {FilterOptions}", url, FilterOptions);
                    return new List<Car>();
                }

                // add them to the list
                List<Car> carItems = new List<Car>();
                for (int i = 0; i < items.Length; i++)
                {
                    var customSubTitle = items[i].Node.Listing.CustomSubTitlesWithRenderingFlags.FirstOrDefault();

                    // add item
                    carItems.Add(new Car(
                        items[i].Node.Listing.MarketplaceListingTitle.HtmlDecode(), // title
                        $"{site}item/{items[i].Node.Listing.Id}", // link
                        items[i].Node.Listing.PrimaryListingPhoto.Image.Uri.AbsoluteUri, // image
                        items[i].Node.Listing.ListingPrice.FormattedAmount.ToInt(), // price
                        customSubTitle == null ? 0 : (int)(customSubTitle.Subtitle.ToFloat() * 1000), // kms
                        Source.FbMarketplace
                        ));
                }

                return carItems;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "FbMarketplace scraper has failed. FbMarketplaceUrl: {url}. FilterOptions: {FilterOptions}", url, FilterOptions);
                return new List<Car>();
            }
        }
    }
}
