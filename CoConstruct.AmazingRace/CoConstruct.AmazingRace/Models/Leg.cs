using System.Collections.Generic;
using System.IO;

namespace CoConstruct.AmazingRace.Models
{
    public class Leg
    {
        public Leg(PitStop origin, PitStop destination)
        {
            From = origin;
            To = destination;
        }

        public PitStop From { get; set; }
        public PitStop To { get; set; }
        public double Distance { get; set; }
        public List<string> Steps { get; set; }

        public string StepsText => Steps == null ? "No steps" : string.Join("<br/>", Steps.ToArray());
    }
}