using System;
using System.Collections.Generic;
using System.Linq;
using CoConstruct.AmazingRace.Models;
using Xamarin.Forms.GoogleMaps;

namespace CoConstruct.AmazingRace.Extensions
{
    public static class PositionExtensionMethods
    {
        public const int EquatorRadius = 6378137;

        /// <summary>
        /// Output a given position to formatted text
        /// </summary>
        /// <param name="p">Position coordinates.</param>
        /// <returns>The <see cref="System.string" />.</returns>
        public static string ToGpsString(this Position p)
        {
            return $"GPS: {p.Latitude:F6}, {p.Longitude:F6}";
        }

        public static double DistanceFrom(this BaseLocation a, BaseLocation b)
        {
            return DistanceFrom(new Position(a.Latitude, a.Longitude), new Position(b.Latitude, b.Longitude));
        }

        /// <summary>
        /// Calculates distance between two locations.
        /// </summary>
        /// <param name="a">Location a</param>
        /// <param name="b">Location b</param>
        /// <returns>The <see cref="System.Double" />The distance in meters</returns>
        public static double DistanceFrom(this Position a, Position b)
        {
            /*
			double distance = Math.Acos(
				(Math.Sin(a.Latitude) * Math.Sin(b.Latitude)) +
				(Math.Cos(a.Latitude) * Math.Cos(b.Latitude))
				* Math.Cos(b.Longitude - a.Longitude));
			 * */

            var dLat = b.Latitude.DegreesToRadians() - a.Latitude.DegreesToRadians();
            var dLon = b.Longitude.DegreesToRadians() - a.Longitude.DegreesToRadians();

            var a1 = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(a.Latitude.DegreesToRadians()) * Math.Cos(b.Latitude.DegreesToRadians()) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var distance = 2 * Math.Atan2(Math.Sqrt(a1), Math.Sqrt(1 - a1));

            var dist = EquatorRadius * distance;
            return dist * 0.000621371;  // converted to miles
        }

        /// <summary>
        /// Calculates bearing between start and stop.
        /// </summary>
        /// <param name="start">Start coordinates.</param>
        /// <param name="stop">Stop coordinates.</param>
        /// <returns>The <see cref="System.Double" />.</returns>
        public static double BearingFrom(this Position start, Position stop)
        {
            var deltaLon = stop.Longitude - start.Longitude;
            var cosStop = Math.Cos(stop.Latitude);
            return Math.Atan2(
                (Math.Cos(start.Latitude) * Math.Sin(stop.Latitude)) -
                (Math.Sin(start.Latitude) * cosStop * Math.Cos(deltaLon)),
                Math.Sin(deltaLon) * cosStop);
        }

        /// <summary>
        /// Radianses to degrees.
        /// </summary>
        /// <param name="rad">The RAD.</param>
        /// <returns>System.Double.</returns>
        public static double RadiansToDegrees(this double rad)
        {
            return 180.0 * rad / Math.PI;
        }

        /// <summary>
        /// Degreeses to radians.
        /// </summary>
        /// <param name="deg">The deg.</param>
        /// <returns>System.Double.</returns>
        public static double DegreesToRadians(this double deg)
        {
            return Math.PI * deg / 180.0;
        }

        public static List<Position> GetShortestRoute(List<Position> positions)
        {
            if (positions == null)
                throw new ArgumentNullException(nameof(positions), "Cannot be null");

            if (positions.Count < 2)
                throw new ArgumentException("Need at least 2 positions to determine a route", nameof(positions));

            var shortRoute = new List<Position>();

            // assume first item is start and last item is end
            var start = positions.FirstOrDefault();
            var stop = positions.LastOrDefault();
            shortRoute.Add(start);  // add the starting point of the route

            var positionsLeft = positions;
            positionsLeft.RemoveAt(0);  // remove the starting position from the remaining position
            positionsLeft.RemoveAt(positionsLeft.Count - 1); // remove the end position from the remaining positions

            var nextPoint = start;
            while (positionsLeft.Count > 0)
            {
                var closestPosition = GetClosestPosition(positionsLeft, nextPoint);
                shortRoute.Add(closestPosition);
                positionsLeft.Remove(closestPosition);  // may be altering the collection being itterated
                nextPoint = closestPosition;
            }

            shortRoute.Add(stop);

            return shortRoute;
        }


        public static Position GetClosestPosition(List<Position> availablePositions, Position pos)
        {
            if (availablePositions == null)
                throw new ArgumentNullException(nameof(availablePositions), "Cannot be null");

            if (pos == null)
                throw new ArgumentNullException(nameof(pos), "Cannot be null");

            if (availablePositions.Count == 1)
                return availablePositions.FirstOrDefault();

            Position closestPosition = pos;
            var dist = 0.0;
            foreach (var position in availablePositions)
            {
                var tmpDist = pos.DistanceFrom(position);
                if (tmpDist >= dist && dist > 0.0) continue;

                closestPosition = position;
                dist = tmpDist;
            }

            return closestPosition;
        }

    }
}