using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHarvest.Models;

namespace AutoHarvest.Singleton
{
    /// <summary>
    /// 
    /// </summary>
    public class LogoLookup
    {
        private readonly Dictionary<string, Logo> Logos = new Dictionary<string, Logo>
        {
            { "Carsales",  new Logo("~/logos/Carsales.png", "174", "64") },
            { "FbMarketplace",  new Logo("~/logos/FbMarketplace.png", "64", "64") },
            { "Gumtree",  new Logo("~/logos/Gumtree.jpg", "90", "64") }
        };

        // get the logo of the listings website that is was scraped from
        public Logo GetLogo(string source)
        {
            Logos.TryGetValue(source, out Logo val);
            return val;
        }
    }
}
