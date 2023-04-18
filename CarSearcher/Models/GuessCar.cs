using System;
using System.Collections.Generic;
using System.Text;

namespace CarSearcher.Models
{
    public class GuessCar
    {
        public string ImgUrl { get; set; }

        public int Answer { get; set; }

        public string[] Choices { get; set; }

        public GuessCar(string imgurl, int ans, string[] choices)
        {
            ImgUrl = imgurl;
            Answer = ans;
            Choices = choices;
        }
    }
}
