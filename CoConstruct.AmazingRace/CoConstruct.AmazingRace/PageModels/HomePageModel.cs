using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CoConstruct.AmazingRace.Extensions;
using CoConstruct.AmazingRace.Models;
using CoConstruct.AmazingRace.Services;
using CoConstruct.AmazingRace.Utilities;
using FreshMvvm;
using Plugin.Geolocator;
using PropertyChanged;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace CoConstruct.AmazingRace.PageModels
{
    [ImplementPropertyChanged]
    public class HomePageModel : FreshBasePageModel
    {
        private readonly IAddressService _addressService;
        public HomePageModel(IAddressService addressService)
        {
            _addressService = addressService;

            PitStops = new ObservableCollection<PitStop>();
            //PitStops.CollectionChanged += (sender, args) =>
            //{
            //    RaisePropertyChanged(nameof(PitStops));
            //};
        }

        public string RawAddress { get; set; } 
        public string DisplayAddress { get; set; } 
        public string ErrorMessage { get; set; }
        public string CurrentAddress { get; set; }

        public ObservableCollection<PitStop> PitStops { get; set; }
        
        public Position MyCurrentPosition { get; set; }
        public bool ShowMap { get; set; }

        private PitStop _selectedPitStop;
        public PitStop SelectedPitStop
        {
            get { return _selectedPitStop; }
            set
            {
                _selectedPitStop = value;
                if (_selectedPitStop == null) return;
                PitStopSelectedCommand.Execute(_selectedPitStop);
                _selectedPitStop = null;
            }
        }

        public Command<PitStop> PitStopSelectedCommand
        {
            get
            {
                return new Command<PitStop>((pitStop) =>
                {
                    
                });
            }
        }

        public Command<string> AddPitStopCommand
        {
            get
            {
                return new Command<string>(async (addr) =>
                {
                    if (string.IsNullOrEmpty(addr)) return;

                    try
                    {
                        var pos = await _addressService.GetPosition(addr);
                        var address = await _addressService.GetAddress(pos);

                        var model = new PitStop("Pit Stop", $"{address}\n{pos.ToGpsString()}", address, pos.Latitude, pos.Longitude);

                        // property change is not correctly received by the mapping behavior for the positions observable collection
                        var tmp = PitStops.ToList();
                        tmp.Add(model);
                        var shortestRoute = GpsUtilities.GetShortestRoute<PitStop>(tmp);
                        PitStops = new ObservableCollection<PitStop>(shortestRoute);

                        RawAddress = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"There was an error retrieving the address/position. {ex.Message}";
                    }
                });
            }
        }

        public Command GoCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var positions = new List<PitStop>();
                    ShowMap = false;
                    if (string.IsNullOrEmpty(RawAddress)) return;
                    try
                    {
                        var pos = await _addressService.GetPosition(RawAddress);
                        DisplayAddress = await _addressService.GetAddress(pos);

                        positions.Add(new PitStop("User Address", $"{DisplayAddress}\n{pos.ToGpsString()}", "", pos.Latitude, pos.Longitude));
                        PitStops = new ObservableCollection<PitStop>(positions);
                        ShowMap = true;
                    }
                    catch (Exception)
                    {
                        ErrorMessage = "There was an error retrieving the address/position.";
                    }
                });
            }
        }

        public Command DeleteAddressCommand
        {
            get
            {
                return new Command<PitStop>((address) =>
                {
                    var tmp = PitStops.ToList();
                    tmp.Remove(address);
                    PitStops = new ObservableCollection<PitStop>(tmp);
                });
            }
        }

        public Command ShowDirectionsCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var pitStops = PitStops.ToList();
                    await CoreMethods.PushPageModel<LegsPageModel>(pitStops);
                });
            }
        }

        public void AddTestData()
        {
            var testAddresses = new List<string>
            {
                "1600 Pennsylvania Avenue NW Washington, D.C. 20500",
                "350 Fifth Avenue Manhattan, New York 10118",
                "465 Huntington Avenue Boston, MA 02115",
                "2600 Benjamin Franklin Parkway, Philadelphia",
                "100 Washington Avenue, St. Louis, 63102",
                "1501 NW 3rd Street Miami, Florida",
                "225 Baker St Atlanta, GA 30313",
                "600 Montgomery Street San Francisco, California",
                "1807 Seminole Trail, Suite 200 Charlottesville, VA 22901",
                "233 S. Wacker Drive Chicago, Illinois 60606"
            };

            foreach (var testAddress in testAddresses)
            {
                AddPitStopCommand.Execute(testAddress);
            }
            var tmp = PitStops.ToList();
            PitStops = new ObservableCollection<PitStop>(tmp);
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(CurrentAddress) || PitStops.Count == 0)
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 50;

                    var pos = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                    var gmPos = new Position(pos.Latitude, pos.Longitude);
                    CurrentAddress = await _addressService.GetAddress(gmPos);
                    PitStops.Add(new PitStop("Race Start", $"{CurrentAddress}\n{gmPos.ToGpsString()}", CurrentAddress, gmPos.Latitude,
                        gmPos.Longitude));
                    var tmp = PitStops.ToList();
                    PitStops = new ObservableCollection<PitStop>(tmp);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unable to get location, may need to increase timeout: {ex.Message}";
            }

            if (PitStops.Count <= 1)
                AddTestData();

            base.ViewIsAppearing(sender, e);
        }
    }
}