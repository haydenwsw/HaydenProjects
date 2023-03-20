using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CarSearcher.Models
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

        public string FolderPath { get { return $"./{Folder}"; } }
        public string GetMakeModelPath { get { return $"{FolderPath}/{MakeModelFile}"; } }
        public string GetFacebookCookiesPath { get { return $"{FolderPath}/{FacebookCookiesFile}"; } }
        public string GetMarketplaceKeysPath { get { return $"{FolderPath}/{MarketplaceKeysFile}"; } }
    }
}
