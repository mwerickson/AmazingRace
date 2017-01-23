using System;
using System.Collections.Generic;
using CoConstruct.AmazingRace.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms.GoogleMaps;

namespace CoConstruct.AmazingRace.Tests
{
    [TestClass]
    public class RouteTests
    {
        [TestMethod]
        public void ShortestRouteThroughAllPoints()
        {
            // build point list
            var points = new List<Position>();
            points.Add(new Position(38.897558, -77.036768));  // white house
            points.Add(new Position(40.748251, -73.985726));  // empire state building
            points.Add(new Position(42.338945, -71.094068));  // Boston mueseum of fine arts
            points.Add(new Position(39.965550, -75.181245));  // Rocky steps!
            points.Add(new Position(38.628634, -90.183903));  // gateway arch
            points.Add(new Position(25.778072, -80.219827));  // Fl. Marlins park
            points.Add(new Position(37.794967, -122.402707)); // trans-america pyramid
            points.Add(new Position(38.086221, -78.470491));  // seminole trail VA
            points.Add(new Position(41.878854, -87.635896));  // wacker drive chicago
            points.Add(new Position(33.763143, -84.395615));  // GA Aquarium

            var actualRoute = PositionExtensionMethods.GetShortestRoute(points);

            Assert.IsNotNull(actualRoute);
        }
    }
}
