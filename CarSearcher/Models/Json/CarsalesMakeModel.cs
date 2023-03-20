using Newtonsoft.Json;

namespace CarSearcher.Scrapers
{
    public partial class Carsales
    {
        private class CarsalesMakeModel
        {
            [JsonProperty("version")]
            public long Version { get; set; }

            [JsonProperty("heading")]
            public string Heading { get; set; }

            [JsonProperty("subHeading")]
            public string SubHeading { get; set; }

            [JsonProperty("searchButtonUrl")]
            public string SearchButtonUrl { get; set; }

            [JsonProperty("searchButtonText")]
            public string SearchButtonText { get; set; }

            [JsonProperty("disableSearch")]
            public bool DisableSearch { get; set; }

            [JsonProperty("expression")]
            public string Expression { get; set; }

            [JsonProperty("tenant")]
            public string Tenant { get; set; }

            [JsonProperty("searchUrl")]
            public string SearchUrl { get; set; }

            [JsonProperty("formReset")]
            public bool FormReset { get; set; }

            [JsonProperty("primaryFields")]
            public PrimaryField[] PrimaryFields { get; set; }

            [JsonProperty("secondaryFields")]
            public SecondaryField[] SecondaryFields { get; set; }
        }

        private class PrimaryField
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("defaultValue")]
            public string DefaultValue { get; set; }

            [JsonProperty("values")]
            public Value[] Values { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("expression")]
            public string Expression { get; set; }

            [JsonProperty("removeAction")]
            public string RemoveAction { get; set; }

            [JsonProperty("noValueText")]
            public string NoValueText { get; set; }

            [JsonProperty("emphasis")]
            public bool Emphasis { get; set; }
        }

        private class Value
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("displayValue", NullValueHandling = NullValueHandling.Ignore)]
            public string DisplayValue { get; set; }

            [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
            public string ValueValue { get; set; }
        }

        private class SecondaryField
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore)]
            public string DefaultValue { get; set; }

            [JsonProperty("fromTitle", NullValueHandling = NullValueHandling.Ignore)]
            public string FromTitle { get; set; }

            [JsonProperty("toTitle", NullValueHandling = NullValueHandling.Ignore)]
            public string ToTitle { get; set; }

            [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
            public Value[] Values { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("expression")]
            public string Expression { get; set; }

            [JsonProperty("removeAction")]
            public string RemoveAction { get; set; }

            [JsonProperty("emphasis")]
            public bool Emphasis { get; set; }

            [JsonProperty("noValueText", NullValueHandling = NullValueHandling.Ignore)]
            public string NoValueText { get; set; }
        }
    }
}
