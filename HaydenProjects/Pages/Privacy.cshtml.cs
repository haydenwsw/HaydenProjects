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
    public class PrivacyModel : PageModel
    {
        // the websites title
        public string Title;

        public Events Events;

        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(Events events, ILogger<PrivacyModel> logger)
        {
            _logger = logger;
            Events = events;
            Title = Events.GetTitle("Privacy");
        }

        public void OnGet()
        {
            Events.TimesVisited++;
        }
    }
}
