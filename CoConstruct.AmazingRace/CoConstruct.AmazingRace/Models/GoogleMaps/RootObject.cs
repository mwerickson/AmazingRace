using System.Collections.Generic;

namespace CoConstruct.AmazingRace.Models.GoogleMaps
{
    public class RootObject
    {
        public List<Route> routes { get; set; }
        public string status { get; set; }
    }
}