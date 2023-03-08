using Newtonsoft.Json;

namespace CarSearcher.Scrapers
{
    /// <summary>
    /// All the classes for the graph ql varables
    /// </summary>
    public partial class FbMarketplace
    {
        private class FbMarketplaceVariablesMakeModel
        {
            [JsonProperty("buyLocation")]
            public BuyLocation BuyLocation { get; set; }

            [JsonProperty("category_ids")]
            public long[] CategoryIds { get; set; }

            [JsonProperty("category_ranking_enabled")]
            public bool CategoryRankingEnabled { get; set; }

            [JsonProperty("contextual_data")]
            public ContextualDatum[] ContextualData { get; set; }

            [JsonProperty("hide_l2_cats")]
            public bool HideL2Cats { get; set; }

            [JsonProperty("savedSearchID")]
            public string SavedSearchId { get; set; }

            [JsonProperty("savedSearchQuery")]
            public object SavedSearchQuery { get; set; }

            [JsonProperty("sellerID")]
            public object SellerId { get; set; }

            [JsonProperty("shouldIncludeHeroBanner")]
            public bool ShouldIncludeHeroBanner { get; set; }

            [JsonProperty("shouldIncludePopularSearches")]
            public bool ShouldIncludePopularSearches { get; set; }

            [JsonProperty("surface")]
            public string Surface { get; set; }

            [JsonProperty("topicPageParams")]
            public TopicPageParams TopicPageParams { get; set; }

            [JsonProperty("virtual_category_ids")]
            public object[] VirtualCategoryIds { get; set; }

            public string ToJson()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        private class FbMarketplaceVariables
        {
            [JsonProperty("buyLocation")]
            public BuyLocation BuyLocation { get; set; }

            [JsonProperty("categoryIDArray")]
            public long[] CategoryIdArray { get; set; }

            [JsonProperty("contextual_data")]
            public ContextualDatum[] ContextualData { get; set; }

            [JsonProperty("count")]
            public long Count { get; set; }

            [JsonProperty("cursor")]
            public string Cursor { get; set; }

            [JsonProperty("flashSaleEventID")]
            public string FlashSaleEventId { get; set; }

            [JsonProperty("hasFlashSaleEventID")]
            public bool? HasFlashSaleEventId { get; set; }

            [JsonProperty("marketplaceSearchMetadataCardEnabled")]
            public bool? MarketplaceSearchMetadataCardEnabled { get; set; }

            [JsonProperty("params")]
            public Params Params { get; set; }

            [JsonProperty("savedSearchQuery")]
            public string SavedSearchQuery { get; set; }

            [JsonProperty("filterSortingParams")]
            public FilterSortingParams FilterSortingParams { get; set; }

            [JsonProperty("marketplaceBrowseContext")]
            public string MarketplaceBrowseContext { get; set; }

            [JsonProperty("numericVerticalFields")]
            public NumericVerticalField[] NumericVerticalFields { get; set; }

            [JsonProperty("numericVerticalFieldsBetween")]
            public object[] NumericVerticalFieldsBetween { get; set; }

            [JsonProperty("priceRange")]
            public long[] PriceRange { get; set; }

            [JsonProperty("radius")]
            public long Radius { get; set; }

            [JsonProperty("scale")]
            public long Scale { get; set; }

            [JsonProperty("shouldIncludePopularSearches")]
            public bool? ShouldIncludePopularSearches { get; set; }

            [JsonProperty("stringVerticalFields")]
            public ContextualDatum[] StringVerticalFields { get; set; }

            [JsonProperty("topicPageParams")]
            public TopicPageParams TopicPageParams { get; set; }

            [JsonProperty("vehicleParams")]
            public string VehicleParams { get; set; }

            public string ToJson()
            {
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
        }

        #region Inner Classes
        private class BuyLocation
        {
            [JsonProperty("latitude")]
            public double Latitude { get; set; }

            [JsonProperty("longitude")]
            public double Longitude { get; set; }
        }

        private class ContextualDatum
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        private class FilterSortingParams
        {
            [JsonProperty("sort_by_filter")]
            public string SortByFilter { get; set; }

            [JsonProperty("sort_order")]
            public string SortOrder { get; set; }
        }

        private class NumericVerticalField
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public long Value { get; set; }
        }

        private class TopicPageParams
        {
            [JsonProperty("location_id")]
            public string LocationId { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }
        #endregion
        #region Search Inner Classes
        private class Params
        {
            [JsonProperty("bqf")]
            public Bqf Bqf { get; set; }

            [JsonProperty("browse_request_params")]
            public BrowseRequestParams BrowseRequestParams { get; set; }

            [JsonProperty("custom_request_params")]
            public CustomRequestParams CustomRequestParams { get; set; }
        }

        private class Bqf
        {
            [JsonProperty("callsite")]
            public string Callsite { get; set; }

            [JsonProperty("query")]
            public string Query { get; set; }
        }

        private class BrowseRequestParams
        {
            [JsonProperty("commerce_enable_local_pickup")]
            public bool CommerceEnableLocalPickup { get; set; }

            [JsonProperty("commerce_enable_shipping")]
            public bool CommerceEnableShipping { get; set; }

            [JsonProperty("commerce_search_and_rp_available")]
            public bool CommerceSearchAndRpAvailable { get; set; }

            [JsonProperty("commerce_search_and_rp_category_id")]
            public object[] CommerceSearchAndRpCategoryId { get; set; }

            [JsonProperty("commerce_search_and_rp_condition")]
            public object CommerceSearchAndRpCondition { get; set; }

            [JsonProperty("commerce_search_and_rp_ctime_days")]
            public object CommerceSearchAndRpCtimeDays { get; set; }

            [JsonProperty("filter_location_latitude")]
            public double FilterLocationLatitude { get; set; }

            [JsonProperty("filter_location_longitude")]
            public double FilterLocationLongitude { get; set; }

            [JsonProperty("filter_price_lower_bound")]
            public long FilterPriceLowerBound { get; set; }

            [JsonProperty("filter_price_upper_bound")]
            public long FilterPriceUpperBound { get; set; }

            [JsonProperty("filter_radius_km")]
            public long FilterRadiusKm { get; set; }

            [JsonProperty("commerce_search_sort_by")]
            public string CommerceSearchSortBy { get; set; }

            [JsonProperty("vertical_field_discrete_auto_transmission_type_multi")]
            public string[] VerticalFieldDiscreteAutoTransmissionTypeMulti { get; set; }
        }

        private class CustomRequestParams
        {
            [JsonProperty("browse_context")]
            public object BrowseContext { get; set; }

            [JsonProperty("contextual_filters")]
            public object[] ContextualFilters { get; set; }

            [JsonProperty("referral_code")]
            public object ReferralCode { get; set; }

            [JsonProperty("saved_search_strid")]
            public object SavedSearchStrid { get; set; }

            [JsonProperty("search_vertical")]
            public string SearchVertical { get; set; }

            [JsonProperty("seo_url")]
            public object SeoUrl { get; set; }

            [JsonProperty("surface")]
            public string Surface { get; set; }

            [JsonProperty("virtual_contextual_filters")]
            public object[] VirtualContextualFilters { get; set; }
        }
        #endregion
    }
}
