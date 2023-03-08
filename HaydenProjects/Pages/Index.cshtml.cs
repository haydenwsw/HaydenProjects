using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaydenProjects.Singletons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace HaydenProjects.Pages
{
    public class IndexModel : PageModel
    {
        // the websites title
        public string Title;

        private readonly ILogger<IndexModel> Logger;

        public IndexModel(ILogger<IndexModel> logger, Events events)
        {
            Logger = logger;
            Title = events.GetTitle("Hayden Projects");
        }

        public void OnGet()
        {
        }
    }
}
