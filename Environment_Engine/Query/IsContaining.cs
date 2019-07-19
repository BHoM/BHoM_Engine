/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Base;

using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

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
                if (lst.Where(x => x.BHoM_Guid == panel.BHoM_Guid).FirstOrDefault() != null) return true;

            return false;
        }

        [Description("Defines whether an a BHoM Geometry Point is contained within a list of Points")]
        [Input("points", "A collection of BHoM Geometry Points")]
        [Input("point", "The point being checked to see if it is contained within the list of points")]
        [Output("isContaining", "True if the point is contained within the list, false if it is not")]
        public static bool IsContaining(this List<Point> pts, Point pt)
        {
            return (pts.Where(point => Math.Round(point.X, 3) == Math.Round(pt.X, 3) && Math.Round(point.Y, 3) == Math.Round(pt.Y, 3) && Math.Round(point.Z, 3) == Math.Round(pt.Z, 3)).FirstOrDefault() != null);
        }

        [Description("Defines whether an Environment Panel contains a provided point")]
        [Input("panel", "An Environment Panel to check with")]
        [Input("point", "The point being checked to see if it is contained within the bounds of the panel")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the panel, default false")]
        [Output("isContaining", "True if the point is contained within the panel, false if it is not")]
        public static bool IsContaining(this Panel panel, Point pt, bool acceptOnEdges = false)
        {
            if (pt == null) return false;
            return new List<Panel> { panel }.IsContaining(pt, acceptOnEdges);
        }

        [Description("Defines whether a collection of Environment Panels contains a provided point")]
        [Input("panels", "A collection of Environment Panels to check with")]
        [Input("point", "The point being checked to see if it is contained within the bounds of the panels")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the panel, default false")]
        [Output("isContaining", "True if the point is contained within at least one of the panels, false if it is not")]
        public static bool IsContaining(this List<Panel> panels, Point point, bool acceptOnEdges = false)
        {
            List<Plane> planes = new List<Plane>();
            foreach (Panel be in panels)
            {
                Plane p = be.Polyline().IControlPoints().FitPlane();
                if (!be.Polyline().IsInPlane(p))
                {
                    //Create a different plane...
                    List<Point> pnts = be.Polyline().IDiscontinuityPoints();
                    if (pnts.Count >= 3)
                        p = BH.Engine.Geometry.Create.Plane(pnts[0], pnts[1], pnts[2]);
                }

                planes.Add(p);
            }

            List<Point> ctrPoints = panels.SelectMany(x => x.Polyline().IControlPoints()).ToList();
            BoundingBox boundingBox = BH.Engine.Geometry.Query.Bounds(ctrPoints);

            if (!BH.Engine.Geometry.Query.IsContaining(boundingBox, point)) return false;

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
                    if ((BH.Engine.Geometry.Query.PlaneIntersection(line, planes[x], false)) == null) continue;

                    List<Point> intersectingPoints = new List<oM.Geometry.Point>();
                    intersectingPoints.Add(BH.Engine.Geometry.Query.PlaneIntersection(line, planes[x]));
                    Polyline pLine = panels[x].Polyline();

                    if (intersectingPoints != null && BH.Engine.Geometry.Query.IsContaining(pLine, intersectingPoints, true, 1e-05))
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
                        if (l.IsOnCurve(point)) isContained = true;
                    }
                }
            }

            return isContained; //If the number of intersections is odd the point is outsde the space
        }
    }
}
