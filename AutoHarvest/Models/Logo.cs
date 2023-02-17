using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Models
{
    /// <summary>
    /// model for the logo in the listing
    /// </summary>
    public class Logo
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
