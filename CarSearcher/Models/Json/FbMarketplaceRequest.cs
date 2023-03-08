using System.Web;

namespace CarSearcher.Scrapers
{
    public partial class FbMarketplace
    {
        private class FbMarketplaceRequest
        {
            public string av;
            public string dpr;
            public string fb_dtsg;
            public string jazoest;
            public string lsd;
            public string fb_api_caller_class;
            public string fb_api_req_friendly_name;
            public string variables;
            public string server_timestamps;
            public string doc_id;

            public override string ToString()
            {
                return $"av={HttpUtility.UrlEncode(av)}" +
                    $"&dpr={HttpUtility.UrlEncode(dpr)}" +
                    $"&fb_dtsg={HttpUtility.UrlEncode(fb_dtsg)}" +
                    $"&jazoest={HttpUtility.UrlEncode(jazoest)}" +
                    $"&lsd={HttpUtility.UrlEncode(lsd)}" +
                    $"&fb_api_caller_class={HttpUtility.UrlEncode(fb_api_caller_class)}" +
                    $"&fb_api_req_friendly_name={HttpUtility.UrlEncode(fb_api_req_friendly_name)}" +
                    $"&variables={HttpUtility.UrlEncode(variables)}" +
                    $"&server_timestamps={HttpUtility.UrlEncode(server_timestamps)}" +
                    $"&doc_id={HttpUtility.UrlEncode(doc_id)}";
            }
        }
    }
}
