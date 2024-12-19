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
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Gets points at start, quarter, mid thre quarters and end points on the Arc.")]
        [Input("curve", "The Arc to get control points from.")]
        [Output("pts", "Points at start, quarter, mid, three quarters and end along the Arc.")]
        public static List<Point> ControlPoints(this Arc curve)
        {
            //TODO: Should this give back the control points of an arc in nurbs form?
            return new List<Point>() { curve.StartPoint(), curve.PointAtParameter(0.25), curve.PointAtParameter(0.5), curve.PointAtParameter(0.75), curve.EndPoint() };
        }

        /***************************************************/

        [Description("Gets points at start, quarter, mid thre quarters and end points on the Circle.")]
        [Input("curve", "The Circle to get control points from.")]
        [Output("pts", "Points at start, quarter, mid, three quarters and end along the Circle.")]
        public static List<Point> ControlPoints(this Circle curve)
        {
            //TODO: Should this give back the control points of a circle in nurbs form?
            return new List<Point>() { curve.StartPoint(), curve.PointAtParameter(0.25), curve.PointAtParameter(0.5), curve.PointAtParameter(0.75), curve.EndPoint() };
        }

        /***************************************************/

        [Description("Gets points at the vertices and co-vertices of the Ellipse.")]
        [Input("curve", "The Ellipse to get control points from.")]
        [Output("pts", "Points at the vertices and co-vertices of the Ellipse.")]
        public static List<Point> ControlPoints(this Ellipse curve)
        {
            //TODO: Should this give back the control points of a circle in nurbs form?
            return new List<Point>()
            {
                curve.Centre + curve.Radius1*curve.Axis1,
                curve.Centre + curve.Radius2*curve.Axis2,
                curve.Centre - curve.Radius1*curve.Axis1,
                curve.Centre - curve.Radius2*curve.Axis2,
                curve.Centre + curve.Radius1*curve.Axis1,
            };
        }

        /***************************************************/

        [Description("Gets the start and end points of the Line.")]
        [Input("curve", "The Line to get control points from.")]
        [Output("pts", "The start and end points of the Line.")]
        public static List<Point> ControlPoints(this Line curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        [Description("Gets the ControlPoints of the NurbsCurve. Note that these points might not be on the curve.")]
        [Input("curve", "The NurbsCurve to get control points from.")]
        [Output("pts", "The ControlPoints of the NurbsCurve.")]
        public static List<Point> ControlPoints(this NurbsCurve curve)
        {
            return curve.ControlPoints;
        }

        /***************************************************/

        [Description("Gets the ControlPoints of the PolyCurve as the ControlPoints of all of its inner curves.")]
        [Input("curve", "The PolyCurve to get control points from.")]
        [Output("pts", "The ControlPoints of the PolyCurve.")]
        public static List<Point> ControlPoints(this PolyCurve curve)
        {
            return curve.Curves.SelectMany((x, i) => x.IControlPoints().Skip((i > 0) ? 1 : 0)).ToList();
        }

        /***************************************************/

        [Description("Gets the ControlPoints of the Polyline.")]
        [Input("curve", "The Polyline to get control points from.")]
        [Output("pts", "The ControlPoints of the Polyline.")]
        public static List<Point> ControlPoints(this Polyline curve)
        {
            return curve.ControlPoints;
        }

        /***************************************************/

        [Description("Gets the controlpoints of the Polygon. These will be the Vertices of the Polygon, with the first point duplicated as the last control point.")]
        [Input("curve", "The Polygon to extract the ControlPoints from.")]
        [Output("points", "The controlpoints of the Polygon. The first and last point will be the same.")]
        public static List<Point> ControlPoints(this Polygon curve)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Cannot extract control points from a null curve.");
                return new List<Point>();
            }
            List<Point> controlPoints =  curve.Vertices.ToList();
            controlPoints.Add(controlPoints[0]);
            return controlPoints;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the ControlPoints of the ICurve. Result will depend on the curve provided. Note that for NurbsCurves might not returns curves that are on the curve.")]
        [Input("curve", "The ICurve to get control points from.")]
        [Output("pts", "The ControlPoints of the ICurve.")]
        public static List<Point> IControlPoints(this ICurve curve)
        {
            return ControlPoints(curve as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<Point> ControlPoints(this ICurve curve)
        {
            Base.Compute.RecordError($"ControlPoints is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}




