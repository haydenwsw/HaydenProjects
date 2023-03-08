using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CarSearcher.Models.Json
{
    public class CarSearcherConfig
    {
        public string Folder { get; set; }
        public bool GenerateMakeModelFile { get; set; }
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

        public CarSearcherConfig()
        {
            // if the main folder doesn't exist create it
            Directory.CreateDirectory($"./{Folder}");
        }
    }
}
