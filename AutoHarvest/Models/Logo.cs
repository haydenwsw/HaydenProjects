using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Models
{
    public struct Logo
    {
        public string LogoUrl;
        public string LogoWidth;
        public string LogoHeight;

        public Logo(string logoUrl = "", string logoWidth = "", string logoHeight = "")
        {
            LogoUrl = logoUrl;
            LogoWidth = logoWidth;
            LogoHeight = logoHeight;
        }
    }
}
