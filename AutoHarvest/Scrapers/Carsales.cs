using AutoHarvest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Scrapers
{
    // the class that webscrapes Carsales.com
    public class Carsales
    {
        // webscrape ebay for all the listings
        public static Task<List<Car>> ScrapeCarsales(string search, uint page, uint transNum)
        {
            throw new NotImplementedException();
        }
    }
}
