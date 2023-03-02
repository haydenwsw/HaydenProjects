using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Models.Json
{
    public class CarFinder
    {
        public string Folder { get; set; }
        public string MakeModelFile { get; set; }
        public string FacebookCookiesFile { get; set; }
        public string MarketplaceKeysFile { get; set; }
        public string UserAgent { get; set; }
        public bool EnableCarsales { get; set; }
        public bool EnableFbMarketplace { get; set; }
        public bool EnableGumtree { get; set; }

        public string GetMakeModelPath { get { return $"./{Folder}/{MakeModelFile}"; } }
        public string GetFacebookCookiesPath { get { return $"./{Folder}/{FacebookCookiesFile}"; } }
        public string GetMarketplaceKeysPath { get { return $"./{Folder}/{MarketplaceKeysFile}"; } }
    }
}
