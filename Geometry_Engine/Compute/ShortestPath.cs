using System;
using System.Collections.Generic;
using System.Text;
using BH.oM.Geometry;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        [Description("A simplified method of sorting a list of points based on how close a given point is to the previous point. Based on the TSP but simpler and non-globally-optimal in the general case.")]
        [Input("points", "The list of points which to sort")]
        [Output("sortedPoints", "The sorted list of points")]

        public static List<Point> ShortestPath(List<Point> points)
        {
            List<Point> sortedPoints = new List<Point>();

            List<Point> ringPoints = points;

            sortedPoints.Add(ringPoints[0]);
            ringPoints.RemoveAt(0);

            while (ringPoints.Count > 0)
            {
                Point np = Query.ClosestPoint(sortedPoints.Last(), ringPoints);
                sortedPoints.Add(np);
                ringPoints.Remove(np);
               
                if (np == null) break;
            }

            return sortedPoints;

        }
    }
}
