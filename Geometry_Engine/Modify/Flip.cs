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

using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Base.Attributes;
using System;
using System.Linq;
using System.Collections.Generic;
using BH.Engine.Base;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Flips the direction of the curve, meaning start becomes end and end becoming start, and that the tangent at each point of the curve is reversed.")]
        [Input("curve", "The curve to flip.")]
        [Output("curve", "The curve with flipped direction.")]
        public static Arc Flip(this Arc curve)
        {
            Cartesian system = Create.CartesianCoordinateSystem(curve.CoordinateSystem.Origin, curve.CoordinateSystem.X, -curve.CoordinateSystem.Y);

            return new Arc { CoordinateSystem = system, StartAngle = -curve.EndAngle, EndAngle = -curve.StartAngle, Radius = curve.Radius };
        }

        /***************************************************/

        [Description("Flips the direction of the curve, meaning start becomes end and end becoming start, and that the tangent at each point of the curve is reversed.")]
        [Input("curve", "The curve to flip.")]
        [Output("curve", "The curve with flipped direction.")]
        public static Circle Flip(this Circle curve)
        {
            return new Circle { Centre = curve.Centre, Normal = -curve.Normal, Radius = curve.Radius };
        }

        /***************************************************/

        [Description("Flips the direction of the curve, meaning start becomes end and end becoming start, and that the tangent at each point of the curve is reversed.")]
        [Input("curve", "The curve to flip.")]
        [Output("curve", "The curve with flipped direction.")]
        public static Ellipse Flip(this Ellipse curve)
        {
            if (curve.IsNull())
                return null;

            return new Ellipse {  CoordinateSystem = Create.CartesianCoordinateSystem(curve.CoordinateSystem.Origin, curve.CoordinateSystem.X, -curve.CoordinateSystem.Y), Radius1 = curve.Radius1, Radius2 = curve.Radius2 };
        }

        /***************************************************/

        [Description("Flips the direction of the curve, meaning start becomes end and end becoming start, and that the tangent at each point of the curve is reversed.")]
        [Input("curve", "The curve to flip.")]
        [Output("curve", "The curve with flipped direction.")]
        public static Line Flip(this Line curve)
        {
            return new Line { Start = curve.End, End = curve.Start };
        }

        /***************************************************/

        [Description("Flips the direction of the curve, meaning start becomes end and end becoming start, and that the tangent at each point of the curve is reversed.")]
        [Input("curve", "The curve to flip.")]
        [Output("curve", "The curve with flipped direction.")]
        public static NurbsCurve Flip(this NurbsCurve curve)
        {
            NurbsCurve result = curve.DeepClone();

            result.ControlPoints.Reverse();
            result.Weights.Reverse();
            result.Knots.Reverse();
            result.Knots = result.Knots.Select(x => -x).ToList();

            return result;
        }

        /***************************************************/

        [Description("Flips the direction of the curve, meaning start becomes end and end becoming start, and that the tangent at each point of the curve is reversed.")]
        [Input("curve", "The curve to flip.")]
        [Output("curve", "The curve with flipped direction.")]
        public static PolyCurve Flip(this PolyCurve curve)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IFlip()).Reverse().ToList() };
        }

        /***************************************************/

        [Description("Flips the direction of the curve, meaning start becomes end and end becoming start, and that the tangent at each point of the curve is reversed.")]
        [Input("curve", "The curve to flip.")]
        [Output("curve", "The curve with flipped direction.")]
        public static Polyline Flip(this Polyline curve)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Reverse<Point>().ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Flips the direction of the curve, meaning start becomes end and end becoming start, and that the tangent at each point of the curve is reversed.")]
        [Input("curve", "The curve to flip.")]
        [Output("curve", "The curve with flipped direction.")]
        public static ICurve IFlip(this ICurve curve)
        {
            return Flip(curve as dynamic);
        }

        /***************************************************/
    }
}





