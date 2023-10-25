/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Defines whether an Environment Panel is contained by at least one group of panels representing spaces")]
        [Input("panelsAsSpaces", "A nested collection of Environment Panels representing spaces")]
        [Input("panel", "The Environment Panel to be checked to see if it is contained by the panelsAsSpaces")]
        [Output("isContaining", "True if the panel is contained by at least one group of panels, false if it is not")]
        public static bool IsContaining(this List<List<Panel>> panelsAsSpaces, Panel panel)
        {
            foreach (List<Panel> lst in panelsAsSpaces)
            {
                if (lst.Where(x => x.BHoM_Guid == panel.BHoM_Guid).FirstOrDefault() != null)
                    return true;
            }

            return false;
        }

        [Description("Defines whether an a BHoM Geometry Point is contained within a list of Points")]
        [Input("pts", "A collection of BHoM Geometry Points")]
        [Input("pt", "The point being checked to see if it is contained within the list of points")]
        [Output("isContaining", "True if the point is contained within the list, false if it is not")]
        public static bool IsContaining(this List<Point> pts, Point pt)
        {
            return (pts.Where(point => Math.Round(point.X, 3) == Math.Round(pt.X, 3) && Math.Round(point.Y, 3) == Math.Round(pt.Y, 3) && Math.Round(point.Z, 3) == Math.Round(pt.Z, 3)).FirstOrDefault() != null);
        }

        [Description("Defines whether an Environment Panel contains a provided point")]
        [Input("panel", "An Environment Panel to check with")]
        [Input("pt", "The point being checked to see if it is contained within the bounds of the panel")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the panel, default false")]
        [Output("isContaining", "True if the point is contained within the panel, false if it is not")]
        public static bool IsContaining(this Panel panel, Point pt, bool acceptOnEdges = false, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (panel == null || pt == null)
                return false;

            return new List<Panel> { panel }.IsContaining(pt, acceptOnEdges, tolerance);
        }

        [Description("Defines whether a collection of Environment Panels contains a provided point")]
        [Input("panels", "A collection of Environment Panels to check with")]
        [Input("point", "The point being checked to see if it is contained within the bounds of the panels")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the panel, default false")]
        [Output("isContaining", "True if the point is contained within the bounds of the panels, false if it is not")]
        public static bool IsContaining(this List<Panel> panels, Point point, bool acceptOnEdges = false, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (panels == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if a collection of panels contains a point if the panels are null.");
                return false;
            }

            List<Plane> planes = new List<Plane>();
            foreach (Panel be in panels)
                planes.Add(be.Polyline().IControlPoints().FitPlane(tolerance));

            List<Point> ctrPoints = panels.SelectMany(x => x.Polyline().IControlPoints()).ToList();
            BoundingBox boundingBox = BH.Engine.Geometry.Query.Bounds(ctrPoints);

            return IsContaining(panels, planes, boundingBox, point, acceptOnEdges);
        }

        [Description("Defines whether a collection of Environment Panels contains each of a provided list of points.")]
        [Input("panels", "A collection of Environment Panels to check with.")]
        [Input("points", "The points to check to see if each point is contained within the bounds of the panels.")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the panel, default false.")]
        [Output("isContaining", "True if the point is contained within the bounds of the panels, false if it is not for each point provided.")]
        public static List<bool> IsContaining(this List<Panel> panels, List<Point> points, bool acceptOnEdges = false, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (panels == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if a collection of panels contains a point if the panels are null.");
                return new List<bool>() { false };
            }

            List<Plane> planes = new List<Plane>();
            foreach (Panel be in panels)
                planes.Add(be.Polyline().IControlPoints().FitPlane(tolerance));

            List<Point> ctrPoints = panels.SelectMany(x => x.Polyline().IControlPoints()).ToList();
            BoundingBox boundingBox = BH.Engine.Geometry.Query.Bounds(ctrPoints);

            return points.Select(point => IsContaining(panels, planes, boundingBox, point, acceptOnEdges)).ToList();
        }

        [Description("Defines whether an Environment Space contains each of a provided list of points.")]
        [Input("space", "An Environment Space object defining a perimeter to build a 3D volume from and check if the volume contains the provided point.")]
        [Input("spaceHeight", "The height of the space.", typeof(BH.oM.Quantities.Attributes.Length))]
        [Input("points", "The points being checked to see if it is contained within the bounds of the 3D volume.")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the space, default false.")]
        [Output("isContaining", "True if the point is contained within the space, false if it is not.")]
        public static List<bool> IsContaining(this Space space, double spaceHeight, List<Point> points, bool acceptOnEdges = false)
        {
            List<Panel> panelsFromSpace = space.ExtrudeToVolume(spaceHeight);
            return panelsFromSpace.IsContaining(points, acceptOnEdges);
        }

        [Description("Defines whether an Environment Space contains a provided Element.")]
        [Input("space", "An Environment Space object defining a perimeter to build a 3D volume from and check if the volume contains the provided element.")]
        [Input("spaceHeight", "The height of the space.", typeof(BH.oM.Quantities.Attributes.Length))]
        [Input("elements", "The elements being checked to see if they are contained within the bounds of the 3D volume.")]
        [Input("acceptOnEdges", "Decide whether to allow the element's point to sit on the edge of the space, default false.")]
        [Output("isContaining", "True if the point is contained within the space, false if it is not.")]
        public static List<bool> IsContaining(this Space space, double spaceHeight, List<IElement> elements, bool acceptOnEdges = false, bool acceptPartialContainment = false)
        {
            List<Panel> panelsFromSpace = space.ExtrudeToVolume(spaceHeight);
            List<List<Point>> pointLists = new List<List<Point>>();

            foreach (IElement elem in elements)
            {
                List<Point> points = elem.IControlPoints();
                pointLists.Add(points);
            }
            return panelsFromSpace.IsContaining(pointLists, acceptOnEdges, acceptPartialContainment);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Defines whether a point lies within a collection of panels using their primitive planes and bounds.")]
        [Input("panels", "A collection of Environment Panels to check with.")]
        [Input("planes", "Planes corresponding to each panel for intersection calculations.")]
        [Input("boundingBox", "The bounding box of the collection of panels.")]
        [Input("point", "The point to check to see if it is contained within the bounds of the panels.")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the panel, default false.")]
        [Input("tolerance", "Distance tolerance to use to determine intersections.")]
        [Output("isContaining", "True if the point is contained within the bounds of the panels, false if it is not for each point provided.")]
        private static bool IsContaining(this List<Panel> panels, List<Plane> planes, BoundingBox boundingBox, Point point, bool acceptOnEdges = false, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            //Return if point is null even without checking boundingBox.IsContaining(point)
            if (point == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if a collection of panels contains a point if the point is null.");
                return false;
            }

            if (!BH.Engine.Geometry.Query.IsContaining(boundingBox, point, true, tolerance))
                return false;

            //We need to check one line that starts in the point and end outside the bounding box
            Vector vector = new Vector() { X = 1, Y = 0, Z = 0 };
            //Use a length longer than the longest side in the bounding box. Change to infinite line?
            Line line = new Line() { Start = point, End = point.Translate(vector * (((boundingBox.Max - boundingBox.Min).Length()) * 10)) };

            bool isInPlane = false;
            do
            {
                isInPlane = false;
                foreach (Plane p in planes)
                    isInPlane = isInPlane || line.IsInPlane(p);

                if (isInPlane)
                {
                    Vector v = new Vector() { X = 0.5, Y = 0.5, Z = 0.5 };
                    line = new Line() { Start = point, End = point.Translate(v * (((boundingBox.Max - boundingBox.Min).Length()) * 10)) };
                }
            }
            while (isInPlane);

            //Check intersections
            List<Point> intersectPoints = new List<Point>();

            for (int x = 0; x < planes.Count; x++)
            {
                if (planes[x] != null)
                {
                    if ((BH.Engine.Geometry.Query.PlaneIntersection(line, planes[x], false, tolerance)) == null)
                        continue;

                    List<Point> intersectingPoints = new List<oM.Geometry.Point>();
                    intersectingPoints.Add(BH.Engine.Geometry.Query.PlaneIntersection(line, planes[x], true, tolerance));
                    Polyline pLine = panels[x].Polyline();

                    if (intersectingPoints != null && BH.Engine.Geometry.Query.IsContaining(pLine, intersectingPoints, true, tolerance))
                        intersectPoints.AddRange(intersectingPoints);
                }
            }

            bool isContained = !((intersectPoints.CullDuplicates().Count % 2) == 0);

            if (!isContained && acceptOnEdges)
            {
                //Check the edges in case the point is on the edge of the BE
                foreach (Panel p in panels)
                {
                    List<Line> subParts = p.Polyline().ISubParts() as List<Line>;
                    foreach (Line l in subParts)
                    {
                        if (l.IsOnCurve(point))
                            isContained = true;
                    }
                }
            }

            return isContained; //If the number of intersections is odd the point is outsde the space
        }

        [Description("Defines whether a collection of Environment Panels contains each of a provided list of list of points.")]
        [Input("panels", "A collection of Environment Panels to check with.")]
        [Input("pointLists", "The List of Lists of points to check to see if each List of points are contained within the bounds of the panels.")]
        [Input("acceptOnEdges", "Decide whether to allow the points to sit on the edge of the panel, default false.")]
        [Input("acceptPartialContainment", "Decide whether to allow some of the points to sit outside the panels as long as at least one is within them.")]
        [Output("isContaining", "True if the points of each sublist are contained within the bounds of the panels, false if it is not for each sublist of points provided.")]
        private static List<bool> IsContaining(this List<Panel> panels, List<List<Point>> pointLists, bool acceptOnEdges = false, bool acceptPartialContainment = false, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (panels == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if a collection of panels contains a point if the panels are null.");
                return new List<bool>() { false };
            }

            List<Plane> planes = new List<Plane>();
            foreach (Panel be in panels)
                planes.Add(be.Polyline().IControlPoints().FitPlane(tolerance));

            List<Point> ctrPoints = panels.SelectMany(x => x.Polyline().IControlPoints()).ToList();
            BoundingBox boundingBox = BH.Engine.Geometry.Query.Bounds(ctrPoints);

            List<bool> areContained = new List<bool>();

            foreach (List<Point> pts in pointLists)
            {
                bool isContained = false;
                if (acceptPartialContainment)
                    isContained = pts.Any(point => IsContaining(panels, planes, boundingBox, point, acceptOnEdges));
                else
                    isContained = pts.All(point => IsContaining(panels, planes, boundingBox, point, acceptOnEdges));
                areContained.Add(isContained);
            }

            return areContained;
        }
    }
}