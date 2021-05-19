using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Singleton
{
    public class CarLookup
    {
        public Dictionary<string, string[]> MakeModel = new Dictionary<string, string[]>
        {
            // default
            { "All Makes", new string[0] },
            // Toyota
            { "Toyota", new string[2] { "Celica", "Camey" } },
            // BMW
            { "BMW", new string[2] { "E36", "E46" } }
        };
    }
}
