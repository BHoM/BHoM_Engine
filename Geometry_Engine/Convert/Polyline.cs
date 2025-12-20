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

namespace BH.Engine.Geometry
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods - Curves                  ****/
        /***************************************************/

        [Description("Converts a PolyCurve to a Polyline by connecting the endpoints of its sub-curves.")]
        [Input("curve", "The PolyCurve to convert. Must contain only Line segments.")]
        [Output("polyline", "The resulting Polyline, or null if the PolyCurve contains non-linear segments.")]
        public static Polyline ToPolyline(this PolyCurve curve)
        {
            if (curve.Curves.Count == 0)
                return new Polyline();

            List<Point> controlPoints = new List<Point> { curve.Curves[0].IStartPoint() };
            foreach (ICurve c in curve.SubParts())
            {
                if (c is Line)
                    controlPoints.Add((c as Line).End);
                else
                    return null;
            }

            return new Polyline { ControlPoints = controlPoints };
        }

        /***************************************************/

        [Description("Converts a Line to a Polyline with two control points.")]
        [Input("curve", "The Line to convert.")]
        [Output("polyline", "The resulting Polyline with start and end points as control points.")]
        public static Polyline ToPolyline(this Line curve)
        {
            return new Polyline { ControlPoints = new List<Point> { curve.Start, curve.End } };
        }

        /***************************************************/

        [Description("Returns the same Polyline (identity conversion).")]
        [Input("curve", "The Polyline to return.")]
        [Output("polyline", "The same Polyline as input.")]
        public static Polyline ToPolyline(this Polyline curve)
        {
            return curve;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Converts any ICurve to a Polyline using dynamic dispatch.")]
        [Input("curve", "The ICurve to convert to a Polyline.")]
        [Output("polyline", "The resulting Polyline, or null if conversion is not possible.")]
        public static Polyline IToPolyline(this ICurve curve)
        {
            return ToPolyline(curve as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Polyline ToPolyline(this ICurve curve)
        {
            Base.Compute.RecordError($"ToPolyline is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}
