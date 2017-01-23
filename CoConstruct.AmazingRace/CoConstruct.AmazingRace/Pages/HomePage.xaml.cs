using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoConstruct.AmazingRace.Behaviors;
using CoConstruct.AmazingRace.Models;
using CoConstruct.AmazingRace.PageModels;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace CoConstruct.AmazingRace.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            var mapBehavior = new AmazingRaceMapBehavior();
            mapBehavior.SetBinding<HomePageModel>(AmazingRaceMapBehavior.ItemsSourceProperty, vm => vm.PitStops);
            mapBehavior.SetBinding<HomePageModel>(AmazingRaceMapBehavior.MyPositionProperty, vm => vm.MyCurrentPosition);
            this.RaceMap.Behaviors.Add(mapBehavior);
        }

        private void PitStopSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var pitStop = e.SelectedItem as PitStop;
            RaceMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(pitStop.Latitude, pitStop.Longitude), Distance.FromMiles(1)), true);
        }
    }
}
