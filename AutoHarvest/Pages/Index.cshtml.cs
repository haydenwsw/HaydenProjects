using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AutoHarvest.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public uint SortNum { get; set; }

        [BindProperty(SupportsGet = true)]
        public uint TransNum { get; set; }

        [BindProperty(SupportsGet = true)]
        public uint PageNum { get; set; } = 1;

        public bool ShowPrevious => PageNum > 1;
        public bool ShowNext => PageNum < 10;

        public IEnumerable<Car> Cars { get; set; }

        private readonly ILogger<IndexModel> _logger;

        // init
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            Cars = new List<Car>();
        }

        public void OnGet()
        {
            // searches for used cars
            if (SearchTerm != null)
            {
                Cars = CarWrapper.getCars(SearchTerm, PageNum, SortNum, TransNum);
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
                case "FbMarketPlace":
                    return "~/logos/FbMarketPlace.png";
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
