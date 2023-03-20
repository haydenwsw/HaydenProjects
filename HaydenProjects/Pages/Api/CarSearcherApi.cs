using CarSearcher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HaydenProjects.Pages.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarSearcherApi : ControllerBase
    {
        CarLookup CarLookup;

        public CarSearcherApi(CarLookup carlookup)
        {
            CarLookup = carlookup;
        }

        // not a real post request but whatever is what i got working (probably didn't work because need cors)
        [HttpPost]
        public JsonResult OnPost([MaxLength(25)] string make)
        {
            if (make != null && CarLookup.MakeModel.ContainsKey(make))
            {
                // skip the "All Models" string and return
                return new JsonResult(CarLookup.MakeModel[make].Item2.Keys.Skip(1));
            }

            return new JsonResult(Ok());
        }
    }
}
