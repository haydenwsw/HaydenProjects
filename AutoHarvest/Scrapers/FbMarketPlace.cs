using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHarvest.Singleton;
using CefSharp.OffScreen;
using AutoHarvest.Pages;
using Microsoft.Extensions.Logging;

namespace AutoHarvest.Scrapers
{
    // the class that webscrapes facebook.com/marketplace
    public class FbMarketplace
    {
        // the urls for scraping
        private readonly string site = "https://www.facebook.com/marketplace/";
        private readonly string[] trans = { "", "&transmissionType=manual", "&transmissionType=automatic" };
        private readonly string[] sort = { "price_ascend", "price_descend", "", "", "" };

        private readonly ILogger<FbMarketplace> Logger;

        public FbMarketplace(ILogger<FbMarketplace> logger)
        {
            Logger = logger;
        }

        // webscrape FbMarketplace for all the listings
        public async Task<List<Car>> ScrapeFbMarketplace(ChromiumWebBrowser Tab, FilterOptions FilterOptions)
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
                string html = await CefSharpHeadless.GetHtmlAsybc(Tab, url);

                // Load HTML doc
                htmlDocument.LoadHtml(html);

                // gets the longest string in the list
                string script = htmlDocument.DocumentNode.Descendants("script").Aggregate("", (max, cur) => max.Length > cur.InnerText.Length ? max : cur.InnerText);

                // get the raw json from the script
                int idx = script.IndexOf("{\"__bbox");
                string json = script.Substring(idx, script.Length - idx - 12); // hard code values for good luck

                // deserialize into the model
                FbMarketplaceJson fbMarketplaceModel = FbMarketplaceJson.FromJson(json);
                var items = fbMarketplaceModel.Bbox.Result.Data.MarketplaceSearch.FeedUnits.Edges;

                // if search comes up empty return
                if (items.Length == 0)
                    return new List<Car>();

                // add them to the list
                List<Car> carItems = new List<Car>();
                for (int i = 0; i < items.Length; i++)
                {
                    var customSubTitle = items[i].Node.Listing.CustomSubTitlesWithRenderingFlags.FirstOrDefault();

                    // add item
                    carItems.Add(new Car(
                        items[i].Node.Listing.MarketplaceListingTitle, // title
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
