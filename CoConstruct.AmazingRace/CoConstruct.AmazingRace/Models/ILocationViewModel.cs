using System;
using System.Windows.Input;
using Xamarin.Forms.GoogleMaps;

namespace CoConstruct.AmazingRace.Models
{
    public interface ILocationViewModel
    {
        string Title { get; set; }
        string Description { get; }
        double Latitude { get; }
        double Longitude { get; }
        DateTime TimeStamp { get; set; }
        PinType PinType { get; }
        ICommand Command { get; }
    }
}