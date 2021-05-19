using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHarvest.Scoped;
using AutoHarvest.Singleton;
using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AutoHarvest.Pages
{
    public class IndexModel : PageModel
    {
        // the search term
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        // the way the car listings are sorted
        [BindProperty(SupportsGet = true)]
        public int SortType { get; set; }

        // the car transmission
        [BindProperty(SupportsGet = true)]
        public int TransType { get; set; }

        // the page's number
        [BindProperty(SupportsGet = true)]
        public int PageNum { get; set; } = 1;

        // page number properties
        public bool ShowPrevious => PageNum > 1;
        public bool ShowNext => PageNum < 10;

        private readonly ILogger<IndexModel> _logger;

        public IEnumerable<Car> Cars { get; set; }

        public readonly CarWrapper CarWrapper;

        public readonly CarLookup CarLookup;

        // init
        public IndexModel(ILogger<IndexModel> logger, CarWrapper carwrapper, CarLookup carlookup)
        {
            _logger = logger;
            Cars = new List<Car>();
            CarWrapper = carwrapper;
            CarLookup = carlookup;
        }

        public async Task OnGet()
        {
            // searches for used cars
            if (SearchTerm != null)
            {
                Cars = await CarWrapper.getCarsAsync(new FilterOptions(SearchTerm, SortType, TransType), PageNum);
                //try
                //{
                //    Cars = CarWrapper.getCars(SearchTerm, PageNum, SortNum, TransNum);
                //}
                //catch (Exception e)
                //{
                //    // log the exception
                //    ErrorLogWriter.WriteLog(e);
                //}
            }
        }

        // get the logo of the listings website that is was scraped from
        public string getLogo(string source)
        {
            switch (source)
            {
                case "FbMarketplace":
                    return "~/logos/FbMarketplace.png";
                case "Gumtree":
                    return "~/logos/Gumtree.jpg";
                case "Carsales":
                    return "~/logos/Carsales.png";
                default:
                    return "";
            }
        }
    }
}
