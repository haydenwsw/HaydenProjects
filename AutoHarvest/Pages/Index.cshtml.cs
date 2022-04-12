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
using Microsoft.AspNetCore.Http;

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

        // the minimum price
        [BindProperty(SupportsGet = true)]
        public string PriceMin { get; set; }

        // the maxium price
        [BindProperty(SupportsGet = true)]
        public string PriceMax { get; set; }

        // the toggles for websites to scrape
        [BindProperty(SupportsGet = true)]
        public bool ToggleCarsales { get; set; } = true;

        [BindProperty(SupportsGet = true)]
        public bool ToggleFBMarketplace { get; set; } = true;

        [BindProperty(SupportsGet = true)]
        public bool ToggleGumtree { get; set; } = true;

        // the page's number
        [BindProperty(SupportsGet = true)]
        public int PageNum { get; set; } = 1;

        // page number properties
        public bool ShowPrevious => PageNum > 1;
        public bool ShowNext => PageNum < 10 && SearchTerm != null;

        // the icons for all the extra info
        public static readonly string[] Icons = new string[3] { "fa fa-car", "fa fa-cog", "fa fa-wrench" };

        // the websites title
        public string Title;

        private readonly ILogger<IndexModel> _logger;

        public IEnumerable<Car> Cars { get; set; }

        public readonly CarWrapper CarWrapper;

        public readonly LogoLookup LogoLookup;

        // init
        public IndexModel(ILogger<IndexModel> logger, CarWrapper carwrapper, LogoLookup logoLookup, Events events)
        {
            Title = events.GetTitle();
            _logger = logger;
            Cars = new List<Car>();
            CarWrapper = carwrapper;
            LogoLookup = logoLookup;
        }

        public async Task OnGet()
        {
            // searches for used cars
            if (SearchTerm != null)
            {
                FilterOptions filterOptions = new FilterOptions(SearchTerm, SortType, TransType, PriceMin.LeaveOnlyNumbers(), PriceMax.LeaveOnlyNumbers());
                Toggles toggles = new Toggles(ToggleCarsales, ToggleFBMarketplace, ToggleGumtree);

                Cars = await CarWrapper.getCarsAsync(filterOptions, toggles, PageNum);
            }
        }

        // get the logo of the listings website that is was scraped from
        public Logo getLogo(string source)
        {
            return LogoLookup.GetLogo(source);
        }
    }
}
