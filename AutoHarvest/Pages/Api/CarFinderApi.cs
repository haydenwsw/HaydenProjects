using AutoHarvest.Singletons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Pages.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarFinderApi : ControllerBase
    {
        CarLookup CarLookup;

        public CarFinderApi(CarLookup carlookup)
        {
            CarLookup = carlookup;
        }

        // not a real post request but whatever is what i got working (probably didn't work because need cors)
        [HttpPost]
        public JsonResult OnPost([MaxLength(25)] string make)
        {
            if (CarLookup.MakeModel.ContainsKey(make))
            {
                return new JsonResult(CarLookup.MakeModel[make].Item2.Keys);
            }

            return new JsonResult(Ok());
        }
    }
}
