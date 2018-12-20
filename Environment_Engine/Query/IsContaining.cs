/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Environment;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsContaining(this List<List<BuildingElement>> containers, BuildingElement element)
        {
            foreach(List<BuildingElement> lst in containers)
                if (lst.Where(x => x.BHoM_Guid == element.BHoM_Guid).FirstOrDefault() != null) return true;

            return false;
        }

        public static bool IsContaining(this List<Point> pts, Point pt)
        {
            Point p = pts.Where(point => point.X == pt.X && point.Y == pt.Y && point.Z == pt.Z).FirstOrDefault();

            return (p != null);
        }

        public static bool IsContaining(this BuildingElement element, Point pt, bool acceptOnEdges = false)
        {
            if (pt == null) return false;
            return new List<BuildingElement> { element }.IsContaining(pt, acceptOnEdges);
        }

        public static bool IsContaining(this List<BuildingElement> space, Point point, bool acceptOnEdges = false)
        {
            List<Plane> planes = space.Select(x => x.PanelCurve.IControlPoints().FitPlane()).ToList();
            List<Point> ctrPoints = space.SelectMany(x => x.PanelCurve.IControlPoints()).ToList();
            BoundingBox boundingBox = BH.Engine.Geometry.Query.Bounds(ctrPoints);

            if (!BH.Engine.Geometry.Query.IsContaining(boundingBox, point)) return false;

            //We need to check one line that starts in the point and end outside the bounding box
            Vector vector = new Vector() { X = 1, Y = 0, Z = 0 };
            //Use a length longer than the longest side in the bounding box. Change to infinite line?
            Line line = new Line() { Start = point, End = point.Translate(vector * (((boundingBox.Max - boundingBox.Min).Length()) * 10)) };

            //Check intersections
            int counter = 0;
            List<Point> intersectPoints = new List<Point>();

            for (int x = 0; x < planes.Count; x++)
            {
                if ((BH.Engine.Geometry.Query.PlaneIntersection(line, planes[x], false)) == null) continue;

                List<Point> intersectingPoints = new List<oM.Geometry.Point>();
                intersectingPoints.Add(BH.Engine.Geometry.Query.PlaneIntersection(line, planes[x]));
                Polyline pLine = new Polyline() { ControlPoints = space[x].PanelCurve.IControlPoints() };

                if (intersectingPoints != null && BH.Engine.Geometry.Query.IsContaining(pLine, intersectingPoints, true, 1e-05))
                {
                    intersectPoints.AddRange(intersectingPoints);
                    if (intersectPoints.CullDuplicates().Count == intersectPoints.Count()) //Check if the point already has been added to the list
                        counter++;
                }
            }

            bool isContained = !((counter % 2) == 0);

            if(!isContained && acceptOnEdges)
            {
                //Check the edges in case the point is on the edge of the BE
                foreach(BuildingElement be in space)
                {
                    List<Line> subParts = be.PanelCurve.ISubParts() as List<Line>;
                    foreach(Line l in subParts)
                    {
                        if (l.IsOnCurve(point)) isContained = true;
                    }
                }
            }

            return isContained; //If the number of intersections is odd the point is outsde the space
        }

        public static bool IsContaining(this Space space, Point point)
        {
            /*List<BHE.BuildingElement> buildingElements = space.BuildingElements;
            List<BHG.Plane> planes = buildingElements.Select(x => x.BuildingElementGeometry.ICurve().IControlPoints().FitPlane()).ToList();
            List<BHG.Point> ctrPoints = buildingElements.SelectMany(x => x.BuildingElementGeometry.ICurve().IControlPoints()).ToList();
            BHG.BoundingBox boundingBox = BH.Engine.Geometry.Query.Bounds(ctrPoints);

            if (!BH.Engine.Geometry.Query.IsContaining(boundingBox, point)) return false;

            //We need to check one line that starts in the point and end outside the bounding box
            BHG.Vector vector = new BHG.Vector() { X = 1, Y = 0, Z = 0 };
            //Use a length longer than the longest side in the bounding box. Change to infinite line?
            BHG.Line line = new BHG.Line() { Start = point, End = point.Translate(vector * (((boundingBox.Max - boundingBox.Min).Length()) * 10)) };

            //Check intersections
            int counter = 0;
            List<BHG.Point> intersectPoints = new List<BHG.Point>();

            for(int x = 0; x < planes.Count; x++)
            {
                if ((BH.Engine.Geometry.Query.PlaneIntersection(line, planes[x], false)) == null) continue;

                List<BHG.Point> intersectingPoints = new List<oM.Geometry.Point>();
                intersectingPoints.Add(BH.Engine.Geometry.Query.PlaneIntersection(line, planes[x]));
                BHG.Polyline pLine = new BHG.Polyline() { ControlPoints = buildingElements[x].BuildingElementGeometry.ICurve().IControlPoints() };

                if(intersectingPoints != null && BH.Engine.Geometry.Query.IsContaining(pLine, intersectingPoints, true, 1e-05))
                {
                    intersectPoints.AddRange(intersectingPoints);
                    if (intersectPoints.CullDuplicates().Count == intersectPoints.Count()) //Check if the point already has been added to the list
                        counter++;
                }
            }

            return ((counter % 2) == 0); //If the number of intersections is odd the point is outsde the space*/

            return false;
        }
    }
}
