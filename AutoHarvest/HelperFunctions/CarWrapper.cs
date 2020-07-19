using AutoHarvest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.HelperFunctions
{
    public static class CarWrapper
    {

        // get all the car listing from the websties
        public static List<Car> getCars(string searchTerm, uint page)
        {
            var Cars = new List<Car>();

            return Cars;
        }

        // get all the car listing from the websties asynchronously
        public static async Task<List<Car>> getCarsAsync(string searchTerm, uint page, uint SortNum, uint TransNum)
        {
            var Cars = new List<Car>();

            return Cars;
        }
    }
}
