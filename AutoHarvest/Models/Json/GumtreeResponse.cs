using Newtonsoft.Json;
using System;

namespace AutoHarvest.Scrapers
{
    public partial class Gumtree
    {
        private class GumtreeResponce
        {
            [JsonProperty("data")]
            public GumtreeResponceData Data { get; set; }
        }

        private class GumtreeResponceData
        {
            [JsonProperty("results")]
            public Results Results { get; set; }
        }

        private class Results
        {           
            [JsonProperty("resultList")]
            public ResultList[] ResultList { get; set; }
        }

        private class ResultList
        {
            [JsonProperty("needsPostCodeOnReply")]
            public bool NeedsPostCodeOnReply { get; set; }

            [JsonProperty("simpleAge")]
            public string SimpleAge { get; set; }

            [JsonProperty("isAutosCategory")]
            public bool IsAutosCategory { get; set; }

            [JsonProperty("isFree")]
            public bool IsFree { get; set; }

            [JsonProperty("review")]
            public object Review { get; set; }

            [JsonProperty("componentName")]
            public string ComponentName { get; set; }

            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("isFeatured")]
            public bool IsFeatured { get; set; }

            [JsonProperty("locationArea")]
            public string LocationArea { get; set; }

            [JsonProperty("locationState")]
            public string LocationState { get; set; }

            [JsonProperty("mainImageUrl")]
            public Uri MainImageUrl { get; set; }

            [JsonProperty("extraImageUrls")]
            public Uri[] ExtraImageUrls { get; set; }

            [JsonProperty("fileUploadAllowed")]
            public bool FileUploadAllowed { get; set; }

            [JsonProperty("needsPhoneOnReply")]
            public bool NeedsPhoneOnReply { get; set; }

            [JsonProperty("isOnWatchlist")]
            public bool IsOnWatchlist { get; set; }

            [JsonProperty("logoUrl")]
            public string LogoUrl { get; set; }

            [JsonProperty("jobsAttributes")]
            public object JobsAttributes { get; set; }

            [JsonProperty("isJobsCategory")]
            public bool IsJobsCategory { get; set; }

            [JsonProperty("isWanted")]
            public bool IsWanted { get; set; }

            [JsonProperty("businessServiceSlogan")]
            public string BusinessServiceSlogan { get; set; }

            [JsonProperty("isPostedByCarDealer")]
            public bool IsPostedByCarDealer { get; set; }

            [JsonProperty("isDriveAway")]
            public bool IsDriveAway { get; set; }

            [JsonProperty("autosAttributes")]
            public AutosAttributes AutosAttributes { get; set; }

            [JsonProperty("isPremium")]
            public bool IsPremium { get; set; }

            [JsonProperty("thumbnailAltText")]
            public string ThumbnailAltText { get; set; }

            [JsonProperty("previousPriceString")]
            public string PreviousPriceString { get; set; }

            [JsonProperty("isUrgent")]
            public bool IsUrgent { get; set; }

            [JsonProperty("distance")]
            public string Distance { get; set; }

            [JsonProperty("mainAttributes")]
            public MainAttributes MainAttributes { get; set; }

            [JsonProperty("sellerName")]
            public string SellerName { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("isDeletedAd")]
            public bool IsDeletedAd { get; set; }

            [JsonProperty("viewableVipAd")]
            public bool ViewableVipAd { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("isCarsCategory")]
            public bool IsCarsCategory { get; set; }

            [JsonProperty("isPriceDrop")]
            public bool IsPriceDrop { get; set; }

            [JsonProperty("zoomBaseLogoUrl")]
            public string ZoomBaseLogoUrl { get; set; }

            [JsonProperty("isSwapTrade")]
            public bool IsSwapTrade { get; set; }

            [JsonProperty("adType")]
            public string AdType { get; set; }

            [JsonProperty("replyMessage")]
            public string ReplyMessage { get; set; }

            [JsonProperty("extraImageBaseUrls")]
            public Uri[] ExtraImageBaseUrls { get; set; }

            [JsonProperty("carVin")]
            public string CarVin { get; set; }

            [JsonProperty("priceText")]
            public string PriceText { get; set; }

            [JsonProperty("isB2CPlus")]
            public bool IsB2CPlus { get; set; }

            [JsonProperty("mainImageZoomBaseUrl")]
            public Uri MainImageZoomBaseUrl { get; set; }

            [JsonProperty("isHighlighted")]
            public bool IsHighlighted { get; set; }

            [JsonProperty("carRecordEnabled")]
            public bool CarRecordEnabled { get; set; }

            [JsonProperty("priceType")]
            public string PriceType { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("isNegotiable")]
            public bool IsNegotiable { get; set; }

            [JsonProperty("extraImageZoomBaseUrls")]
            public Uri[] ExtraImageZoomBaseUrls { get; set; }

            [JsonProperty("carRecordReportUrl")]
            public string CarRecordReportUrl { get; set; }

            [JsonProperty("location")]
            public string Location { get; set; }

            [JsonProperty("age")]
            public string Age { get; set; }

            [JsonProperty("categoryId")]
            public long CategoryId { get; set; }

            [JsonProperty("mainImageBaseUrl")]
            public Uri MainImageBaseUrl { get; set; }
        }

        private class AutosAttributes
        {
            [JsonProperty("data")]
            public AutosAttributesData Data { get; set; }
        }

        private class AutosAttributesData
        {
            [JsonProperty("forsaleby")]
            public string Forsaleby { get; set; }
        }

        private class MainAttributes
        {
            [JsonProperty("data")]
            public MainAttributesData Data { get; set; }
        }

        private class MainAttributesData
        {
            [JsonProperty("engine_capacity_litres", NullValueHandling = NullValueHandling.Ignore)]
            public string EngineCapacityLitres { get; set; }

            [JsonProperty("cylinder_configuration", NullValueHandling = NullValueHandling.Ignore)]
            public string CylinderConfiguration { get; set; }

            [JsonProperty("carmileageinkms")]
            public string Carmileageinkms { get; set; }

            [JsonProperty("carbodytype")]
            public string Carbodytype { get; set; }

            [JsonProperty("cartransmission")]
            public string Cartransmission { get; set; }

            [JsonProperty("highest_price", NullValueHandling = NullValueHandling.Ignore)]
            public string HighestPrice { get; set; }
        }
    }
}
