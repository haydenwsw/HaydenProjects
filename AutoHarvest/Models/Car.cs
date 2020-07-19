using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Models
{
    public class Car
    {
        public string Name { get; private set; }
        public string Link { get; private set; }
        public string ImgUrl { get; private set; }
        public uint Price { get; private set; }
        public uint KMs { get; private set; }
        public string Source { get; private set; }

        // init
        public Car(string name, string link, string imgurl, uint price, uint kms, string source)
        {
            Name = name;
            Link = link;
            ImgUrl = imgurl;
            Price = price;
            KMs = kms;
            Source = source;
        }
    }
}
