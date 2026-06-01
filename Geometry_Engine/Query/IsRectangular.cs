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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines whether a Polycurve is a rectangular.")]
        [Input("polycurve", "The Polycurve to check if it is rectangular.")]
        [Output("bool", "True for Polycurves that are rectangular or false for Polycurves that are not rectangular.")]
        public static bool IsRectangular(this PolyCurve polycurve)
        {
            if (polycurve == null)
                return false;

            if (polycurve.SubParts().Any(x => !x.IIsLinear()))
                return false;

            List<Point> points = polycurve.DiscontinuityPoints();
            if (points.Count != 4)
                return false;
            if (!points.IsCoplanar())
                return false;

            List<Vector> vectors = VectorsBetweenPoints(points);

            List<double> angles = AnglesBetweenVectors(vectors);

            //Check the three angles are pi/2 degrees within tolerance
            return (angles.Any(x => Math.Abs(Math.PI / 2 - x) > Tolerance.Angle)) ? false : true;
        }

        /***************************************************/

        [Description("Checks whether a Polyline is a rectangular.")]
        [Input("polyline", "The Polyline to check if it is rectangular.")]
        [Input("tolerance", "Maximum allowed distance deviation for closure and diagonal equality checks.")]
        [Output("isRectangular", "True for the Polylines that are rectangular or false for Polylines that are not rectangular.")]
        public static bool IsRectangular(this Polyline polyline)
        {
            List<Point> pts = polyline?.ControlPoints;
            if (pts == null || pts.Count != 5)
                return false;

            if (polyline.IsPlanar(Tolerance.Distance) != true)
                return false;

            if (polyline.IsClosed(Tolerance.Distance) != true)
                return false;

            double diagonal1 = pts[2].Distance(pts[0]);
            double diagonal2 = pts[3].Distance(pts[1]);

            return Math.Abs(diagonal1 - diagonal2) <= Tolerance.Distance;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Computes the vectors between the provided list of points.")]
        [Input("points", "The list of points.")]
        [Output("vectors", "The vectors computed from the list of points.")]
        private static List<Vector> VectorsBetweenPoints(this List<Point> points)
        {
            List<Vector> vectors = new List<Vector>();

            for (int i = 0; i < points.Count; i++)
            {
                int next = (i + 1) % points.Count;
                vectors.Add(points[next] - points[i]);
            }

            return vectors;
        }

        /***************************************************/

        [Description("Gets the internal angle between sequential vectors.")]
        [Input("vectors", "The vectors to find the internal angle between.")]
        [Output("angles", "The internal angles between sequential vectors.")]
        private static List<double> AnglesBetweenVectors(this List<Vector> vectors)
        {

            List<double> angles = new List<double>();
            for (int i = 0; i < vectors.Count; i++)
            {
                int next = (i + 1) % vectors.Count;
                angles.Add(vectors[i].Angle(vectors[next]));
            }

            return angles;
        }

        /***************************************************/

    }

}
