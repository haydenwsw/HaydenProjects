using CarSearcher.Models;
using HaydenProjects.Singletons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HaydenProjects.Pages.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarGuesserApi : ControllerBase
    {
        CarWrapper CarWrapper;

        public CarGuesserApi(CarWrapper carwrapper)
        {
            CarWrapper = carwrapper;
        }

        [HttpPost]
        public async Task<JsonResult> OnPost([MaxLength(25)] string body, [MaxLength(25)] string company, bool isEasyMode)
        {
            GuessCar guessCar = await CarWrapper.GuessCarAsync(body, company, isEasyMode);

            return new JsonResult(guessCar);
        }
    }
}
