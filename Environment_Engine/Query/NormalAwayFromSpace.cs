/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Base;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns whether the normal of a given Environment Panel is facing away from the containing space")]
        [Input("panel", "An Environment Panel to check")]
        [Input("panelsAsSpace", "A collection of Environment Panels which represent a single space")]
        [Input("tolerance", "Distance tolerance for planar checks, default set to BH.oM.Geometry.Tolerance.Distance")]
        [Output("normalAwayFromSpace", "True if the normal of the panel is facing away from the space, false otherwise")]
        public static bool NormalAwayFromSpace(this Panel panel, List<Panel> panelsAsSpace, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query whether the normal of a panel is pointing away from its containing space if the panel itself is null.");
                return false;
            }

            return NormalAwayFromSpace(panel.Polyline(), panelsAsSpace, tolerance);
        }

        [Description("Returns whether the normal of a given polyline is facing away from the containing space")]
        [Input("polyline", "A BHoM Geometry Polyline to check")]
        [Input("panelsAsSpace", "A collection of Environment Panels which represent a single space")]
        [Input("tolerance", "Distance tolerance for planar checks, default set to BH.oM.Geometry.Tolerance.Distance")]
        [Output("normalAwayFromSpace", "True if the normal of the polyline is facing away from the space, false otherwise")]
        public static bool NormalAwayFromSpace(this Polyline polyline, List<Panel> panelsAsSpace, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            List<Point> centrePtList = new List<Point>();
            Point centrePt = polyline.PointInRegion(false, tolerance); //Modifed to Centroid to fix special cases Point centrePt = polyline.Centre();
            
            if (centrePt == null)
                centrePt = polyline.Centroid();
            if (centrePt == null)
                centrePt = polyline.ControlPoints.CullDuplicates().Average();
            if (centrePt == null)
                return false; //Problems

            centrePtList.Add(centrePt);

            if (!polyline.IsClosed())
                return false; //Prevent failures of the clockwise check

            List<Point> pts = polyline.DiscontinuityPoints(tolerance);
            if (pts.Count < 3) return false; //Protection in case there aren't enough points to make a plane
            Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);

            //The polyline can be locally concave. Check if the polyline is clockwise.
            if (!BH.Engine.Geometry.Query.IsClockwise(polyline, plane.Normal, tolerance))
                plane.Normal = -plane.Normal;

            if (!BH.Engine.Geometry.Query.IsContaining(polyline, centrePtList, false, tolerance))
            {
                Point pointOnLine = polyline.ClosestPoint(centrePt);
                Vector vector = new Vector();
                if (BH.Engine.Geometry.Query.Distance(pointOnLine, centrePt) > BH.oM.Geometry.Tolerance.MicroDistance)
                    vector = pointOnLine - centrePt;
                else
                {
                    Line line = BH.Engine.Geometry.Query.GetLineSegment(polyline, pointOnLine);
                    vector = ((line.Start - line.End).Normalise()).CrossProduct(plane.Normal);
                }

                centrePt = BH.Engine.Geometry.Modify.Translate(pointOnLine, BH.Engine.Geometry.Modify.Normalise(vector) * 0.001);
            }

            //Move centrepoint along the normal.
            if(panelsAsSpace.IsContaining(centrePt.Translate(plane.Normal * 0.01)))
                return false;
            else
                return true;
        }
    }
}





