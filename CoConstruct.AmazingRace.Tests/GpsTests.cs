using System;
using CoConstruct.AmazingRace.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms.GoogleMaps;

namespace CoConstruct.AmazingRace.Tests
{
    [TestClass]
    public class GpsTests
    {
        [TestMethod]
        public void DistanceBetweenTwoPoints()
        {
            var point1 = new Position(38.897558, -77.036768);  // 1600 Pennsylvania Ave.
            var point2 = new Position(40.748251, -73.985726);  // Empire State Building
            double expectedDist = 206.518754;  // miles

            var dist = point1.DistanceFrom(point2);

            Assert.AreEqual(expectedDist, dist, 0.000001, "Distance calculation is not correct" );
        }
    }
}
