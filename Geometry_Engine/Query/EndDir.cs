/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        [Description("Gets the tangent vector at the end of the Arc.")]
        [Input("arc", "The Arc from which to get the tangent at the end point.")]
        [Output("endTan", "The tangent vector at the end of the Arc.")]
        public static Vector EndDir(this Arc arc)
        {
            return arc.CoordinateSystem.Y.Rotate(arc.EndAngle, arc.CoordinateSystem.Z);
        }

        /***************************************************/

        [Description("Gets the tangent vector at the end of the Circle.")]
        [Input("circle", "The Circle from which to get the tangent at the end point.")]
        [Output("endTan", "The tangent vector at the end of the Circle.")]
        public static Vector EndDir(this Circle circle)
        {
            Vector n = circle.Normal;
            Vector endDir = Math.Abs(n.Z) < Math.Abs(n.X) ? new Vector { X = n.Y, Y = -n.X, Z = 0 } : new Vector { X = 0, Y = n.Z, Z = -n.Y };
            return circle.Normal.CrossProduct(endDir).Normalise();
        }

        /***************************************************/

        [Description("Gets the tangent vector at the end of the Ellipse. An ellipse starts and ends at the first axis point, and move towards the second axis, hence second axis is the tangent at start and end.")]
        [Input("ellipse", "The Ellipse from which to get the tangent at the end point.")]
        [Output("endTan", "The tangent vector at the end of the Ellipse.")]
        public static Vector EndDir(this Ellipse ellipse)
        {
            return ellipse?.Axis2;
        }

        /***************************************************/

        [Description("Gets the tangent vector at the end of the Line.")]
        [Input("line", "The Line from which to get the tangent at the end point.")]
        [Output("endTan", "The tangent vector at the end of the Line.")]
        public static Vector EndDir(this Line line)
        {
            return line.Direction();
        }

        /***************************************************/

        [Description("Gets the tangent vector at the end of the PolyCurve.")]
        [Input("curve", "The PolyCurve from which to get the tangent at the end point.")]
        [Output("endTan", "The tangent vector at the end of the PolyCurve.")]
        public static Vector EndDir(this PolyCurve curve)
        {
            return curve.Curves.Count > 0 ? curve.Curves.Last().IEndDir() : null;
        }

        /***************************************************/

        [Description("Gets the tangent vector at the end of the Polyline.")]
        [Input("curve", "The Polyline from which to get the tangent at the end point.")]
        [Output("endTan", "The tangent vector at the end of the Polyline.")]
        public static Vector EndDir(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;

            if (pts.Count < 2)
                return null;

            Point pt1 = pts[pts.Count - 2];
            Point pt2 = pts[pts.Count - 1];

            return new Vector { X = pt2.X - pt1.X, Y = pt2.Y - pt1.Y, Z = pt2.Z - pt1.Z }.Normalise();
        }


        /***************************************************/
        /**** Public Methods = Interfaces               ****/
        /***************************************************/

        [Description("Gets the tangent vector at the end of the curve.")]
        [Input("curve", "The curve from which to get the tangent at the end point.")]
        [Output("endTan", "The tangent vector at the end of the curve.")]
        public static Vector IEndDir(this ICurve curve)
        {
            return EndDir(curve as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Vector EndDir(this ICurve curve)
        {
            Base.Compute.RecordError($"EndDir is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}






