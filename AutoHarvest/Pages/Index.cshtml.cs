using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHarvest.Scoped;
using AutoHarvest.HelperFunctions;
using AutoHarvest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AutoHarvest.Pages
{
    public class IndexModel : PageModel
    {
        //[BindProperty(SupportsGet = true)]
        //public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int MakeType { get; set; }

        [BindProperty(SupportsGet = true)]
        public int ModelType { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SortType { get; set; }

        [BindProperty(SupportsGet = true)]
        public int TransType { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNum { get; set; } = 1;

        public bool ShowPrevious => PageNum > 1;
        public bool ShowNext => PageNum < 10;

        public IEnumerable<Car> Cars { get; set; }

        private readonly ILogger<IndexModel> _logger;

        private readonly CarWrapper CarWrapper;

        // init
        public IndexModel(ILogger<IndexModel> logger, CarWrapper carwrapper)
        {
            _logger = logger;
            Cars = new List<Car>();
            CarWrapper = carwrapper;
        }

        public async Task OnGet()
        {
            // searches for used cars
            if (MakeType != 0)
            {
                Cars = await CarWrapper.getCarsAsync("celica", PageNum, new FilterOptions(SortType, TransType));
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
