using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHarvest.Models;

namespace AutoHarvest.Singletons
{
    /// <summary>
    /// gives the right logo to each listing
    /// </summary>
    public class LogoLookup
    {
        private readonly Dictionary<Source, Logo> Logos = new Dictionary<Source, Logo>
        {
            { Source.Carsales,  new Logo("~/logos/Carsales.png", "174", "64") },
            { Source.FbMarketplace,  new Logo("~/logos/FbMarketplace.png", "64", "64") },
            { Source.Gumtree,  new Logo("~/logos/Gumtree.jpg", "90", "64") }
        };

        // get the logo of the listings website that is was scraped from
        public Logo GetLogo(Source source)
        {
            Logos.TryGetValue(source, out Logo val);
            return val;
        }
    }
}
