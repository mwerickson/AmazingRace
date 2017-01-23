using System;

namespace CoConstruct.AmazingRace.Models
{
    public class GpsPosition
    {
        public GpsPosition()
        {

        }

        public GpsPosition(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}