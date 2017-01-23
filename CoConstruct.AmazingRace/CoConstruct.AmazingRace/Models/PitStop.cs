using System;
using System.Windows.Input;
using Xamarin.Forms.GoogleMaps;

namespace CoConstruct.AmazingRace.Models
{
    public class PitStop : BaseLocation, ILocationViewModel
    {
        public PitStop(string title, string description, string address, double latitude, double longitude, PinType pinType = 0)
        {
            Title = title;
            Latitude = latitude;
            Longitude = longitude;
            Description = description;
            Address = address;
            PinType = pinType;
            TimeStamp = DateTime.UtcNow;
        }

        public string Title { get; set; }
        public string Description { get; }
        public string Address { get; }
        public PinType PinType { get; set; }
        public ICommand Command { get; }
        public DateTime TimeStamp { get; set; }
    }
}