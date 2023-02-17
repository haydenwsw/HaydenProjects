using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Models
{
    public enum ExtraInfo
    {
        Body,
        Transmission,
        Engine
    }

    public enum Source
    {
        Carsales,
        FbMarketplace,
        Gumtree
    }

    /// <summary>
    /// Model class that holds the a car listing
    /// </summary>
    public class Car
    {
        public readonly string Name;
        public readonly string Link;
        public readonly string ImgUrl;
        public readonly int Price;
        public readonly int KMs;
        public readonly string[] ExtraInfo;
        public readonly Source Source;

        public Car(string name, string link, string imgurl, int price, int kms, Source source, string[] extrainfo = null)
        {
            Name = name;
            Link = link;
            ImgUrl = imgurl;
            Price = price;
            KMs = kms;
            ExtraInfo = extrainfo;
            Source = source;
        }
    }
}
