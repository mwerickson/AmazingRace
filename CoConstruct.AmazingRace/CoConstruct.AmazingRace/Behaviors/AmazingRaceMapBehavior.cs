using System;
using System.Collections.Generic;
using System.Linq;
using CoConstruct.AmazingRace.Extensions;
using CoConstruct.AmazingRace.Models;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace CoConstruct.AmazingRace.Behaviors
{
    public class AmazingRaceMapBehavior : BindableBehavior<Map>
    {
        // was deprecated
        // public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create<MapBehavior, IEnumerable<ILocationViewModel>>(
        //p => p.ItemsSource, null, BindingMode.Default, null, ItemsSourceChanged);
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource",
            typeof(IEnumerable<ILocationViewModel>), typeof(AmazingRaceMapBehavior), null, BindingMode.Default, null,
            ItemsSourceChanged);

        public static readonly BindableProperty MyPositionProperty = BindableProperty.Create("MyPosition",
            typeof(GpsPosition), typeof(AmazingRaceMapBehavior), propertyChanged: UserPositionChanged);

        public GpsPosition MyPosition
        {
            get { return (GpsPosition)GetValue(MyPositionProperty); }
            set { SetValue(MyPositionProperty, value); }
        }

        public IEnumerable<ILocationViewModel> ItemsSource
        {
            get { return (IEnumerable<ILocationViewModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void UserPositionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = bindable as AmazingRaceMapBehavior;
            behavior?.DrawNavLine();
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = bindable as AmazingRaceMapBehavior;
            behavior?.AddPins();
            //behavior?.DrawPathLine();
            //behavior?.DrawNavLine();
        }

        private void DrawNavLine()
        {
            var map = AssociatedObject;

            // get the last known position of the path
            var lkp = ItemsSource?.LastOrDefault();

            if (MyPosition == null || lkp == null) return;

            // get existing nav line if it exists
            var navLine = map.Polylines.FirstOrDefault(l => (string)l.Tag == "nav");
            if (navLine != null)
            {
                map.Polylines.Remove(navLine);
            }

            navLine = new Polyline()
            {
                Tag = "nav",
                Positions = { new Position(MyPosition.Latitude, MyPosition.Longitude), new Position(lkp.Latitude, lkp.Longitude) },
                StrokeColor = Color.Red,
                StrokeWidth = 5f
            };

            // add the line to the map
            map.Polylines.Add(navLine);
        }

        private void DrawPathLine()
        {
            var map = AssociatedObject;

            if (map == null || ItemsSource == null) return;

            // find any "path" lines
            var pathLines = map.Polylines.Where(l => (string)l.Tag == "path").ToList();
            if (pathLines != null && pathLines.Count > 0)
            {
                // remove these lines
                foreach (var line in pathLines)
                {
                    map.Polylines.Remove(line);
                }
            }

            // refresh the lines
            var positions = ItemsSource
                .OrderBy(s => s.TimeStamp)
                .Select(x =>
                {
                    var pos = new Position(x.Latitude, x.Longitude);
                    return pos;
                })
                .ToList();

            if (positions == null || positions.Count <= 1) return;   // must have at least 2 points to make a line.

            var pathLine = new Polyline()
            {
                Tag = "path",
                StrokeColor = Color.Yellow,
                StrokeWidth = 2f
            };

            foreach (var pos in positions)
            {
                pathLine.Positions.Add(pos);
            }

            map.Polylines.Add(pathLine);
        }

        private void AddPins()
        {
            var map = AssociatedObject;
            map.PinClicked -= PinOnClicked;
            for (int i = map.Pins.Count - 1; i >= 0; i--)
            {
                map.Pins.RemoveAt(i);
            }

            if (ItemsSource == null) return;

            var tmpSource = ItemsSource.ToList();
            tmpSource = tmpSource.OrderBy(s => s.TimeStamp).ToList();

            // get first position
            var firstPos = tmpSource.FirstOrDefault();
            if (firstPos == null) return;  // obviously we don't have any positions yet

            var firstPin = new Pin
            {
                Type = PinType.Place,
                Label = "Start Position",
                Address = firstPos.Description,
                Position = new Position(firstPos.Latitude, firstPos.Longitude),
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.Lime)
            };
            map.Pins.Add(firstPin);

            if (tmpSource.Count > 1)
            {
                // get last position
                var lastPos = tmpSource.LastOrDefault();
                var lastPin = new Pin
                {
                    Type = PinType.Place,
                    Label = "Last Pit Stop",
                    Address = lastPos.Description,
                    Position = new Position(lastPos.Latitude, lastPos.Longitude),
                    Icon = BitmapDescriptorFactory.DefaultMarker(Color.Red)
                };
                map.Pins.Add(lastPin);

                // TODO: add the rest of the pins
                // remove first and last
                tmpSource.RemoveAt(0);
                tmpSource.RemoveAt(tmpSource.Count - 1);

                foreach (var pos in tmpSource)
                {
                    var pin = new Pin()
                    {
                        Type = PinType.Place,
                        Label = "Pit Stop",
                        Address = pos.Description,
                        Position = new Position(pos.Latitude, pos.Longitude),
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.Orange)
                    };
                    map.Pins.Add(pin);
                }
            }

            // add pin click handler
            map.PinClicked += PinOnClicked;

            PositionMap();
        }

        private void PinOnClicked(object sender, EventArgs eventArgs)
        {
            var pin = sender as Pin;
            if (pin == null) return;
            var viewModel = ItemsSource.FirstOrDefault(x => x.Title == pin.Label);
            viewModel?.Command?.Execute(null);
        }

        private void PositionMap()
        {
            try
            {
                var map = AssociatedObject;

                if (ItemsSource == null || !ItemsSource.Any()) return;

                var lastPosition = ItemsSource.LastOrDefault();
                if (lastPosition == null) return;

                var centerPosition = new Position(ItemsSource.Average(x => x.Latitude),
                    ItemsSource.Average(x => x.Longitude));

                var minLongitude = ItemsSource.Min(x => x.Longitude);
                var minLatitude = ItemsSource.Min(x => x.Latitude);
                var minPosition = new Position(minLatitude, minLongitude);

                var maxLongitude = ItemsSource.Max(x => x.Longitude);
                var maxLatitude = ItemsSource.Max(x => x.Latitude);
                var maxPosition = new Position(maxLatitude, maxLongitude);

                var distance = minPosition.DistanceFrom(maxPosition) / 2;

                //var centerPosition = new Position(lastPosition.Latitude, lastPosition.Longitude);
                //var positions = new List<Position> {centerPosition};

                Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
                {
                    //map?.MoveToRegion(MapSpan.FromPositions(positions));
                    map?.MoveToRegion(MapSpan.FromCenterAndRadius(centerPosition,
                        Distance.FromMiles(distance)), true);
                    return false;
                });
            }
            catch (Exception e)
            {
                var x = e.Message;
            }
        }
    }
}