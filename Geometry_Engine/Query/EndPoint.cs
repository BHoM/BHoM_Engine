/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Gets the end Point of an Arc.")]
        [Input("arc", "The Arc to get the end point of.")]
        [Output("endPoint", "The end Point of the Arc.")]
        public static Point EndPoint(this Arc arc)
        {
            Vector locSt = arc.CoordinateSystem.X * arc.Radius;
            return arc.CoordinateSystem.Origin + locSt.Rotate(arc.EndAngle, arc.CoordinateSystem.Z);
        }

        /***************************************************/

        [Description("Gets the end Point of a Circle, which is the same as the start point for closed curves.")]
        [Input("circle", "The Circle to get the end point of.")]
        [Output("endPoint", "The end Point of the Circle.")]
        public static Point EndPoint(this Circle circle)
        {
            return circle.StartPoint();
        }

        /***************************************************/

        [Description("Gets the end Point of an Ellipse, which is the same as the start point for closed curves.")]
        [Input("ellipse", "The Ellipse to get the end point of.")]
        [Output("endPoint", "The end Point of the Ellipse.")]
        public static Point EndPoint(this Ellipse ellipse)
        {
            return ellipse.StartPoint();
        }

        /***************************************************/

        [Description("Gets the end Point of a Line.")]
        [Input("line", "The Line to get the end point of.")]
        [Output("endPoint", "The end Point of the Line.")]
        public static Point EndPoint(this Line line)
        {
            return line.End;
        }

        /***************************************************/

        [Description("Gets the end Point of a NurbsCurve. For non-periodic curves, returns the last control point.")]
        [Input("curve", "The NurbsCurve to get the end point of.")]
        [Output("endPoint", "The end Point of the NurbsCurve.")]
        public static Point EndPoint(this NurbsCurve curve)
        {
            //TODO: This should be based on the basis function?
            if (!curve.IsPeriodic())
                return curve.ControlPoints.LastOrDefault();
            else
            {
                Base.Compute.RecordError("EndPoint is not implemented for periodic NurbsCurves");
                return null;
            }
        }

        /***************************************************/

        [Description("Gets the end Point of a PolyCurve.")]
        [Input("curve", "The PolyCurve to get the end point of.")]
        [Output("endPoint", "The end Point of the PolyCurve.")]
        public static Point EndPoint(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            for (int i = curves.Count -1; i >= 0; i--)
            {
                Point End = curves[i].IEndPoint();
                if (End != null)
                    return End;
            }

            return null;
        }

        /***************************************************/

        [Description("Gets the end Point of a Polyline.")]
        [Input("curve", "The Polyline to get the end point of.")]
        [Output("endPoint", "The end Point of the Polyline.")]
        public static Point EndPoint(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.Last();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the end Point of any ICurve.")]
        [Input("curve", "The ICurve to get the end point of.")]
        [Output("endPoint", "The end Point of the curve.")]
        public static Point IEndPoint(this ICurve curve)
        {
            return EndPoint(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static Point EndPoint(this ICurve curve)
        {
            Base.Compute.RecordError("EndPoint is not implemented for curve of type: " + curve.GetType().Name + ". ");
            return null;
        }

        /***************************************************/
    }
}
