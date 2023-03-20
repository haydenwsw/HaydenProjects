using CarSearcher.Models;
using CarSearcher.ExtensionFunctions;
using CarSearcher.Exceptions;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Web;

namespace CarSearcher.Scrapers
{
    /// <summary>
    /// facebook.com/marketplace webscrape
    /// </summary>
    public partial class FbMarketplace
    {
        // the urls for scraping
        private readonly string Site = "https://www.facebook.com/marketplace/";
        private readonly string Url = "https://www.facebook.com/api/graphql/";

        // for Make Model selection
        private readonly FilterSortingParams[] SortBy = { new FilterSortingParams { SortByFilter = "PRICE_AMOUNT", SortOrder = "ASCEND" }, new FilterSortingParams { SortByFilter = "PRICE_AMOUNT", SortOrder = "DESCEND" }, new FilterSortingParams { SortByFilter = "CREATION_TIME", SortOrder = "DESCEND" }, new FilterSortingParams { SortByFilter = "VEHICLE_MILEAGE", SortOrder = "ASCEND" }, new FilterSortingParams { SortByFilter = "VEHICLE_MILEAGE", SortOrder = "DESCEND" } };
        private readonly NumericVerticalField[][] Transmission = { new NumericVerticalField[0], new NumericVerticalField[1] { new NumericVerticalField { Name = "is_manual_transmission", Value = 1 } }, new NumericVerticalField[1] { new NumericVerticalField { Name = "is_manual_transmission", Value = 0 } } };

        // for Search
        private readonly string[] SortBy2 = { "PRICE_ASCEND", "PRICE_DESCEND", "CREATION_TIME_DESCEND", "VEHICLE_MILEAGE_ASCEND", "VEHICLE_MILEAGE_DESCEND" };
        private readonly string[][] Transmission2 = { null, new string[1] { "MANUAL" }, new string[1] { "AUTOMATIC" } };

        // request data
        private readonly string Cookie = null;
        private readonly string X_ASBD_ID = null;
        private FbMarketplaceRequest MarketplaceRequest;
        private DateTime Date = DateTime.Now.Date;

        private readonly HttpClient HttpClient;
        private readonly CarLookup CarLookup;
        private readonly CarSearcherConfig CarSearcherConfig;

        public FbMarketplace(HttpClient httpclient, CarLookup carlookup, CarSearcherConfig carsearcherconfig)
        {
            HttpClient = httpclient;
            CarSearcherConfig = carsearcherconfig;
            CarLookup = carlookup;

            // get cookies for scraping
            if (File.Exists(CarSearcherConfig.GetFacebookCookiesPath))
            {
                string[] cookies = File.ReadAllLines(CarSearcherConfig.GetFacebookCookiesPath);
                Cookie = cookies[0];
                X_ASBD_ID = cookies[1];
            }
            else
                throw new FbMarketplaceException("The website cannot startup, missing Facebook cookies");

            // get the graph ql keys for scraping
            if (File.Exists(CarSearcherConfig.GetMarketplaceKeysPath))
            {
                string json = File.ReadAllText(CarSearcherConfig.GetMarketplaceKeysPath);
                MarketplaceRequest = JsonConvert.DeserializeObject<FbMarketplaceRequest>(json);
            }
            else
            {
                MarketplaceRequest = GetFbMarketplaceKeys().Result;
                string json = JsonConvert.SerializeObject(MarketplaceRequest, Formatting.Indented);
                File.WriteAllText(CarSearcherConfig.GetMarketplaceKeysPath, json);
            }
        }

        public async Task<List<Car>> ScrapeCars(FilterOptions filterOptions)
        {
            FbMarketplaceVariables fbMarketplaceVariables = new FbMarketplaceVariables();

            try
            {
                // if a day has passed or they're no keys regenerate them
                if (Date != DateTime.Now.Date || !File.Exists(CarSearcherConfig.GetMarketplaceKeysPath))
                {
                    MarketplaceRequest = await GetFbMarketplaceKeys();
                    string jsonKeys = JsonConvert.SerializeObject(MarketplaceRequest, Formatting.Indented);
                    File.WriteAllText(CarSearcherConfig.GetMarketplaceKeysPath, jsonKeys);

                    Date = DateTime.Now.Date;
                }

                // do price min max
                long priceMin = filterOptions.MinPrice == "" ? 0 : long.Parse(filterOptions.MinPrice);
                long priceMax = filterOptions.MaxPrice == "" ? 214748364700 : long.Parse(filterOptions.MaxPrice);

                if (filterOptions.SearchTerm == null)
                {
                    MarketplaceRequest.fb_api_req_friendly_name = "CometMarketplaceCategoryContentPaginationQuery";

                    // get the make model keys
                    string make = CarLookup.MakeModel[filterOptions.Make].Item1.FbMaketplaceKey;
                    string model = CarLookup.MakeModel[filterOptions.Make].Item2[filterOptions.Model].FbMaketplaceKey;

                    // do make model
                    ContextualDatum[] contextualData = new ContextualDatum[0];
                    ContextualDatum[] stringVerticalFields = new ContextualDatum[0];
                    if (filterOptions.Make != "All Makes")
                    {
                        contextualData = filterOptions.Model == "All Models" ? new ContextualDatum[1] { new ContextualDatum { Name = "make", Value = $"\"{make}\"" } } : new ContextualDatum[2] { new ContextualDatum { Name = "make", Value = $"\"{make}\"" }, new ContextualDatum { Name = "model", Value = $"\"{model}\"" } };
                        stringVerticalFields = filterOptions.Model == "All Models" ? new ContextualDatum[1] { new ContextualDatum { Name = "canonical_make_id", Value = make } } : new ContextualDatum[2] { new ContextualDatum { Name = "canonical_make_id", Value = make }, new ContextualDatum { Name = "canonical_model_id", Value = model } };
                    }

                    // do filter
                    FilterSortingParams filterSortingParams = filterOptions.GetSort(SortBy);

                    // do transmission
                    NumericVerticalField[] numericVerticalFields = filterOptions.GetTrans(Transmission);

                    // extra params
                    int count = 24;
                    TopicPageParams topicPageParams = new TopicPageParams { LocationId = "sydney", Url = "vehicles" };

                    // do page number and set keys to the request
                    string cursor = null;
                    if (filterOptions.PageNumber > 1)
                    {
                        cursor = $"{{\"basic\":{{\"item_index\":{(filterOptions.PageNumber - 1) * 24}}}}}";
                        MarketplaceRequest.doc_id = "4567898856668038";
                        contextualData = null;
                        count = 10; // maximum count per request
                        topicPageParams = null;
                    }
                    else
                    {
                        MarketplaceRequest.doc_id = "5459254634179638";
                    }

                    // set the varables to get make model
                    fbMarketplaceVariables = new FbMarketplaceVariables
                    {
                        BuyLocation = new BuyLocation { Latitude = -33.8675, Longitude = 151.208 },
                        CategoryIdArray = new long[1] { 807311116002614 },
                        ContextualData = contextualData,
                        Count = count,
                        Cursor = cursor,
                        FilterSortingParams = filterSortingParams,
                        MarketplaceBrowseContext = "CATEGORY_FEED",
                        NumericVerticalFields = numericVerticalFields,
                        NumericVerticalFieldsBetween = new object[0],
                        PriceRange = new long[2] { priceMin, priceMax },
                        Radius = 65000,
                        Scale = 1,
                        StringVerticalFields = stringVerticalFields,
                        TopicPageParams = topicPageParams
                    };
                }
                else
                {
                    Params paramss = new Params
                    {
                        Bqf = new Bqf
                        {
                            Callsite = "COMMERCE_MKTPLACE_WWW",
                            Query = filterOptions.SearchTerm
                        },
                        BrowseRequestParams = new BrowseRequestParams
                        {
                            CommerceEnableLocalPickup = true,
                            CommerceEnableShipping = true,
                            CommerceSearchAndRpAvailable = true,
                            CommerceSearchAndRpCategoryId = new object[0],
                            FilterLocationLatitude = -33.8675,
                            FilterLocationLongitude = 151.208,
                            FilterPriceLowerBound = priceMin,
                            FilterPriceUpperBound = priceMax,
                            FilterRadiusKm = 65,
                            VerticalFieldDiscreteAutoTransmissionTypeMulti = filterOptions.GetTrans(Transmission2),
                            CommerceSearchSortBy = filterOptions.GetSort(SortBy2)
                        },
                        CustomRequestParams = new CustomRequestParams
                        {
                            ContextualFilters = new object[0],
                            SearchVertical = "C2C",
                            Surface = "SEARCH",
                            VirtualContextualFilters = new object[0]
                        }
                    };

                    // set the varables to get search and set make model settings in the request
                    if (filterOptions.PageNumber > 1)
                    {
                        // todo cache the id (br) sent back or get one when needed
                        return new List<Car>();
                    }
                    else
                    {
                        MarketplaceRequest.fb_api_req_friendly_name = "CometMarketplaceSearchContentContainerQuery";
                        MarketplaceRequest.doc_id = "5240929566007459";

                        fbMarketplaceVariables = new FbMarketplaceVariables
                        {
                            BuyLocation = new BuyLocation { Latitude = -33.8675, Longitude = 151.208 },
                            Count = 24,
                            FlashSaleEventId = "",
                            HasFlashSaleEventId = false,
                            MarketplaceSearchMetadataCardEnabled = true,
                            Params = paramss,
                            SavedSearchQuery = filterOptions.SearchTerm,
                            Scale = 1,
                            ShouldIncludePopularSearches = false,
                            TopicPageParams = new TopicPageParams
                            {
                                LocationId = "sydney",
                                Url = null
                            },
                            VehicleParams = ""
                        };
                    }
                }

                MarketplaceRequest.variables = fbMarketplaceVariables.ToJson();

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Url);

                request.Headers.Add("User-Agent", CarSearcherConfig.UserAgent);
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                request.Headers.Add("X-FB-Friendly-Name", MarketplaceRequest.fb_api_req_friendly_name);
                request.Headers.Add("X-FB-LSD", MarketplaceRequest.lsd);
                request.Headers.Add("X-ASBD-ID", X_ASBD_ID);
                request.Headers.Add("Origin", "https://www.facebook.com");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("Cookie", Cookie);
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Site", "same-origin");
                request.Headers.Add("TE", "trailers");

                request.Content = new StringContent(MarketplaceRequest.ToString());
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();

                if (json == "")
                    return await Task.FromException<List<Car>>(new FbMarketplaceException($"FbMarketplace GetCars has failed. The request parameters are invaild, out dated cookies? FilterOptions: {fbMarketplaceVariables.ToJson()}"));

                FbMarketplaceResponse fbMarketplaceResponse = JsonConvert.DeserializeObject<FbMarketplaceResponse>(json);

                // get all the listings
                Edge[] edges = null;
                if (filterOptions.SearchTerm == null)
                    edges = fbMarketplaceResponse.Data.Viewer.MarketplaceFeedStories.Edges;
                else
                    edges = fbMarketplaceResponse.Data.MarketplaceSearch.FeedUnits.Edges;

                //if search comes up empty return
                if (edges.Length == 0)
                    return new List<Car>();

                // add them to the list
                List<Car> carItems = new List<Car>();
                for (int i = 0; i < edges.Length; i++)
                {
                    if (edges[i].Node.Listing == null)
                        continue;

                    var customSubTitle = edges[i].Node.Listing.CustomSubTitlesWithRenderingFlags.FirstOrDefault();

                    // add item
                    carItems.Add(new Car(
                        edges[i].Node.Listing.MarketplaceListingTitle.HtmlDecode(), // title
                        $"{Site}item/{edges[i].Node.Listing.Id}", // link
                        edges[i].Node.Listing.PrimaryListingPhoto.Image.Uri.AbsoluteUri, // image
                        edges[i].Node.Listing.ListingPrice.FormattedAmount.ToInt(), // price
                        customSubTitle == null ? 0 : (int)(customSubTitle.Subtitle.ToFloat() * 1000), // kms
                        Source.FbMarketplace
                        ));
                }

                return carItems;
            }
            catch (Exception ex)
            {
                return await Task.FromException<List<Car>>(new FbMarketplaceException($"FbMarketplace GetCars has failed. FilterOptions: {fbMarketplaceVariables.ToJson()}", ex));
            }
        }

        /// <summary>
        /// Gets all the makes and model from Facebook marketplace
        /// </summary>
        /// <param name="allMakeModel"></param>
        /// <returns></returns>
        public async Task GetMakeModel(Dictionary<string, (string, List<(string, string)>)> allMakeModel)
        {
            // set the varables to get all the makes
            FbMarketplaceVariablesMakeModel fbMarketplaceVariables = new FbMarketplaceVariablesMakeModel
            {
                BuyLocation = new BuyLocation { Latitude = -33.8675, Longitude = 151.208 },
                CategoryRankingEnabled = true,
                CategoryIds = new long[1] { 807311116002614 },
                ContextualData = new ContextualDatum[1] { new ContextualDatum { Name = "seo_url", Value = "\"vehicles\"" } },
                HideL2Cats = true,
                SavedSearchId = "",
                ShouldIncludeHeroBanner = false,
                ShouldIncludePopularSearches = false,
                Surface = "CATEGORY_FEED",
                TopicPageParams = new TopicPageParams { LocationId = "category", Url = "vehicles" },
                VirtualCategoryIds = new object[0]
            };

            MarketplaceRequest.variables = fbMarketplaceVariables.ToJson();
            MarketplaceRequest.fb_api_req_friendly_name = "CometMarketplaceSearchRootQuery";
            MarketplaceRequest.doc_id = "4980743115337835";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Url);

            request.Headers.Add("User-Agent", CarSearcherConfig.UserAgent);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("X-FB-Friendly-Name", "CometMarketplaceSearchRootQuery");
            request.Headers.Add("X-FB-LSD", MarketplaceRequest.lsd);
            request.Headers.Add("X-ASBD-ID", X_ASBD_ID);
            request.Headers.Add("Origin", "https://www.facebook.com");
            request.Headers.Add("DNT", "1");
            request.Headers.Add("Alt-Used", "www.facebook.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Referer", "https://www.facebook.com/marketplace/category/vehicles");
            request.Headers.Add("Cookie", Cookie);
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("TE", "trailers");

            request.Content = new StringContent(MarketplaceRequest.ToString());
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpResponseMessage response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();

            FbMarketplaceResponse fbMarketplaceResponse = JsonConvert.DeserializeObject<FbMarketplaceResponse>(json);

            MarketplaceStructuredField makes = fbMarketplaceResponse.Data.Viewer.MarketplaceStructuredFields.FirstOrDefault(x => x.FilterKey == "make");
            if (makes == null)
                throw new FbMarketplaceException("They're no makes");

            // loop through all the makes
            for (int i = 0; i < makes.Choices.Length; i++)
            {
                allMakeModel.TryAdd(makes.Choices[i].DisplayName.RemoveDiacritics(), (makes.Choices[i].Value, new List<(string, string)>()));
            }

            fbMarketplaceVariables.ContextualData = new ContextualDatum[2] { new ContextualDatum { Name = "make", Value = "" }, new ContextualDatum { Name = "seo_url", Value = "\"vehicles\"" } };

            // then loop through all the makes to get all the models
            foreach (KeyValuePair<string, (string, List<(string, string)>)> item in allMakeModel)
            {
                // set the varables to get all the models
                fbMarketplaceVariables.ContextualData.First().Value = $"\"{item.Value.Item1}\"";
                MarketplaceRequest.variables = fbMarketplaceVariables.ToJson();

                request = new HttpRequestMessage(HttpMethod.Post, Url);

                request.Headers.Add("User-Agent", CarSearcherConfig.UserAgent);
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                request.Headers.Add("X-FB-Friendly-Name", MarketplaceRequest.fb_api_req_friendly_name);
                request.Headers.Add("X-FB-LSD", MarketplaceRequest.lsd);
                request.Headers.Add("X-ASBD-ID", X_ASBD_ID);
                request.Headers.Add("Origin", "https://www.facebook.com");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Alt-Used", "www.facebook.com");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("Referer", "https://www.facebook.com/marketplace/category/vehicles");
                request.Headers.Add("Cookie", Cookie);
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Site", "same-origin");
                request.Headers.Add("TE", "trailers");

                request.Content = new StringContent(MarketplaceRequest.ToString());
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                // wait a bit so we don't spam the server
                await Task.Delay(500);

                response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                json = await response.Content.ReadAsStringAsync();

                fbMarketplaceResponse = JsonConvert.DeserializeObject<FbMarketplaceResponse>(json);

                MarketplaceStructuredField models = fbMarketplaceResponse.Data.Viewer.MarketplaceStructuredFields.FirstOrDefault(x => x.FilterKey == "model");
                if (models == null)
                    continue;

                // add model to the main list
                for (int i = 0; i < models.Choices.Length; i++)
                {
                    allMakeModel[item.Key].Item2.Add((models.Choices[i].Value, models.Choices[i].DisplayName.Replace($"{item.Key} ", "")));
                }
            }
        }

        /// <summary>
        /// gets the unique keys used for facebook market place graphql requests
        /// </summary>
        /// <returns></returns>
        private async Task<FbMarketplaceRequest> GetFbMarketplaceKeys()
        {
            try
            { 
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://www.facebook.com/marketplace/");

                request.Headers.Add("User-Agent", CarSearcherConfig.UserAgent);
                request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Alt-Used", "www.facebook.com");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("Cookie", Cookie);
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.Headers.Add("Sec-Fetch-Dest", "document");
                request.Headers.Add("Sec-Fetch-Mode", "navigate");
                request.Headers.Add("Sec-Fetch-Site", "cross-site");
                request.Headers.Add("TE", "trailers");

                HttpResponseMessage response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(responseBody);

                string[] scripts = htmlDocument.DocumentNode.Descendants("script").Select(x => x.InnerText).ToArray();

                string eqmc = htmlDocument.DocumentNode.Descendants("script").First(x => x.Id == "__eqmc").InnerText;

                FbMarketplaceKeys fbMarketplaceKeys = JsonConvert.DeserializeObject<FbMarketplaceKeys>(eqmc);
                NameValueCollection paramas = HttpUtility.ParseQueryString(fbMarketplaceKeys.U);

                string script = htmlDocument.DocumentNode.Descendants("script").First(x => x.InnerText.Contains("IntlCurrentLocale")).InnerText;

                // json to long and confusing to parse so using substring instead
                int idx = script.IndexOf("LSD");
                string lsd = script.Substring(idx + 18, 22);

                FbMarketplaceRequest fbMarketplaceRequest = new FbMarketplaceRequest
                {
                    av = paramas.Get("__user"),
                    dpr = "1",
                    fb_dtsg = fbMarketplaceKeys.F,
                    jazoest = paramas.Get("jazoest"),
                    lsd = lsd,
                    fb_api_caller_class = "RelayModern",
                    fb_api_req_friendly_name = null,
                    variables = null,
                    server_timestamps = "true",
                    doc_id = null
                };

                return fbMarketplaceRequest;
            }
            catch (Exception ex)
            {
                return await Task.FromException<FbMarketplaceRequest>(new FbMarketplaceException("Facebook marketplace has failed to get the keys", ex));
            }
        }

        #region FbMarketplaceKeys Model
        private class FbMarketplaceKeys
        {
            public string U { get; set; }
            public string E { get; set; }
            public string S { get; set; }
            public long W { get; set; }
            public string F { get; set; }
            public object L { get; set; }
        }
        #endregion
    }
}
