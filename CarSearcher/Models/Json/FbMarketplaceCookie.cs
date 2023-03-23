
namespace CarSearcher.Scrapers
{
    public partial class FbMarketplace
    {
        private class FbMarketplaceCookie
        {
            public string sb { get; set; }

            public string fr { get; set; }

            public string wd { get; set; }

            public string datr { get; set; }

            public string c_user { get; set; }

            public string xs { get; set; }

            public string presence { get; set; }

            public override string ToString()
            {
                return $"sb={sb}; fr={fr}; wd={wd}; datr={datr}; c_user={c_user}; xs={xs}; presence={presence}";
            }
        }
    }
}
