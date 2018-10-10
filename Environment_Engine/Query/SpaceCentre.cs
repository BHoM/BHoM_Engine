using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;

using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> SpaceCentres(this List<List<BuildingElement>> spaces)
        {
            List<Point> centrePts = new List<Point>();
            foreach (List<BuildingElement> space in spaces)
                centrePts.Add(space.SpaceCentre());

            return centrePts;
        }

        public static Point SpaceCentre(this List<BuildingElement> space)
        {
            //Calculate the centre point of the space comprised of the building elements
            //Done using the centre of mass of the vertices

            List<Point> vertexPoints = new List<Point>();
            foreach (BuildingElement be in space)
                vertexPoints.AddRange(be.PanelCurve.IControlPoints());

            double centreX = 0;
            double centreY = 0;
            double centreZ = 0;

            foreach(Point p in vertexPoints)
            {
                centreX += p.X;
                centreY += p.Y;
                centreZ += p.Z;
            }

            centreX /= vertexPoints.Count;
            centreY /= vertexPoints.Count;
            centreZ /= vertexPoints.Count;

            return BH.Engine.Geometry.Create.Point(centreX, centreY, centreZ);
        }
    }
}

