/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        [Description("Modifies a BHoM IGeometry's coordinates to be rounded to the number of provided decimal places.")]
        [Input("geometry", "The BHoM IGeometry to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("geometry", "The modified BHoM IGeometry.")]
        public static IGeometry IRoundCoordinates(this IGeometry geometry, int decimalPlaces = 6)
        {
            return RoundCoordinates(geometry as dynamic, decimalPlaces);
        }

        /***************************************************/

        [Description("Modifies a BHoM ICurve's coordinates to be rounded to the number of provided decimal places.")]
        [Input("curve", "The BHoM ICurve to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM ICurve.")]
        public static ICurve IRoundCoordinates(this ICurve curve, int decimalPlaces = 6)
        {
            return RoundCoordinates(curve as dynamic, decimalPlaces);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Modifies a BHoM Geometry Point to be rounded to the number of provided decimal places.")]
        [Input("point", "The BHoM Geometry Point to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("point", "The modified BHoM Geometry Point.")]
        public static Point RoundCoordinates(this Point point, int decimalPlaces = 6)
        {
            return new Point
            {
                X = Math.Round(point.X, decimalPlaces),
                Y = Math.Round(point.Y, decimalPlaces),
                Z = Math.Round(point.Z, decimalPlaces),
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Line's points to be rounded to the number of provided decimal places.")]
        [Input("line", "The BHoM Geometry Line to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("line", "The modified BHoM Geometry Line.")]
        public static Line RoundCoordinates(this Line line, int decimalPlaces = 6)
        {
            return new Line()
            {
                Start = line.Start.RoundCoordinates(decimalPlaces),
                End = line.End.RoundCoordinates(decimalPlaces)
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Arc's start and end coordinates to be rounded to the number of provided decimal places.")]
        [Input("arc", "The BHoM Geometry Arc to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry Arc. As Line if zero length.")]
        public static ICurve RoundCoordinates(this Arc arc, int decimalPlaces = 6)
        {
            Point start = arc.StartPoint().RoundCoordinates(decimalPlaces);
            Point end = arc.EndPoint().RoundCoordinates(decimalPlaces);
            if (start.SquareDistance(end) == 0)
                return new Line() { Start = start, End = end };

            // ---Set start point---
            Arc c = arc.Translate(start - arc.StartPoint());

            // ---Set end point---
            // Orient towards target (rotate)
            Point pivot = start;
            Point from = c.EndPoint();
            Vector current = from - pivot;
            Vector next = end - pivot;
            Vector axis = current.CrossProduct(next);
            c = c.Rotate(pivot, axis, current.SignedAngle(next, axis));

            // scale to attatch to target
            double factor = Math.Sqrt(pivot.SquareDistance(end) / pivot.SquareDistance(from));
            Vector scaleVector = new Vector() { X = factor, Y = factor, Z = factor };
            return c.Scale(pivot, scaleVector);
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Circle's centre and radius to be rounded to the number of provided decimal places.")]
        [Input("circle", "The BHoM Geometry Circle to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry Circle. As Line if zero length.")]
        public static Circle RoundCoordinates(this Circle circle, int decimalPlaces = 6)
        {
            return new Circle()
            {
                Centre = circle.Centre.RoundCoordinates(decimalPlaces),
                Radius = Math.Round(circle.Radius, decimalPlaces),
                Normal = circle.Normal,
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Ellipse's centre and radius to be rounded to the number of provided decimal places.")]
        [Input("ellipse", "The BHoM Geometry Ellipse to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry Ellipse. As Line if zero length.")]
        public static Ellipse RoundCoordinates(this Ellipse ellipse, int decimalPlaces = 6)
        {
            return new Ellipse()
            {
                Centre = ellipse.Centre.RoundCoordinates(decimalPlaces),
                Radius1 = Math.Round(ellipse.Radius1, decimalPlaces),
                Radius2 = Math.Round(ellipse.Radius2, decimalPlaces),
                Axis1 = ellipse.Axis1,
                Axis2 = ellipse.Axis2
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Polyline's control points to be rounded to the number of provided decimal places.")]
        [Input("polyline", "The BHoM Geometry Polyline to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry Polyline.")]
        public static Polyline RoundCoordinates(this Polyline polyline, int decimalPlaces = 6)
        {
            return new Polyline()
            {
                ControlPoints = polyline.ControlPoints.Select(x => x.RoundCoordinates(decimalPlaces)).ToList()
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry PolyCurve's curves to be rounded to the number of provided decimal places.")]
        [Input("polycurve", "The BHoM Geometry PolyCurve to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry PolyCurve.")]
        public static PolyCurve RoundCoordinates(this PolyCurve polycurve, int decimalPlaces = 6)
        {
            return new PolyCurve()
            {
                Curves = polycurve.Curves.Select(x => x.IRoundCoordinates(decimalPlaces)).ToList()
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry NurbsCurve's control points to be rounded to the number of provided decimal places.")]
        [Input("nurbscurve", "The BHoM Geometry NurbsCurve to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry NurbsCurve.")]
        public static NurbsCurve RoundCoordinates(this NurbsCurve nurbscurve, int decimalPlaces = 6)
        {
            return new NurbsCurve()
            {
                ControlPoints = nurbscurve.ControlPoints.Select(x => x.RoundCoordinates(decimalPlaces)).ToList(),
                Knots = nurbscurve.Knots.ToList(),
                Weights = nurbscurve.Weights.ToList()
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry PlanarSurface's defining curves to be rounded to the number of provided decimal places.")]
        [Input("nurbscurve", "The BHoM Geometry PlanarSurface to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("surface", "The modified BHoM Geometry PlanarSurface.")]
        public static PlanarSurface RoundCoordinates(this PlanarSurface planarSurface, int decimalPlaces = 6)
        {
            ICurve externalBoundery = planarSurface.ExternalBoundary.IRoundCoordinates(decimalPlaces);
            List<ICurve> internalBounderies = planarSurface.InternalBoundaries.Select(x => x.IRoundCoordinates(decimalPlaces)).ToList();

            return Create.PlanarSurface(externalBoundery, internalBounderies);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static IGeometry RoundCoordinates(this IGeometry geometry, int decimalPlaces = 6)
        {
            throw new NotImplementedException("IGeometry of type: " + geometry.GetType().Name + " is not implemented for RoundCoordinates.");
        }

        /***************************************************/

    }
}
