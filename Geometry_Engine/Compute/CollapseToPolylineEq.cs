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
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Collapses a PolyCurve to a polyline where curved segments are split into segments of length shortestLength")]
        [Input("curve", "The curve to collapse")]
        [Input("shortestLength", "the length of a segment the curved parts are broken up into")]
        [Output("C", "A polyline aproximating the provided curve")]
        public static Polyline CollapseToPolylineEq(this PolyCurve curve, double shortestLength = 0.1)
        {
            if (shortestLength < Tolerance.Distance)
                return null;

            List<Polyline> list = new List<Polyline>();
            foreach (ICurve c in curve.Curves)
            {
                list.Add(Divide(c as dynamic, shortestLength));
            }
            return (Geometry.Compute.Join(list))[0];
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Polyline Divide(this Line line, double shortestLength)
        {
            return new Polyline() { ControlPoints = new List<Point>() { line.Start, line.End } };
        }

        /***************************************************/

        private static Polyline Divide(this Polyline line, double shortestLength)
        {
            return new Polyline() { ControlPoints = line.ControlPoints };
        }

        /***************************************************/

        private static Polyline Divide(this PolyCurve line, double shortestLength)
        {
            return CollapseToPolylineEq(line);          //TODO
        }

        /***************************************************/

        private static Polyline Divide(this Arc arc, double shortestLength)
        {
            Polyline result = new Polyline();
            double res = Math.Floor(arc.Length() / shortestLength);
            res = res < 1 ? 1 : res;
            for (double i = 0; i <= res; i++)
                result.ControlPoints.Add(arc.PointAtParameter((i / res)));

            return result;
        }

        /***************************************************/

        private static Polyline Divide(this Circle circle, double shortestLength)
        {
            Polyline result = new Polyline();
            double res = Math.Floor(circle.Length() / shortestLength);
            res = res < 4 ? 4 : res;
            for (double i = 0; i <= res; i++)
                result.ControlPoints.Add(circle.PointAtParameter((i / res)));

            return result;
        }

        /***************************************************/

    }
}
