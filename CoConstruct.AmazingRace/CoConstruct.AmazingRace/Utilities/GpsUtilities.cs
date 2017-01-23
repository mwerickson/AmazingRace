using System;
using System.Collections.Generic;
using System.Linq;
using CoConstruct.AmazingRace.Extensions;
using CoConstruct.AmazingRace.Models;

namespace CoConstruct.AmazingRace.Utilities
{
    public class GpsUtilities
    {
        public static List<T> GetShortestRoute<T>(List<T> positions) where T: BaseLocation
        {
            if (positions == null)
                return null;

            if (positions.Count < 2)
                return positions;

            var shortRoute = new List<T>();

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


        public static T GetClosestPosition<T>(List<T> availablePositions, T pos) where T: BaseLocation
        {
            if (availablePositions == null)
                throw new ArgumentNullException(nameof(availablePositions), "Cannot be null");

            if (pos == null)
                throw new ArgumentNullException(nameof(pos), "Cannot be null");

            if (availablePositions.Count == 1)
                return availablePositions.FirstOrDefault();

            T closestPosition = pos;
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