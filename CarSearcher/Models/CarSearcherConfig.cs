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
        public string FacebookCookieFile { get; set; }
        public string FacebookX_ASBD_IDFile { get; set; }
        public string MarketplaceKeysFile { get; set; }
        public string UserAgent { get; set; }
        public bool EnableCarsales { get; set; }
        public bool EnableFbMarketplace { get; set; }
        public bool EnableGumtree { get; set; }

        public string FolderPath { get { return $"./{Folder}"; } }
        public string GetMakeModelPath { get { return $"{FolderPath}/{MakeModelFile}"; } }
        public string GetFacebookCookiePath { get { return $"{FolderPath}/{FacebookCookieFile}"; } }
        public string GetFacebookX_ASBD_IDPath { get { return $"{FolderPath}/{FacebookX_ASBD_IDFile}"; } }
        public string GetMarketplaceKeysPath { get { return $"{FolderPath}/{MarketplaceKeysFile}"; } }
    }
}
