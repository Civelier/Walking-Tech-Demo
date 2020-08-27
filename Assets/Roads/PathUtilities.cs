
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Roads
{
    public static class PathUtilities
    {
        public static IEnumerable<Vector3> GetPointsAfterTraveledDistance(RoadTravel travel)
        {
            yield return travel.CurrentPoint;
            var path = travel.Road.Path.path;
            for (int i = 0; i < path.NumPoints; i++)
            {
                if (path.cumulativeLengthAtEachVertex[i] >= travel.Distance)
                {
                    yield return path.GetPoint(i);
                }
            }
        }

        public static IEnumerable<Vector3> GetPointsAfterTraveledDistance(RoadTravel travel, float maxDistance)
        {
            yield return travel.CurrentPoint;
            var path = travel.Road.Path.path;
            for (int i = 0; i < path.NumPoints; i++)
            {
                if (path.cumulativeLengthAtEachVertex[i] > maxDistance) break;
                if (path.cumulativeLengthAtEachVertex[i] >= travel.Distance)
                {
                    yield return path.GetPoint(i);
                }
            }
        }

        public static bool DoVectorsCross(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        {
            float x1 = a1.x, y1 = a1.y, x2 = a2.x, y2 = a2.y, x3 = b1.x, y3 = b1.y, x4 = b2.x, y4 = b2.y;
            try
            {
                var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
                intersection = new Vector2(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
            }
            catch (System.DivideByZeroException)
            {
                intersection = new Vector2();
                return false;
            }
            //var u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
            
            return true;
        }

        public static bool DoVectorsCross(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float x1 = a1.x, y1 = a1.y, x2 = a2.x, y2 = a2.y, x3 = b1.x, y3 = b1.y, x4 = b2.x, y4 = b2.y;
            return ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)) != 0;
        }

        public static bool DoRoadsCrossXZ(RoadTravel t1, RoadTravel t2)
        {
            Vector2? t1Point1 = null, t2Point1 = null;
            foreach (var t1Point2 in GetPointsAfterTraveledDistance(t1))
            {
                t2Point1 = null;
                if (t1Point1 != null)
                {
                    foreach (var t2Point2 in GetPointsAfterTraveledDistance(t2))
                    {
                        if (t2Point1 != null)
                        {
                            if (DoVectorsCross(t1Point1.Value, new Vector2(t1Point2.x, t1Point2.z), t2Point1.Value, new Vector2(t2Point2.x, t2Point2.z))) return true;
                        }
                        t2Point1 = new Vector2(t2Point2.x, t2Point2.z);
                    }
                }
                t1Point1 = new Vector2(t1Point2.x, t1Point2.z);
            }
            return false;
        }

        public static bool DoRoadsCrossXZ(RoadTravel t1, RoadTravel t2, float maxDistance)
        {
            Vector2? t1Point1 = null, t2Point1 = null;
            foreach (var t1Point2 in GetPointsAfterTraveledDistance(t1, maxDistance))
            {
                t2Point1 = null;
                if (t1Point1 != null)
                {
                    foreach (var t2Point2 in GetPointsAfterTraveledDistance(t2, maxDistance))
                    {
                        if (t2Point1 != null)
                        {
                            if (DoVectorsCross(t1Point1.Value, new Vector2(t1Point2.x, t1Point2.z), t2Point1.Value, new Vector2(t2Point2.x, t2Point2.z))) return true;
                        }
                        t2Point1 = new Vector2(t2Point2.x, t2Point2.z);
                    }
                }
                t1Point1 = new Vector2(t1Point2.x, t1Point2.z);
            }
            return false;
        }
    }
}
