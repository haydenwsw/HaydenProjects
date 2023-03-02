using Newtonsoft.Json;
using System;

namespace AutoHarvest.Scrapers
{
    public partial class FbMarketplace
    {
        private class FbMarketplaceResponse
        {
            [JsonProperty("data")]
            public Data Data { get; set; }

            [JsonProperty("extensions")]
            public Extensions Extensions { get; set; }
        }

        private class Data
        {
            [JsonProperty("marketplace_search")]
            public MarketplaceSearch MarketplaceSearch { get; set; }

            [JsonProperty("viewer")]
            public Viewer Viewer { get; set; }

            [JsonProperty("vehicle_model_query")]
            public object VehicleModelQuery { get; set; }

            [JsonProperty("flash_sale_event")]
            public object FlashSaleEvent { get; set; }

            [JsonProperty("marketplace_seo_page")]
            public MarketplaceSeoPage MarketplaceSeoPage { get; set; }
        }

        private class MarketplaceSearch
        {
            [JsonProperty("feed_units")]
            public FeedUnits FeedUnits { get; set; }
        }

        private class FeedUnits
        {
            [JsonProperty("edges")]
            public Edge[] Edges { get; set; }

            [JsonProperty("page_info")]
            public PageInfo PageInfo { get; set; }

            [JsonProperty("session_id")]
            public string SessionId { get; set; }

            [JsonProperty("logging_unit_id")]
            public string LoggingUnitId { get; set; }
        }

        private class MarketplaceSeoPage
        {
            [JsonProperty("__typename")]
            public string Typename { get; set; }

            [JsonProperty("seo_localized_page_title")]
            public string SeoLocalizedPageTitle { get; set; }

            [JsonProperty("seo_publish_state")]
            public string SeoPublishState { get; set; }

            [JsonProperty("virtual_category")]
            public VirtualCategory VirtualCategory { get; set; }

            [JsonProperty("seo_navigation")]
            public SeoNavigation[] SeoNavigation { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class SeoNavigation
        {
            [JsonProperty("__typename")]
            public string Typename { get; set; }

            [JsonProperty("seo_url")]
            public string SeoUrl { get; set; }

            [JsonProperty("seo_page_is_geo_agnostic")]
            public bool SeoPageIsGeoAgnostic { get; set; }

            [JsonProperty("seo_localized_page_title")]
            public string SeoLocalizedPageTitle { get; set; }

            [JsonProperty("virtual_category")]
            public VirtualCategory VirtualCategory { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class VirtualCategory
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("taxonomy_path")]
            public TaxonomyPath[] TaxonomyPath { get; set; }

            [JsonProperty("virtual_taxonomy_publish_state")]
            public string VirtualTaxonomyPublishState { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class TaxonomyPath
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("seo_info")]
            public SeoInfo SeoInfo { get; set; }

            [JsonProperty("virtual_taxonomy_publish_state")]
            public string VirtualTaxonomyPublishState { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("icon_name")]
            public string IconName { get; set; }
        }

        private class SeoInfo
        {
            [JsonProperty("seo_url")]
            public string SeoUrl { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class Viewer
        {
            [JsonProperty("marketplace_ranked_categories")]
            public MarketplaceRankedCategories MarketplaceRankedCategories { get; set; }

            [JsonProperty("bsg_suppression_holdout_marketplace_cvds")]
            public BsgSuppressionHoldoutMarketplaceCvds BsgSuppressionHoldoutMarketplaceCvds { get; set; }

            [JsonProperty("marketplace_structured_fields")]
            public MarketplaceStructuredField[] MarketplaceStructuredFields { get; set; }

            [JsonProperty("marketplace_seo_filters")]
            public MarketplaceSeoFilters MarketplaceSeoFilters { get; set; }

            [JsonProperty("marketplace_feed_stories")]
            public MarketplaceFeedStories MarketplaceFeedStories { get; set; }

            [JsonProperty("buy_location")]
            public ViewerBuyLocation BuyLocation { get; set; }

            [JsonProperty("marketplace_settings")]
            public MarketplaceSettings MarketplaceSettings { get; set; }

            [JsonProperty("marketplace_saved_searches")]
            public MarketplaceSavedSearches MarketplaceSavedSearches { get; set; }
        }

        private class BsgSuppressionHoldoutMarketplaceCvds
        {
            [JsonProperty("if_viewer_can_see_bsg_listings_in_mp")]
            public IfViewerCanSeeBsgListingsInMp IfViewerCanSeeBsgListingsInMp { get; set; }
        }

        private class IfViewerCanSeeBsgListingsInMp
        {
            [JsonProperty("__typename")]
            public string Typename { get; set; }
        }

        private class MarketplaceRankedCategories
        {
            [JsonProperty("prediction_id")]
            public string PredictionId { get; set; }

            [JsonProperty("categories_virtual_taxonomy")]
            public TaxonomyPath[] CategoriesVirtualTaxonomy { get; set; }
        }

        private class MarketplaceSeoFilters
        {
            [JsonProperty("filters")]
            public string Filters { get; set; }
        }

        private class MarketplaceStructuredField
        {
            [JsonProperty("__typename")]
            public string Typename { get; set; }

            [JsonProperty("filter_type")]
            public string FilterType { get; set; }

            [JsonProperty("filter_key")]
            public string FilterKey { get; set; }

            [JsonProperty("should_render_as_sub_filter")]
            public bool ShouldRenderAsSubFilter { get; set; }

            [JsonProperty("__isMarketplaceStructuredField")]
            public string IsMarketplaceStructuredField { get; set; }

            [JsonProperty("display_name")]
            public string DisplayName { get; set; }

            [JsonProperty("choices")]
            public Choice[] Choices { get; set; }

            [JsonProperty("field_key")]
            public string FieldKey { get; set; }

            [JsonProperty("child_filters")]
            public string[] ChildFilters { get; set; }

            [JsonProperty("clear_fields")]
            public object ClearFields { get; set; }

            [JsonProperty("range")]
            public Range Range { get; set; }
        }

        private class Choice
        {
            [JsonProperty("display_name")]
            public string DisplayName { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("active")]
            public bool? Active { get; set; }

            [JsonProperty("seo_navigable_filters")]
            public string SeoNavigableFilters { get; set; }

            [JsonProperty("slug")]
            public string Slug { get; set; }

            [JsonProperty("virtual_attribute_value")]
            public object VirtualAttributeValue { get; set; }

            [JsonProperty("contextual_filters")]
            public object ContextualFilters { get; set; }
        }

        private class Range
        {
            [JsonProperty("filter_key_max")]
            public string FilterKeyMax { get; set; }

            [JsonProperty("filter_key_min")]
            public string FilterKeyMin { get; set; }
        }

        private class ViewerBuyLocation
        {
            [JsonProperty("buy_location")]
            public BuyLocationBuyLocation BuyLocation { get; set; }
        }

        private class BuyLocationBuyLocation
        {
            [JsonProperty("location")]
            public BuyLocationLocation Location { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class BuyLocationLocation
        {
            [JsonProperty("reverse_geocode")]
            public PurpleReverseGeocode ReverseGeocode { get; set; }
        }

        private class PurpleReverseGeocode
        {
            [JsonProperty("city")]
            public string City { get; set; }
        }

        private class MarketplaceFeedStories
        {
            [JsonProperty("edges")]
            public Edge[] Edges { get; set; }

            [JsonProperty("page_info")]
            public PageInfo PageInfo { get; set; }
        }

        private class Edge
        {
            [JsonProperty("node")]
            public Node Node { get; set; }

            [JsonProperty("cursor")]
            public string Cursor { get; set; }

            [JsonProperty("__typename")]
            public string Typename { get; set; }
        }

        private class Node
        {
            [JsonProperty("__typename")]
            public string Typename { get; set; }

            [JsonProperty("story_type")]
            public string StoryType { get; set; }

            [JsonProperty("story_key")]
            public string StoryKey { get; set; }

            [JsonProperty("tracking")]
            public string Tracking { get; set; }

            [JsonProperty("listing")]
            public Listing Listing { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class Listing
        {
            [JsonProperty("__typename")]
            public string Typename { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("primary_listing_photo")]
            public PrimaryListingPhoto PrimaryListingPhoto { get; set; }

            [JsonProperty("__isMarketplaceListingRenderable")]
            public string IsMarketplaceListingRenderable { get; set; }

            [JsonProperty("listing_price")]
            public ListingPrice ListingPrice { get; set; }

            [JsonProperty("strikethrough_price")]
            public StrikethroughPrice StrikethroughPrice { get; set; }

            [JsonProperty("__isMarketplaceListingWithComparablePrice")]
            public string IsMarketplaceListingWithComparablePrice { get; set; }

            [JsonProperty("comparable_price")]
            public object ComparablePrice { get; set; }

            [JsonProperty("comparable_price_type")]
            public object ComparablePriceType { get; set; }

            [JsonProperty("location")]
            public ListingLocation Location { get; set; }

            [JsonProperty("is_hidden")]
            public bool IsHidden { get; set; }

            [JsonProperty("is_live")]
            public bool IsLive { get; set; }

            [JsonProperty("is_pending")]
            public bool IsPending { get; set; }

            [JsonProperty("is_sold")]
            public bool IsSold { get; set; }

            [JsonProperty("is_viewer_seller")]
            public bool IsViewerSeller { get; set; }

            [JsonProperty("min_listing_price")]
            public object MinListingPrice { get; set; }

            [JsonProperty("max_listing_price")]
            public object MaxListingPrice { get; set; }

            [JsonProperty("marketplace_listing_category_id")]
            public string MarketplaceListingCategoryId { get; set; }

            [JsonProperty("marketplace_listing_title")]
            public string MarketplaceListingTitle { get; set; }

            [JsonProperty("custom_title")]
            public string CustomTitle { get; set; }

            [JsonProperty("custom_sub_titles_with_rendering_flags")]
            public CustomSubTitlesWithRenderingFlag[] CustomSubTitlesWithRenderingFlags { get; set; }

            [JsonProperty("origin_group")]
            public object OriginGroup { get; set; }

            [JsonProperty("pre_recorded_videos")]
            public object[] PreRecordedVideos { get; set; }

            [JsonProperty("__isMarketplaceListingWithChildListings")]
            public string IsMarketplaceListingWithChildListings { get; set; }

            [JsonProperty("parent_listing")]
            public object ParentListing { get; set; }

            [JsonProperty("marketplace_listing_seller")]
            public MarketplaceListingSeller MarketplaceListingSeller { get; set; }

            [JsonProperty("__isMarketplaceListingWithDeliveryOptions")]
            public string IsMarketplaceListingWithDeliveryOptions { get; set; }

            [JsonProperty("delivery_types")]
            public string[] DeliveryTypes { get; set; }
        }

        private class CustomSubTitlesWithRenderingFlag
        {
            [JsonProperty("subtitle")]
            public string Subtitle { get; set; }
        }

        private class ListingPrice
        {
            [JsonProperty("formatted_amount")]
            public string FormattedAmount { get; set; }

            [JsonProperty("amount_with_offset_in_currency")]
            public string AmountWithOffsetInCurrency { get; set; }

            [JsonProperty("amount")]
            public string Amount { get; set; }
        }

        private class ListingLocation
        {
            [JsonProperty("reverse_geocode")]
            public FluffyReverseGeocode ReverseGeocode { get; set; }
        }

        private class FluffyReverseGeocode
        {
            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("state")]
            public string State { get; set; }

            [JsonProperty("city_page")]
            public CityPage CityPage { get; set; }
        }

        private class CityPage
        {
            [JsonProperty("display_name")]
            public string DisplayName { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class MarketplaceListingSeller
        {
            [JsonProperty("__typename")]
            public string Typename { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class PrimaryListingPhoto
        {
            [JsonProperty("__typename")]
            public string Typename { get; set; }

            [JsonProperty("image")]
            public Image Image { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class Image
        {
            [JsonProperty("uri")]
            public Uri Uri { get; set; }
        }

        private class StrikethroughPrice
        {
            [JsonProperty("formatted_amount")]
            public string FormattedAmount { get; set; }

            [JsonProperty("amount")]
            public string Amount { get; set; }
        }

        private class PageInfo
        {
            [JsonProperty("end_cursor")]
            public string EndCursor { get; set; }

            [JsonProperty("has_next_page")]
            public bool HasNextPage { get; set; }
        }

        private class MarketplaceSavedSearches
        {
            [JsonProperty("edges")]
            public object[] Edges { get; set; }
        }

        private class MarketplaceSettings
        {
            [JsonProperty("current_marketplace")]
            public CurrentMarketplace CurrentMarketplace { get; set; }
        }

        private class CurrentMarketplace
        {
            [JsonProperty("__typename")]
            public string Typename { get; set; }

            [JsonProperty("is_metric_base")]
            public bool IsMetricBase { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        private class Extensions
        {
            [JsonProperty("is_final")]
            public bool IsFinal { get; set; }
        }
    }
}
