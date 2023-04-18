using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HaydenProjects.Singletons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace HaydenProjects.Pages
{
    public class CarGuesserModel : PageModel
    {
        // the websites title
        public string Title;

        private readonly ILogger<CarGuesserModel> Logger;

        public CarGuesserModel(ILogger<CarGuesserModel> logger, Events events)
        {
            Logger = logger;
            Title = events.GetTitle("Car Guesser");
        }

        public void OnGet()
        {
        }
    }
}
